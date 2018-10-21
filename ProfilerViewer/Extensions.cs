using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerViewer
{
    //This is a great waste of resources. ObservableRangedCollection should be implemented (implementation from
    //https://gist.github.com/weitzhandler/65ac9113e31d12e697cb58cd92601091 throws NotSupported exception on AddRange method).
    public static class ObservableCollectionExtensions
    {
        public static void Update<T>(this ObservableCollection<T> collection, IList<T> enumerable)
        {
            collection.Clear();
            foreach (var entry in enumerable)
                collection.Add(entry);
        }

        public static void RemoveRange<T>(this ObservableCollection<T> collection, Int32 position, Int32 count)
        {
            while(count-- > 0)
            {
                collection.RemoveAt(position);
            }
        }

        public static void TryAdd<T>(this ICollection<T> collection, T element)
        {
            if (!collection.Contains(element))
                collection.Add(element);
        }
    }

    public static class DictionaryExtensions
    {
        public static void AddOrUpdateOnNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                if (dictionary[key] == null)
                    dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
                
        }
    }
}
