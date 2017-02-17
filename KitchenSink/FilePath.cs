using System;
using System.IO;
using static System.Environment;

namespace KitchenSink
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
        public static FilePath Profile => new FilePath(GetFolderPath(SpecialFolder.UserProfile));
        public static FilePath Desktop => new FilePath(GetFolderPath(SpecialFolder.DesktopDirectory));
        public static FilePath Documents => new FilePath(GetFolderPath(SpecialFolder.MyDocuments));
        public static FilePath AppData => new FilePath(GetFolderPath(SpecialFolder.ApplicationData));
        public static FilePath LocalAppData => new FilePath(GetFolderPath(SpecialFolder.LocalApplicationData));
        public static FilePath Programs => new FilePath(GetFolderPath(SpecialFolder.ProgramFiles));

        public static FilePath Current
        {
            get { return new FilePath(CurrentDirectory); }
            set { CurrentDirectory = value; }
        }
    }

    public static class UNC
    {
        public static FilePath Path(string host, string share)
        {
            return Host(host).Share(share);
        }

        public static UNCHost Host(string host)
        {
            return new UNCHost(host);
        }
    }

    public sealed class UNCHost : NewType<string>
    {
        internal UNCHost(string host) : base(host) {}

        public FilePath Share(string share)
        {
            return new FilePath(Path.Combine(Value.StartsWith(@"\\") ? Value : @"\\" + Value, share));
        }

        public static FilePath operator /(UNCHost host, FilePath end)
        {
            return host.Share(end.Value);
        }

        public static FilePath operator /(UNCHost host, string end)
        {
            return host.Share(end);
        }
    }

    public sealed class FilePath : NewType<string>
    {
        internal FilePath(string path) : base(path) {}

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
        public static FilePath operator /(FilePath begin, string end)
        {
            return new FilePath(Path.Combine(begin.Value, end));
        }

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(string begin, FilePath end)
        {
            return new FilePath(Path.Combine(begin, end.Value));
        }

        public static implicit operator string(FilePath filePath)
        {
            return filePath.Value;
        }
    }
}