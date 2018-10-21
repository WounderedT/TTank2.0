using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter
{ 
    class MyList<T> where T : struct
    {
        private T[] entities;
        private int[] next;
        private int sizeLimit;
        private int nextFree;
        private int holesCount;

        private T defaultValue;

        public int Size { get; private set; }
        public int Capasity { get { return sizeLimit; } }

        public int FilledSize { get { return Size + holesCount; } }
        public T[] Data { get { return entities; } }

        public MyList(int sizeLimit, T defaultValue = default(T))
        {
            this.defaultValue = defaultValue;
            this.sizeLimit = sizeLimit;

            entities = new T[sizeLimit];
            next = new int[sizeLimit];
            Clear();
        }

        public void Clear()
        {
            nextFree = 0;
            holesCount = 0;
            Size = 0;

            for(int i=0; i < sizeLimit; i++)
            {
                entities[i] = defaultValue;
                next[i] = i + 1;
            }
        }

        public int Allocate()
        {
            var free = nextFree;

            if(free == sizeLimit)
            {
                var newSize = (int)(Capasity * (Capasity < 1024 ? 2 : 1.5f));
                Array.Resize(ref entities, newSize);
                Array.Resize(ref next, newSize);

                for (int i = sizeLimit; i < newSize; i++)
                    next[i] = i + 1;

                sizeLimit = newSize;
            }

            nextFree = next[free];
            next[free] = -1;
            if (free < FilledSize - 1)
                holesCount--;
            Size++;
            return free;
        }

        public void Free(int index)
        {
            if (Size == 0)
                return;
            next[index] = nextFree;
            nextFree = index;
            entities[index] = defaultValue;
            if (Size == 1)
                holesCount = 0;
            else if (index < FilledSize - 1)
                holesCount++;
            Size--;
        }
    }
}
