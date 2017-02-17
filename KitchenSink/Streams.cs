using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KitchenSink
{
    public static class Streams
    {
        public static Stream ToStream(this string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(str);
            return new MemoryStream(bytes);
        }

        public static Stream ToStream(this IEnumerable<string> seq, Encoding encoding = null, string separator = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            separator = separator ?? "";
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

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { throw new NotImplementedException(); }
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override long Position
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
    }
}
