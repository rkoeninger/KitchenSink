using System;
using System.Collections.Generic;
using System.IO;
using static KitchenSink.Operators;

namespace KitchenSink.FileSystem
{
    public class ResilientFileSystem : IFileSystem
    {
        private readonly IFileSystem fs;
        private readonly int retryCount;
        private readonly TimeSpan retryDelay;

        public ResilientFileSystem(IFileSystem fs, int retryCount, TimeSpan retryDelay)
        {
            this.fs = fs;
            this.retryCount = retryCount;
            this.retryDelay = retryDelay;
        }

        private A DoRetry<A>(Func<A> action) =>
            Retry.Exponential(retryCount, retryDelay, action, Is<IOException>).OrElseThrow();

        private void DoRetry(Action action) =>
            Retry.Exponential(retryCount, retryDelay, action, Is<IOException>).ForEach(x => throw x);

        public void Create(EntryType type, string path) => DoRetry(() => fs.Create(type, path));
        public void Delete(string path) => DoRetry(() => fs.Delete(path));
        public void Move(string source, string destination) => DoRetry(() => fs.Move(source, destination));
        public EntryInfo GetInfo(string path) => DoRetry(() => fs.GetInfo(path));
        public IEnumerable<EntryInfo> ReadDirectory(string path) => DoRetry(() => fs.ReadDirectory(path));
        public Stream ReadFile(string path) => DoRetry(() => fs.ReadFile(path));
        public Stream WriteFile(string path, bool append = false) => DoRetry(() => fs.WriteFile(path, append));
    }
}
