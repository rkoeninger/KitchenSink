using System;
using System.IO;
using static System.Environment;

namespace KitchenSink
{
    /// <summary>
    /// Starts a path at the root of a drive letter. Windows only.
    /// </summary>
    public static class Drive
    {
        /// <summary>
        /// Starts a path at the root of the given drive letter.
        /// </summary>
        public static FilePath Letter(char letter)
        {
            if (! char.IsLetter(letter))
            {
                throw new ArgumentException("Drive letter must be a letter, instead it was: " + letter);
            }

            return new FilePath(letter + @":\");
        }

        /// <summary>Starts a path at A:\</summary>
        public static readonly FilePath A = Letter('A');

        /// <summary>Starts a path at B:\</summary>
        public static readonly FilePath B = Letter('B');

        /// <summary>Starts a path at C:\</summary>
        public static readonly FilePath C = Letter('C');

        /// <summary>Starts a path at D:\</summary>
        public static readonly FilePath D = Letter('D');

        /// <summary>Starts a path at E:\</summary>
        public static readonly FilePath E = Letter('E');

        /// <summary>Starts a path at F:\</summary>
        public static readonly FilePath F = Letter('F');

        /// <summary>Starts a path at G:\</summary>
        public static readonly FilePath G = Letter('G');

        /// <summary>Starts a path at H:\</summary>
        public static readonly FilePath H = Letter('H');

        /// <summary>Starts a path at I:\</summary>
        public static readonly FilePath I = Letter('I');

        /// <summary>Starts a path at J:\</summary>
        public static readonly FilePath J = Letter('J');

        /// <summary>Starts a path at K:\</summary>
        public static readonly FilePath K = Letter('K');

        /// <summary>Starts a path at L:\</summary>
        public static readonly FilePath L = Letter('L');

        /// <summary>Starts a path at M:\</summary>
        public static readonly FilePath M = Letter('M');

        /// <summary>Starts a path at N:\</summary>
        public static readonly FilePath N = Letter('N');

        /// <summary>Starts a path at O:\</summary>
        public static readonly FilePath O = Letter('O');

        /// <summary>Starts a path at P:\</summary>
        public static readonly FilePath P = Letter('P');

        /// <summary>Starts a path at Q:\</summary>
        public static readonly FilePath Q = Letter('Q');

        /// <summary>Starts a path at R:\</summary>
        public static readonly FilePath R = Letter('R');

        /// <summary>Starts a path at S:\</summary>
        public static readonly FilePath S = Letter('S');

        /// <summary>Starts a path at T:\</summary>
        public static readonly FilePath T = Letter('T');

        /// <summary>Starts a path at U:\</summary>
        public static readonly FilePath U = Letter('U');

        /// <summary>Starts a path at V:\</summary>
        public static readonly FilePath V = Letter('V');

        /// <summary>Starts a path at W:\</summary>
        public static readonly FilePath W = Letter('W');

        /// <summary>Starts a path at X:\</summary>
        public static readonly FilePath X = Letter('X');

        /// <summary>Starts a path at Y:\</summary>
        public static readonly FilePath Y = Letter('Y');

        /// <summary>Starts a path at Z:\</summary>
        public static readonly FilePath Z = Letter('Z');

        /// <summary>
        /// Starts a path at the root of the system drive.
        /// </summary>
        public static readonly FilePath System = new FilePath(Path.GetPathRoot(GetFolderPath(SpecialFolder.System)));
    }

    /// <summary>
    /// Quick-access properties for common system and user folders.
    /// </summary>
    public static class Folder
    {
        public static FilePath Profile => new FilePath(GetFolderPath(SpecialFolder.UserProfile));
        public static FilePath Desktop => new FilePath(GetFolderPath(SpecialFolder.DesktopDirectory));
        public static FilePath Documents => new FilePath(GetFolderPath(SpecialFolder.MyDocuments));
        public static FilePath AppData => new FilePath(GetFolderPath(SpecialFolder.ApplicationData));
        public static FilePath LocalAppData => new FilePath(GetFolderPath(SpecialFolder.LocalApplicationData));
        public static FilePath Programs => new FilePath(GetFolderPath(SpecialFolder.ProgramFiles));
        public static FilePath System => new FilePath(GetFolderPath(SpecialFolder.System));

        public static FilePath Current
        {
            get => new FilePath(CurrentDirectory);
            set => CurrentDirectory = value;
        }
    }

    /// <summary>
    /// Starts a UNC network path. Example: \\hostname\sharename
    /// </summary>
    public static class UNC
    {
        public static FilePath Path(string host, string share) => Host(host).Share(share);

        public static UNCHost Host(string host) => new UNCHost(host);
    }

    /// <summary>
    /// Represents a host machine on the network that UNC paths can be built from.
    /// </summary>
    public sealed class UNCHost : NewType<string>
    {
        internal UNCHost(string host) : base(host) {}

        public FilePath Share(string share) =>
            new FilePath(Path.Combine(Value.StartsWith(@"\\") ? Value : @"\\" + Value, share));

        public static FilePath operator /(UNCHost host, FilePath end) => host.Share(end.Value);

        public static FilePath operator /(UNCHost host, string end) => host.Share(end);
    }

    public sealed class FilePath : NewType<string>
    {
        internal FilePath(string path) : base(path) {}

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(FilePath begin, FilePath end) =>
            new FilePath(Path.Combine(begin.Value, end.Value));

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(FilePath begin, string end) =>
            new FilePath(Path.Combine(begin.Value, end));

        /// <summary>
        /// Concatenates two file paths.
        /// </summary>
        public static FilePath operator /(string begin, FilePath end) =>
            new FilePath(Path.Combine(begin, end.Value));

        public static implicit operator string(FilePath filePath) => filePath.Value;
    }
}