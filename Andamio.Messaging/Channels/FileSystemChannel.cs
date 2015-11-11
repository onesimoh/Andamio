using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Andamio;
using Andamio.Threading;
using Andamio.Messaging.Formatters;

namespace Andamio.Messaging.Channels
{
    #region Settings
    public class FileSystemChannelSettings
    {
        public string InboxPath { get; set; }
        public string OutboxPath { get; set; }
        public string ArchivePath { get; set; }
        public string ErrorPath { get; set; }
        public string Extension { get; set; }
        public TimeSpan? Throttle { get; set; }
    }

    #endregion


    public class FileSystemChannel : BidirectionalChannel
    {
        #region Constructos
        public FileSystemChannel(string inboxPath, IMessageSerializer serializer, string fileExtension = ".txt") : base(serializer)
        {
            if (inboxPath.IsNullOrBlank()) throw new ArgumentNullException("inboxPath");
            
            DirectoryInfo parentDirectory = Directory.GetParent(inboxPath);

            Settings = new FileSystemChannelSettings();
            Settings.InboxPath = inboxPath;
            Settings.OutboxPath = Path.Combine(parentDirectory.FullName, "Outbox");
            Settings.ArchivePath = Path.Combine(parentDirectory.FullName, "Archived");
            Settings.ErrorPath = Path.Combine(parentDirectory.FullName, "Error");
            Settings.ErrorPath = Path.Combine(parentDirectory.FullName, "Error");
            Settings.Extension = fileExtension;
        }

        public FileSystemChannel(FileSystemChannelSettings settings, IMessageSerializer formatter) : base(formatter)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (settings.InboxPath.IsNullOrBlank()) throw new ArgumentException("Inbox Path not specified.");
            if (settings.OutboxPath.IsNullOrBlank()) throw new ArgumentException("Outbox Path not specified.");
            if (settings.ArchivePath.IsNullOrBlank()) throw new ArgumentException("Archive Path not specified.");
            if (settings.ErrorPath.IsNullOrBlank()) throw new ArgumentException("Error Path not specified.");
            if (settings.Extension.IsNullOrBlank()) throw new ArgumentException("File Extension not specified.");
            Settings = settings;
        }

        #endregion

        #region Settings
        public FileSystemChannelSettings Settings { get; private set; }

        #endregion

        #region Broadcasting
        public override void Publish(RequestMessage request)
        {
            try
            {
                Write(request);
            }
            catch (Exception e)
            {
                OnChannelError(e);
            }            
        }

        public override void Publish(ReplyMessage reply)
        {
            try
            {
                Write(reply);
            }
            catch (Exception e)
            {
                OnChannelError(e);
            }            
        }

        void Write(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (Settings.OutboxPath.IsNullOrBlank()) throw new InvalidOperationException("Outbox Path has not been defined.");
            if (!Directory.Exists(Settings.OutboxPath))
            { Directory.CreateDirectory(Settings.OutboxPath); }

            string fileName = String.Format("{0}-{1:yyMMdd}-1{2}-{3}{4}"
                , message.Environment
                , message.TimeStamp
                , message.CorrelationId.Substring(message.CorrelationId.Length - 5)
                , message.Event
                , Settings.Extension);

            string outputPath = Path.Combine(Settings.OutboxPath, fileName);
            using (FileStream stream = File.Open(outputPath, FileMode.CreateNew, FileAccess.Write))
            {
                Formatter.Write(stream, message);
            }
        }

        #endregion

        #region Receiving
        public override void StartListening()
        {
            FileListener watcher = FileListener.Watch(Settings.InboxPath, Settings.Throttle, Settings.Extension);
            watcher.OnIncoming((file) =>
            {
                Message message;
                using (FileStream stream = file.OpenWhenReady())
                {
                    message = Formatter.Read(stream);
                }

                WorkItem workItem;
                if (message is RequestMessage)
                {
                    workItem = OnRequestReceived((RequestMessage) message);
                }
                else if (message is ReplyMessage)
                {
                    workItem = OnReplyReceived((ReplyMessage) message);
                }
                else
                {
                    throw new NotSupportedException(String.Format("Message Type: '{0}' Is Not Supported.", message.GetType()));
                }

                if (workItem != null)
                {
                    workItem.Completed += (sender, e) => file.Move(Settings.ArchivePath);
                    workItem.Error += (sender, e) => file.Move(Settings.ErrorPath);
                }
                else
                {
                    file.Move(Settings.ArchivePath);
                }
            })
            .OnError((file, exception) =>
            {
                file.Move(Settings.ErrorPath);
                OnChannelError(exception);
            })
            .OnShutdown((exception) =>
            {
                OnChannelError(exception);
            })
            .OnRecovery(() =>
            {
                OnChannelRecovery();
            })
            .Begin();
        }

        #endregion
    }
}
