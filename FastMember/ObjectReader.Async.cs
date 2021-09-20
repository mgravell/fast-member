using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FastMember
{
    /// <summary>
    /// Provides a means of reading a sequence of objects as a data-reader, for example
    /// for use with SqlBulkCopy or other data-base oriented code
    /// </summary>
    public abstract partial class ObjectReader : DbDataReader
    {
        static readonly Task<bool> s_True = Task.FromResult(true), s_False = Task.FromResult(false);
        private sealed class AsyncObjectReader<T> : ObjectReader
        {
            private IAsyncEnumerator<T> source;
            internal AsyncObjectReader(Type type, IAsyncEnumerable<T> source, string[] members, CancellationToken cancellationToken) : base(type, members)
            {
                if (source == null) throw new ArgumentOutOfRangeException(nameof(source));

                cancellationToken.ThrowIfCancellationRequested();
                this.source = source.GetAsyncEnumerator(cancellationToken);
                
            }

            protected override void Shutdown()
            {
                base.Shutdown();
                var tmp = source as IDisposable;
                source = null;
                if (tmp != null) tmp.Dispose();
            }

            public override bool IsClosed => source == null;

            public override Task<bool> ReadAsync(CancellationToken cancellationToken)
            {
                static async Task<bool> FromAsync(AsyncObjectReader<T> source, ValueTask<bool> pending)
                {
                    if (await pending.ConfigureAwait(false))
                    {
                        source.current = source.source.Current;
                        return true;
                    }
                    source.active = false;
                    source.current = null;
                    return false;
                }
                if (active)
                {
                    var tmp = source;
                    if (tmp != null)
                    {
                        var pending = tmp.MoveNextAsync();
                        if (!pending.IsCompletedSuccessfully)
                        {
                            return FromAsync(this, pending);
                        }
                        if (pending.Result)
                        {
                            current = tmp.Current;
                            return s_True;
                        }
                        else
                        {
                            active = false;
                        }
                    }
                    else
                    {
                        active = false;
                    }
                }
                current = null;
                return s_False;
            }
        }
        public override bool Read()
            => ReadAsync().GetAwaiter().GetResult(); // sync-over-async, self-inflicted

        /// <summary>
        /// Creates a new ObjectReader instance for reading the supplied data
        /// </summary>
        /// <param name="source">The sequence of objects to represent</param>
        /// <param name="members">The members that should be exposed to the reader</param>
        public static ObjectReader Create<T>(IAsyncEnumerable<T> source, params string[] members)
            => new AsyncObjectReader<T>(typeof(T), source, members, CancellationToken.None);

        /// <summary>
        /// Creates a new ObjectReader instance for reading the supplied data
        /// </summary>
        /// <param name="source">The sequence of objects to represent</param>
        /// <param name="members">The members that should be exposed to the reader</param>
        public static ObjectReader Create<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken, params string[] members)
            => new AsyncObjectReader<T>(typeof(T), source, members, cancellationToken);
    }
}