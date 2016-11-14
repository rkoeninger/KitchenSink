using System;
using System.IO;
using static System.Environment;

namespace ZedSharp
{
    public static class Drive
    {
        public static FilePath Letter(char letter)
        {
            if (! char.IsLetter(letter))
            {
                throw new ArgumentException("Drive letter must be a letter, instead it was: " + letter);
            }

            return new FilePath(letter + @":\");
        }

        public static readonly FilePath A = Letter('A');
        public static readonly FilePath B = Letter('B');
        public static readonly FilePath C = Letter('C');
        public static readonly FilePath D = Letter('D');
        public static readonly FilePath E = Letter('E');
        public static readonly FilePath F = Letter('F');
        public static readonly FilePath G = Letter('G');

        public static readonly FilePath System = new FilePath(Path.GetPathRoot(GetFolderPath(SpecialFolder.System)));
    }

    public static class Folder
    {
        public static FilePath Profile
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.UserProfile)); }
        }

        public static FilePath Desktop
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.DesktopDirectory)); }
        }

        public static FilePath Documents
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.MyDocuments)); }
        }

        public static FilePath AppData
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.ApplicationData)); }
        }

        public static FilePath LocalAppData
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.LocalApplicationData)); }
        }

        public static FilePath Programs
        {
            get { return new FilePath(GetFolderPath(SpecialFolder.ProgramFiles)); }
        }

        public static FilePath Current
        {
            get { return new FilePath(CurrentDirectory); }
            set { CurrentDirectory = value; }
        }
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
            return new FilePath(Path.Combine(Value.StartsWith(@"\\") ? Value : @"\\" + Value, share));
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
            return new FilePath(Path.Combine(begin.Value, end.Value));
        }

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(FilePath begin, String end)
        {
            return new FilePath(Path.Combine(begin.Value, end));
        }

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(String begin, FilePath end)
        {
            return new FilePath(Path.Combine(begin, end.Value));
        }

        public static implicit operator String(FilePath filePath)
        {
            return filePath.Value;
        }
    }
}