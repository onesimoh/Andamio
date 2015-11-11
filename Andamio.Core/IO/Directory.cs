using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Security.AccessControl;

using Andamio;

namespace Andamio.IO
{
	/// <summary>
	/// Encapsulates Directory functionality.
	/// </summary>
	static public class DirectoryFunctions
    {
        #region XCopy
        /// <summary>
		/// Recursively copies files and directory trees.
		/// </summary>
		/// <param name="srcPath">The source path.</param>
		/// <param name="destPath">The destination path.</param>
		static public void XCopy(string srcPath, string destPath)
		{
			// Initialize the ProcessStartInfo with "xcopy.exe" and arguments
			ProcessStartInfo xcopyProcStartInfo = new ProcessStartInfo("xcopy.exe");
			xcopyProcStartInfo.CreateNoWindow = true;
			xcopyProcStartInfo.UseShellExecute = false;
			xcopyProcStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			xcopyProcStartInfo.Arguments = @"""" + srcPath + @"""" + " " + @"""" + destPath + @"""" + " /c /s /e /y /h";

			// Start "xcopy.exe"
			Process xcopyProcess = Process.Start(xcopyProcStartInfo);
			xcopyProcess.WaitForExit();
		}
		#endregion

		#region ReadOnly
		static public void MakeWriteable(this DirectoryInfo dirInfo)
		{
			FileAttributes attributes = dirInfo.Attributes;
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				dirInfo.Attributes &= ~FileAttributes.ReadOnly;
			}
		}
		
        static public void MakeWriteable(string path, bool recursive)
		{
			DirectoryInfo[] dirInfos;
			FileInfo[] fileInfos;
			if (recursive)
			{
				string[] directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
				dirInfos = directories.Select(d => new DirectoryInfo(d)).ToArray();

				string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
				fileInfos = files.Select(f => new FileInfo(f)).ToArray();
			}
			else
			{
				dirInfos = new DirectoryInfo[1] { new DirectoryInfo(path) };
				string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
				fileInfos = files.Select(f => new FileInfo(f)).ToArray();
			}			
			
			foreach (DirectoryInfo dirInfo in dirInfos)
			{
				dirInfo.MakeWriteable();
			}

			foreach (FileInfo fileInfo in fileInfos)
			{
				fileInfo.MakeWriteable();
			}
		}
		#endregion

		#region Delete
		/// <summary>
		/// This method will delete all of the directory's contents including read-only contents.
		/// </summary>
		/// <remarks>
		/// The .Net method, System.IO.Directory.Delete method is very sensitive to read-only files.
		/// </remarks>
		/// <param name="path"></param>
		/// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
		static public void Delete(string path, bool recursive)
		{
			// The System.IO.Directory.Delete() method will fail with UnauthorizedAccessException 
			// if any files in the directory tree have the read-only flag.
			MakeWriteable(path, recursive);
			Directory.Delete(path, recursive);
		}

		/// <summary>
		/// This method will delete all of the directory's contents including read-only contents.
		/// </summary>
		/// <remarks>
		/// The .Net method, System.IO.Directory.Delete method is very sensitive to read-only files.
		/// </remarks>
		/// <param name="path"></param>
		static public void Delete(string path)
		{
			Delete(path, false);
		}

		#endregion

