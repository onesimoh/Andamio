using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Andamio
{
    #region Exceptions
    public class CtxProcessException : Exception
    {
        public CtxProcessException()
            : base()
        { }

        public CtxProcessException(string message)
            : base(message)
        { }

        public CtxProcessException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public sealed class CtxProcessTimeoutException : CtxProcessException
    {
        public CtxProcessTimeoutException()
            : base()
        { }

        public CtxProcessTimeoutException(string message)
            : base(message)
        { }

        public CtxProcessTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    #endregion


    /// <summary>
    /// Utility to enable command-line application execution.
    /// </summary>
    public static class CtxProcess
    {
        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string[] args, out string processOutput)
        {
            return Run(applicationFullName, workingDir, null, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string[] args, out string processOutput)
        {
            return Run(applicationFullName, null, null, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <param name="standardError">Standard error output.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string args, out string processOutput, out string standardError)
        {
            return Run(applicationFullName, null, null, args, 0, out processOutput, out standardError);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string[] args, int timeout, out string processOutput)
        {
            return Run(applicationFullName, workingDir, null, args, timeout, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string[] args, int timeout, out string processOutput)
        {
            return Run(applicationFullName, null, null, args, timeout, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="stdInput">Application input.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string stdInput, string[] args, out string processOutput)
        {
            return Run(applicationFullName, workingDir, stdInput, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="stdInput">Application input.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string stdInput, string[] args, int timeout, out string processOutput)
        {
            string arguments = (args != null && args.Length > 0) ? String.Join(" ", args) : String.Empty;
            return Run(applicationFullName, workingDir, stdInput, arguments, timeout, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string args, out string processOutput)
        {
            return Run(applicationFullName, workingDir, null, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string args, out string processOutput)
        {
            return Run(applicationFullName, null, null, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string workingDir, string args, int timeout, out string processOutput)
        {
            return Run(applicationFullName, workingDir, null, args, 0, out processOutput);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="processOutput">Contains the output produced when application is executed.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName, string args, int timeout, out string processOutput)
        {
            return Run(applicationFullName, null, null, args, 0, out processOutput);
        }


        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="stdInput">Application input.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="standardOutput">Contains the output produced when application is executed.</param>
        /// <param name="standardError">Standard error output.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName
            , string workingDir
            , string stdInput
            , string args
            , int timeout
            , out string standardOutput)
        {
            string standardError;
            return Run(applicationFullName, workingDir, stdInput, args, timeout, out standardOutput, out standardError);
        }

        /// <summary>
        /// Runs the specified applicaiton in the backgroud.
        /// </summary>
        /// <param name="applicationFullName">The fully qualified path of the application to run.</param>
        /// <param name="workingDir">The directory for the process to start.</param>
        /// <param name="stdInput">Application input.</param>
        /// <param name="args">The command-line arguments to pass to the application.</param>
        /// <param name="standardOutput">Contains the output produced when application is executed.</param>
        /// <param name="standardError">Standard error output.</param>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is
        /// started (for example, if an existing process is reused).
        ///</returns>
        public static int Run(string applicationFullName
            , string workingDir
            , string stdInput
            , string args
            , int timeout
            , out string standardOutput
            , out string standardError)
        {
            Process process = null;
            try
            {
                process = new Process();
                standardOutput = String.Empty;

                //set properties for running the process
                process.StartInfo.FileName = applicationFullName;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = args;

                if (!String.IsNullOrEmpty(workingDir))
                    process.StartInfo.WorkingDirectory = workingDir;

                process.Start();

                if (!String.IsNullOrEmpty(stdInput))
                {
                    StreamWriter writer = process.StandardInput;
                    writer.Write(stdInput);
                    writer.Close();
                }

                standardOutput = process.StandardOutput.ReadToEnd();
                standardError = process.StandardError.ReadToEnd();

                // Wait for the sort process to write the sorted text lines.
                if (timeout > 0)
                {
                    if (!process.WaitForExit(timeout))
                    {
                        string timeoutErr = String.Format("'{0}' did not complete within the allowed time of {1} milliseconds. Operation Timeout."
                            , applicationFullName
                            , timeout);
                        throw new CtxProcessTimeoutException(timeoutErr);
                    }
                }
                else
                {
                    process.WaitForExit();
                }

                int exitCode = process.ExitCode;
                return exitCode;
            }
            catch (Exception e)
            {
                throw new CtxProcessException(String.Format("An error ocurred while running '{0}'.", applicationFullName), e);
            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    process.Kill();
                }
            }
        }
    }
}
