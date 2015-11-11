using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Defines a generalized comparison method that a class implements to create a type-specific comparison method. 
    /// </summary>
    public interface IEquatable
    {
        /// <summary>
        /// Compare the current instace with another instance.
        /// </summary>
        /// <param name="objValue">An object to compare with this instance.</param>
        /// <returns>true if both instances are "Equal"; otherwise, false.</returns>
        bool Equals(object objValue);
    }
}
