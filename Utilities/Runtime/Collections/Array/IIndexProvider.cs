using System.Collections.Generic;

namespace Irisu.Collections
{
    public interface IIndexProvider : IEnumerator<int>
    {
        public void Remove(int index);
        public void Add(int index);
    }
}