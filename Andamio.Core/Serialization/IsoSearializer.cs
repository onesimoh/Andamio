using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Andamio.Serialization
{
    /// <summary>
    /// Utility class that provides functionality for Serialization into Isolated Storage.
    /// </summary>
    public static class IsoSerializer
    {
        /// <summary>
        /// Deserializes and reconstitutes the specified type from Isolated Storage.
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to deserialize.</typeparam>
        /// <param name="filename">The relative path of the file within isolated storage.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        public static T DeSerialize<T>( string filename ) where T : new()
        {
            try
            {
                IsolatedStorageFile isf = GetIsoStore();
                
                using( Stream reader = new IsolatedStorageFileStream( filename, FileMode.OpenOrCreate, isf ) )
                {
                    if( reader.Length > 0 )
                    {
                        GenericBinaryFormatter gf = new GenericBinaryFormatter();
                        T obj = (T) gf.Deserialize<T>( reader );
                        reader.Close();
                        
                        return obj;
                    }
                }
            }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                throw new ApplicationException( "ReadFromStorage failed:" + ex.Message );
            }

            return default(T);
        }    
        
        /// <summary>
        /// Serializes an object, or graph of objects with the given root to Isolated Storage.
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to serialize.</typeparam>
        /// <param name="filename">The relative path of the file within isolated storage.</param>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>    
        public static void Serialize<T>( string filename, T graph )
        {
            try
            {
                IsolatedStorageFile isf = GetIsoStore();
                using( Stream writer = new IsolatedStorageFileStream( filename, FileMode.Create, FileAccess.Write, isf ) )
                {
                    GenericBinaryFormatter gf = new GenericBinaryFormatter();
                    gf.Serialize<T>( writer, graph );
                    writer.Close();
                }
            }
            catch( FileNotFoundException ) { }
            catch( Exception ex )
            {
                throw new ApplicationException( "WriteToStorage failed:" + ex.Message );
            }
        }
        
        /// <summary>
        /// Saves a string value to isolated storage.
        /// </summary>
        /// <param name="filename">The relative path of the file within isolated storage.</param>
        /// <param name="value">String value to save to isolated storage.</param>
        public static void SaveToIsoStore( string filename, string value )
        {
            IsolatedStorageFile isf = GetIsoStore();
            
            try
            {
                using( Stream isoStream = new IsolatedStorageFileStream( filename, FileMode.Create, FileAccess.Write, isf ) )
                {
                    using( StreamWriter streamWriter = new StreamWriter( isoStream ) )
                    {
                        streamWriter.Write( value );
                    }
                }
            }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                throw new ApplicationException("WriteToStorage failed:" + ex.Message);
            }        
        }
        
        /// <summary>
        /// Retreives a string value to isolated storage.
        /// </summary>
        /// <param name="filename">The relative path of the file within isolated storage.</param>
        /// <returns>A string value.</returns>
        public static string LoadFromIsoStore( string filename )
        {
            IsolatedStorageFile isf = GetIsoStore();
            
            string value = String.Empty;
            
            try
            {
                using( Stream isoStream = new IsolatedStorageFileStream( filename, FileMode.OpenOrCreate, isf ) )
                {
                    if( isoStream.Length > 0 )
                    {
                        using( StreamReader streamReader = new StreamReader( isoStream ) )
                        {
                            value = streamReader.ReadToEnd();
                        }
                    }                    
                }
            }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                throw new ApplicationException("WriteToStorage failed:" + ex.Message);
            }
            
            return value;        
        }
        
        #region Private Methods
        private static IsolatedStorageFile GetIsoStore()
        {
            IsolatedStorageFile isf = null;
            
            // Test for AppDomain.CurrentDomain.ActivationContext value, if null it means application isn't running under ClickOnce
            // otherwise get Store for Application and User.
            if( AppDomain.CurrentDomain.ActivationContext == null )
                isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            else
                isf = IsolatedStorageFile.GetUserStoreForApplication();
                
            return isf;
        }
        #endregion
    }
}
