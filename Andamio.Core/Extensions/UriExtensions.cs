using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace Andamio
{
    static public class UriExtensions
    {
        #region File
        /// <summary>
        /// Regular expression for a File Uri
        /// </summary>
        public static readonly Regex FileUriRegEx = new Regex(@"^(\w+:)?(?<path>.+)", RegexOptions.Singleline);
        
        /// <summary>
        /// If true, the scheme is a pointer to a file.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static public bool IsFileScheme(this Uri uri)
        {
            return uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase);
        }

        static public bool IsFileScheme(string uriString)
        {
            Uri uri;
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
            {
                throw new Exception(String.Format("Invalid Uri '{0}'", uriString));
            }
            return uri.IsFileScheme();
        }


        static public Uri CreateFileUri(string path)
        {
            return new Uri(String.Format("{0}:{1}", Uri.UriSchemeFile, path));
        }

        /// <summary>
        /// Returns a path to the file or folder contained in the Uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static public string GetFilePath(this Uri uri)
        {
            if (!uri.IsFileScheme())
            {
                throw new ArgumentException(String.Format("Expecting Uri scheme to be \'{0}\'.  Actual scheme is \'{1}\'."
                    , Uri.UriSchemeFile
                    , uri.Scheme));
            }

            Match match = FileUriRegEx.Match(uri.OriginalString);
            if (!match.Success)
            { throw new ArgumentException(String.Format("Invalid file Uri \'{0}\'", uri.OriginalString)); }

            string filePath = match.Groups["path"].Value.Replace('/', '\\');

            return filePath;
        }

        static public string GetFilePath(string uri)
        {
            return GetFilePath(new Uri(uri));
        }

        /// <summary>
        /// Returns the file name contained in the uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static public string GetFileName(this Uri uri)
        {

            if (uri.IsFileScheme())
            {
                return Path.GetFileName(GetFilePath(uri));
            }
            else
            {
                throw new ArgumentException(String.Format("Unexpected Uri scheme \'{0}\'.", uri.Scheme));
            }
        }

        /// <summary>
        /// Returns the Uri and local file names of the files in the specified uri that match the search pattern.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        static public Doublet<Uri, FileInfo>[] GetFiles(this Uri uri, string searchPattern, bool recursiveSearch)
        {
            if (!uri.IsFileScheme())
            {
                throw new ArgumentException(String.Format("Expecting Uri scheme to be \'{0}\'.  Actual scheme is \'{1}\'."
                    , Uri.UriSchemeFile, uri.Scheme));
            }

            string searchFilePath = uri.GetFilePath();
            if (!Directory.Exists(searchFilePath))
            {
                return new Doublet<Uri, FileInfo>[0];
            }

            string pattern = !String.IsNullOrEmpty(searchPattern) ? searchPattern : "*";

            SearchOption searchOption = (recursiveSearch) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] filePaths = Directory.GetFiles(searchFilePath, pattern, searchOption);
            if (filePaths.Length == 0)
            {
                return new Doublet<Uri, FileInfo>[0];
            }
            else
            {
                List<Doublet<Uri, FileInfo>> physicalFiles = new List<Doublet<Uri, FileInfo>>();
                foreach (string filePath in filePaths)
                {
                    Doublet<Uri, FileInfo> uriFileInfoPair = new Doublet<Uri, FileInfo>()
                    {
                        First = new Uri(String.Format("{0}:{1}", Uri.UriSchemeFile, filePath)),
                        Second = new FileInfo(filePath)
                    };

                    physicalFiles.Add(uriFileInfoPair);
                }

                return physicalFiles.ToArray();
            }
        }

        /// <summary>
        /// Returns the Uri and local file names of the files in the specified uri that match the search pattern.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        static public Doublet<Uri, FileInfo>[] GetFiles(this Uri uri, string searchPattern)
        {
            return GetFiles(uri, searchPattern, false);
        }
        #endregion
    }
}