        #region Create
        public static void Create(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                int result = Interop._mkdir(directoryPath);
                if (result != 0)
                {
                    throw new Exception(String.Format("Error calling Unmanaged [msvcrt.dll]:_wmkdir('{0}'). Error Code: {1}."
                        , directoryPath
                        , result));
                }
            }
        }

        #endregion

        #region Network Share (WMI)
        /*
		static private ManagementObject GetNetworkShareObject(string machineName, string shareName)
		{
            ManagementScope machineScope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machineName));     

			// Get all of the network shares
			string query = String.Format("Select * From Win32_Share Where Type = 0 AND Name = \'{0}\'", shareName);
			ObjectQuery objQuery = new ObjectQuery(query);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(machineScope, objQuery);
			ManagementObjectCollection queryResults = searcher.Get();
			return queryResults.Cast<ManagementObject>().FirstOrDefault();
		}
		/// <summary>
		/// Determines if a network share exists.
		/// </summary>
		/// <param name="machineName"></param>
		/// <param name="shareName"></param>
		/// <returns></returns>
		static public bool NetworkShareExists(string machineName, string shareName)
		{
			string path;
			return NetworkShareExists(machineName, shareName, out path);
		}
		/// <summary>
		/// Determines if a network share exists
		/// </summary>
		/// <param name="machineName"></param>
		/// <param name="shareName"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		static public bool NetworkShareExists(string machineName, string shareName, out string path)
		{
			path = null;
			ManagementObject mgmtObj = GetNetworkShareObject(machineName, shareName);
			if (mgmtObj == null)
				return false;
            path = String.Format(@"\\{0}\{1}", machineName, shareName);
            return true;
		}
		/// <summary>
		/// Creates a network share.
		/// </summary>
		/// <param name="machineName"></param>
		/// <param name="path"></param>
		/// <param name="shareName"></param>
		/// <param name="description"></param>
		static public void CreateNetworkShare(string machineName
			, string path
			, string shareName
			, string description)
		{
            string[] pathElements = path.Split(':');
            string drive = pathElements[0];
            string folder = pathElements[1];

            string share = String.Format(@"\\{0}\{1}$\{2}", machineName, drive, folder);

            if (!Directory.Exists(share))
                Directory.CreateDirectory(share);

            // Use WMI to create the network share.
            ManagementScope mgmtScope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machineName));
            ObjectGetOptions objGetOptions = new ObjectGetOptions();
            ManagementPath managementPath = new ManagementPath("Win32_Share");
            ManagementClass networkShare = new ManagementClass(mgmtScope, managementPath, objGetOptions);

            //List<object> createArgs = new List<object>();
            ManagementBaseObject createArgs = networkShare.GetMethodParameters("Create");
            ManagementBaseObject resultParms;

            // Set the input parameters
            createArgs["Path"] = path;
            createArgs["Name"] = shareName;
            createArgs["Description"] = description;
            createArgs["Access"] = null;
            createArgs["Type"] = 0x0; // Disk Drive

            resultParms = networkShare.InvokeMethod("Create", createArgs, null);
            uint result = (uint)resultParms.Properties["ReturnValue"].Value;

            // Success
            if (result == 0)
                return;

			switch (result)
			{
				// Access Denied
				case 2:
					throw new System.ComponentModel.Win32Exception((int)result);

				case 9:
					throw new ArgumentException("Invalid Name", "shareName");

				case 22:
					throw new Exception(String.Format("Share already exists. Name={0}", shareName));
				case 24:
					throw new DirectoryNotFoundException("path");

				// Unknown Failure
				default:
					throw new Exception(String.Format("Unknown failure while creating network share. Result={0}", result));

			}
		}

		/// <summary>
		/// Removes the network share.
		/// </summary>
		/// <param name="machineName"></param>
		/// <param name="shareName"></param>
		static public void RemoveNetworkShare(string machineName, string shareName)
		{
			ManagementObject mgmtObj = GetNetworkShareObject(machineName, shareName);
			if (mgmtObj != null)
			{
				uint result = (uint)mgmtObj.InvokeMethod("Delete", null);

				// Success
				if (result == 0)
					return;

				switch (result)
				{
					// Access Denied
					case 2:
						throw new System.ComponentModel.Win32Exception((int)result);

					case 9:
						throw new ArgumentException("Invalid Name", "shareName");

					// Unknown Failure
					default:
						throw new Exception(String.Format("Unknown failure while removing network share. Result={0}", result));
				}
			}
		}

        /// <summary>
        /// Sets full share permissions on the network share.
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="shareName"></param>
        static public void SetNetworkSharePermissions(string machineName, string shareName)
        {
            //Select user to apply security settings
            NTAccount ntAccount = new NTAccount("Everyone");

            //SID
            SecurityIdentifier userSID = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
            byte[] utenteSIDArray = new byte[userSID.BinaryLength];
            userSID.GetBinaryForm(utenteSIDArray, 0);

            //Trustee
            ManagementObject userTrustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
            userTrustee["Name"] = "Everyone";
            userTrustee["SID"] = utenteSIDArray;

            //ACE
            ManagementObject userACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
            userACE["AccessMask"] = 2032127;                                 //Full access
            userACE["AceFlags"] = AceFlags.ObjectInherit | AceFlags.ContainerInherit;
            userACE["AceType"] = AceType.AccessAllowed;
            userACE["Trustee"] = userTrustee;

            ManagementObject userSecurityDescriptor = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
            userSecurityDescriptor["ControlFlags"] = 4; //SE_DACL_PRESENT 
            userSecurityDescriptor["DACL"] = new object[] { userACE };

            //Update security permissions on share
            ManagementClass mc = new ManagementClass("Win32_Share");
            ManagementObject mgmtObj = GetNetworkShareObject(machineName, shareName);
            uint result = (uint)mgmtObj.InvokeMethod("SetShareInfo", new object[] { Int32.MaxValue, null, userSecurityDescriptor });

            if (result == 0)
                return;

            switch (result)
            {
                // Access Denied
                case 2:
                    throw new System.ComponentModel.Win32Exception((int)result);

                // Unknown Failure
                default:
                    throw new Exception(String.Format("Unknown failure while updating network share permissions. Result={0}", result));
            }
        }
        */
		#endregion
	}
}
