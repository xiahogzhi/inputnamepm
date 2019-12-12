using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ExtensionX
{
    public interface IStreamData
    {
        void WriteStreamData(Stream stream);
        void ReadStreamData(Stream stream);
    }
}