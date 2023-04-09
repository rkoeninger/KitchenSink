using KitchenSink.Extensions;
using System;
using System.Collections.Generic;

namespace KitchenSink
{
    public class AggregateDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> disposables;

        public AggregateDisposable(IEnumerable<IDisposable> disposables) => this.disposables = disposables;

        public void Dispose() => disposables.ForEach(d => d.Dispose());
    }
}
