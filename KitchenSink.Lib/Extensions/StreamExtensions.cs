using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KitchenSink.Extensions
{
    public static class StreamExtensions
    {
        public static IEnumerable<byte> AsEnumerable(this Stream stream)
        {
            for (int b; (b = stream.ReadByte()) >= 0;)
            {
                yield return (byte)b;
            }
        }

        public static IEnumerable<string> AsEnumerable(this TextReader reader)
        {
            for (string line; (line = reader.ReadLine()) != null;)
            {
                yield return line;
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

        public static Stream ToStream(this IEnumerable<byte> bytes)
        {
            return new EnumerableStream(bytes);
        }

        public static Stream ToStream<A>(this IEnumerable<A> seq, Func<A, IEnumerable<byte>> f)
        {
            return new EnumerableStream(seq.SelectMany(f));
        }

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

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
