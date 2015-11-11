using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio
{
    public interface IDownloadableContent
    {
        string FileName();
        string ContentType();
        long WriteTo(Stream stream);
        Task WriteToAsync(Stream stream);
    }
}
