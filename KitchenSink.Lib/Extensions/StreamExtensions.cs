using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KitchenSink.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadToEnd(this Stream source)
        {
            using (var memory = new MemoryStream())
            {
                source.CopyTo(memory);
                memory.Position = 0;
                return memory.ToArray();
            }
        }

        public static string ReadTextToEnd(this Stream source, Encoding encoding = null) =>
            source.AsReader().ReadToEnd();

        public static StreamReader AsReader(this Stream stream, Encoding encoding = null) =>
            new StreamReader(stream, encoding ?? Encoding.UTF8);

        public static StreamWriter AsWriter(this Stream stream, Encoding encoding = null) =>
            new StreamWriter(stream, encoding ?? Encoding.UTF8);

        public static IEnumerable<byte> AsEnumerable(this Stream stream)
        {
            using (stream)
            {
                for (int b; (b = stream.ReadByte()) >= 0;)
                {
                    yield return (byte)b;
                }
            }
        }

        public static IEnumerable<char> AsEnumerableChars(this TextReader reader)
        {
            using (reader)
            {
                for (int ch; (ch = reader.Read()) >= 0;)
                {
                    yield return Convert.ToChar(ch);
                }
            }
        }

        public static IEnumerable<string> AsEnumerableLines(this TextReader reader)
        {
            using (reader)
            {
                for (string line; (line = reader.ReadLine()) != null;)
                {
                    yield return line;
                }
            }
        }

        public static Stream ToStream(this string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(str);
            return new MemoryStream(bytes);
        }

        public static Stream ToStream(this IEnumerable<string> seq, Encoding encoding = null, string separator = "")
        {
            encoding = encoding ?? Encoding.UTF8;
            return new EnumerableStream(seq.Intersperse(separator).SelectMany(encoding.GetBytes));
        }

        public static Stream ToStream(this IEnumerable<byte> bytes) => new EnumerableStream(bytes);

        public static Stream ToStream<A>(this IEnumerable<A> seq, Func<A, IEnumerable<byte>> f) => new EnumerableStream(seq.SelectMany(f));

        private class EnumerableStream : Stream
        {
            private readonly IEnumerator<byte> Bytes;

            public EnumerableStream(IEnumerable<byte> bytes)
            {
                Bytes = bytes.GetEnumerator();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var bytesRead = 0;

                for (var i = offset; i < offset + count && Bytes.MoveNext(); ++i)
                {
                    buffer[i] = Bytes.Current;
                    bytesRead++;
                }

                return bytesRead;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush() => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}
