using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio
{
    /// <summary>
    /// Basic class that stores two related objects.
    /// </summary>
    /// <remarks>
    /// This class functions very similar to KeyValuePair, the only difference being that it is defined as a class 
    /// therefore it may be set to null.
    /// </remarks>
    /// <typeparam name="Tfirst">Generic type T of the first element in the relationship.</typeparam>
    /// <typeparam name="Tsecond">Generic type T of the second element in the relationship.</typeparam>
    [Serializable]
    public class Doublet<Tfirst, Tsecond>
    {
        /// <summary>
        /// Default RelationPair Constructors.
        /// </summary>
        public Doublet()
        {
            First = default(Tfirst); 
            Second = default(Tsecond);              
        }
        
        /// <summary>
        /// Constructs RelationPair object using the specified pair.
        /// </summary>
        /// <param name="first">Generic type of the first object pair.</param>
        /// <param name="second">Generic type of the second object pair.</param>
        public Doublet( Tfirst first, Tsecond second )
        {
            First = first; 
            Second = second;   
        }
        
        /// <summary>
        /// Gets or sets first object pair.
        /// </summary>
        public Tfirst First { get; set; }
        
        /// <summary>
        /// Gets or sets second object pair.
        /// </summary>        
        public Tsecond Second { get; set; }
        
        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>Returns string representation of the object in the format First - Second.</returns>
        public override string ToString()
        {
            return String.Format( "{0}-{1}", this.First.ToString(), this.Second.ToString() );
        }        
    }
}
