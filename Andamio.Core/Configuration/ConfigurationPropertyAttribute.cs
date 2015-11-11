using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Configuration
{
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]    
    public sealed class SettingsPropertyAttribute : Attribute
    {        
        private SettingsPropertyAttribute()
        {        
        }        
        public SettingsPropertyAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }
    }
}
