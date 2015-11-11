using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Threading;

using Andamio;
using Andamio.Threading;

namespace Andamio
{
    public sealed class FileListener : Disposable
    {
        #region Events
        private Action<FileInfo> _OnIncoming;
        public FileListener OnIncoming(Action<FileInfo> method)
        {
            _OnIncoming += method;
            return this;
        }

        private Action<FileInfo, Exception> _OnError;
        public FileListener OnError(Action<FileInfo, Exception> method)
        {
            _OnError += method;
            return this;
        }

        private Action<Exception> _OnShutdown;
        public FileListener OnShutdown(Action<Exception> method)
        {
            _OnShutdown += method;
            return this;
        }

        private Action _OnRecovery;
        public FileListener OnRecovery(Action method)
        {
            _OnRecovery += method;
            return this;
        }

        #endregion

        #region Constructors
        private FileListener()
        {
        }

        private FileListener(string path, TimeSpan? interval = null, string extension = null) : this()
        {
            if (path.IsNullOrBlank()) throw new ArgumentNullException("path");
            Path = path;
            Extension = extension;
            Interval = interval;
            FileQueue = new WorkerThreadQueue();
        }

        public static FileListener Watch(string path, TimeSpan? interval = null, string extension = null)
        {
            return new FileListener(path, interval, extension);
        }

        #endregion

        #region Watcher
        public string Path { get; private set; }
        public string Extension { get; private set; }
        public TimeSpan? Interval { get; set; }

        FileSystemWatcher _watcher;
        RecurringTask _recurring;
        public void Begin()
        {
            if (!Extension.IsNullOrBlank())
            { 
                string extension = Extension.StartsWith(".") ? String.Format("*{0}", Extension) : String.Format("*.{0}", Extension);
                _watcher = new FileSystemWatcher(Path, extension);
            }
            else
            {
                _watcher = new FileSystemWatcher(Path);
            }
            
            //_watcher.NotifyFilter = NotifyFilters.;
            _watcher.Created += OnIncomingFile;
            //_watcher.Changed += OnIncomingFile;
            _watcher.Error += OnWatcherError;            

            Action task = () => 
            {
                if (Directory.Exists(Path))
                { Directory.GetFiles(Path).Select(f => new FileInfo(f)).ForEach(f => QueueIncomingFile(f)); }            
            };
            Task.Run(task).Wait();
            
            FileQueue.Start();            
            _watcher.EnableRaisingEvents = true;
            if (Interval.HasValue)
            { 
                _recurring = task.AsRecurring(Interval.Value).Start();
            }
        }

        #endregion

        #region Event Handling
        private readonly WorkerThreadQueue FileQueue;

        void OnIncomingFile(object source, FileSystemEventArgs e)
        {
            FileInfo file = new FileInfo(e.FullPath);
            QueueIncomingFile(file);
        }

        void QueueIncomingFile(FileInfo file)
        {
            try
            {
                var queuedFiles = FileQueue.Items.Select(f => f.AsyncState as FileInfo).Where(f => f != null);
                if (!queuedFiles.Any(f => f.Name.Equals(file.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    Action handler = () => _OnIncoming.SafeInvoke(file);
                    WorkItem workItem = new WorkItem(file, handler);
                    workItem.Error += (sender, e) => _OnError.SafeInvoke(file, e.Item);
                    FileQueue.QueueWorkItem(workItem);                    
                }
            }
            catch (Exception exception)
            {
                _OnError.SafeInvoke(file, exception);
            }
        }


        #endregion

        #region Error Handling
        void OnWatcherError(object sender, ErrorEventArgs e)
        {
            DestroyFileWatcher();
            _OnShutdown.SafeInvoke(e.GetException());

            while (!Directory.Exists(Path))
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }

            Begin();
            _OnRecovery.SafeInvoke();
        }

        #endregion

        #region Disposable
        protected override void Cleanup()
        {
            if (_watcher != null)
            { DestroyFileWatcher(); }            
        }

        void DestroyFileWatcher()
        {
            if (_watcher != null && _watcher.EnableRaisingEvents)
            {
                _watcher.EnableRaisingEvents = false;
            }

            if (_watcher != null)
            { _watcher.Dispose(); }

            FileQueue.Stop();

            if (_recurring != null)
            {
                _recurring.Dispose();
            }
        }

        #endregion
    }
}
