using System;
using P = System.IO.Path;

namespace ZedSharp
{
    public static class Drive
    {
        public static FilePath Letter(char letter)
        {
            return new FilePath(letter + @":\");
        }

        public static readonly FilePath C = Letter('C');
        public static readonly FilePath D = Letter('D');
        public static readonly FilePath E = Letter('E');

        // TODO Drive.Windows - looks up drive running OS is installed on
        // class Folder, TODO Folder.Home, Folder.RecyclingBin, Folder.Programs...
        // etc
    }

    public static class UNC
    {
        public static FilePath Path(String host, String share)
        {
            return Host(host).Share(share);
        }

        public static UNCHost Host(String host)
        {
            return new UNCHost(host);
        }
    }

    public sealed class UNCHost : NewType<String>
    {
        internal UNCHost(String host) : base(host) {}

        public FilePath Share(String share)
        {
            return new FilePath(P.Combine(Value.StartsWith(@"\\") ? Value : @"\\" + Value, share));
        }

        public static FilePath operator /(UNCHost host, FilePath end)
        {
            return host.Share(end.Value);
        }

        public static FilePath operator /(UNCHost host, String end)
        {
            return host.Share(end);
        }
    }

    public sealed class FilePath : NewType<String>
    {
        internal FilePath(String path) : base(path) {}

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(FilePath begin, FilePath end)
        {
            return new FilePath(P.Combine(begin.Value, end.Value));
        }

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(FilePath begin, String end)
        {
            return new FilePath(P.Combine(begin.Value, end));
        }

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(String begin, FilePath end)
        {
            return new FilePath(P.Combine(begin, end.Value));
        }

        public static implicit operator String(FilePath filePath)
        {
            return filePath.Value;
        }
    }
}