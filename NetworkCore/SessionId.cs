using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    internal static class IdGenerator<T>
    {
        private static uint s_nextId = 1;

        public static uint Generate()
        {
            return Interlocked.Increment(ref s_nextId);
        }
    }

    public struct UniqueId<T>
    {
        public uint Id { get; private set; } = IdGenerator<T>.Generate();

        public UniqueId() { }

        public static implicit operator uint(UniqueId<T> uniqueId)
        {
            return uniqueId.Id;
        }

        public static implicit operator UniqueId<T>(uint id)
        {
            return new UniqueId<T> { Id = id };
        }
    }
}
