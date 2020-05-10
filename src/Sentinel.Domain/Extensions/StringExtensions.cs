using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sentinel.Domain.Extensions
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
