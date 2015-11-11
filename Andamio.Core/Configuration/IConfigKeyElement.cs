using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Configuration
{
    /// <summary>
    /// Provides a unique key element that indentifies a configuration element.
    /// </summary>
    public interface IConfigKeyElement
    {
        /// <summary>
        /// Gets or sets the unique key that indentifies a configuration element.
        /// </summary>
        string Key{ get; set; }
    }
}
