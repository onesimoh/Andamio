using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Diagnostics.Listeners
{
    /// <summary>
    /// Defines the type of Log Files available.
    /// </summary>
    public enum FileLogMode
    {
        /// <summary>Represents a continuous log file.</summary>
        Continuous = 1,

        /// <summary>Represents a log file that can reach only a limit size.</summary>
        Rolling = 2
    }

    public static class FileLogModeExtensions
    {
        public static bool IsContinuous(this FileLogMode mode)
        {
            return mode == FileLogMode.Continuous;
        }

        public static bool IsRolling(this FileLogMode mode)
        {
            return mode == FileLogMode.Rolling;
        }
    }
}
