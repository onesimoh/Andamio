using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Threading
{
    /// <summary>
    /// Represents a parameter-less method.
    /// </summary>
    public delegate void GenericEventHandler();

    /// <summary>
    /// Represents a method that takes one parameter.
    /// </summary>
    /// <typeparam name="T">Generic Type of the parameter.</typeparam>
    /// <param name="value">Method parameter.</param>
    public delegate void GenericEventHandler<T>(T value);

    /// <summary>
    /// Represents a method that takes two parameter. 
    /// </summary>
    /// <typeparam name="T1">Generic Type of the first parameter.</typeparam>
    /// <typeparam name="T2">Generic Type of the second parameter.</typeparam>
    /// <param name="arg1">First parameter.</param>
    /// <param name="arg2">Second parameter.</param>
    public delegate void GenericEventHandler<T1, T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// Represents a method that takes two parameter. 
    /// </summary>
    /// <typeparam name="T1">Generic Type of the first parameter.</typeparam>
    /// <typeparam name="T2">Generic Type of the second parameter.</typeparam>
    /// <typeparam name="T2">Generic Type of the third parameter.</typeparam>
    /// <param name="arg1">First parameter.</param>
    /// <param name="arg2">Second parameter.</param>
    /// <param name="arg3">Third parameter.</param>
    public delegate void GenericEventHandler<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Represents a method that takes two parameter. 
    /// </summary>
    /// <typeparam name="T1">Generic Type of the first parameter.</typeparam>
    /// <typeparam name="T2">Generic Type of the second parameter.</typeparam>
    /// <typeparam name="T2">Generic Type of the third parameter.</typeparam>
    /// <typeparam name="T4">Generic Type of the fourth parameter.</typeparam>
    /// <param name="arg1">First parameter.</param>
    /// <param name="arg2">Second parameter.</param>
    /// <param name="arg3">Third parameter.</param>
    /// <param name="arg4">Fourth parameter.</param>
    public delegate void GenericEventHandler<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Represents a method that will execute a specified method and arguments.
    /// </summary>
    /// <param name="del">Method to Invoke.</param>
    /// <param name="args">Arguments to pass into the method to Invoke.</param>
    public delegate void AsyncFire(Delegate del, object[] args);
}
