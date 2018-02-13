using System;
using System.IO;

namespace System.IO
{
    public static class StreamExt
    {
        public static void SaveToFile(this Stream stream, string file)
        {
            using (var fileStream = File.Create(file))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }
}
