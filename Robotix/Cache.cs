using System.Collections.Generic;

namespace Robotix
{
    /// <summary>
    /// Caching data for later use
    /// </summary>
    /// <typeparam name="T">Type of object to cache</typeparam>
    public class Cache<T>
    {
        List<T> cacheList;
        private readonly object _lock = new object();

        /// <summary>
        /// Caching data for later use
        /// </summary>
        public Cache()
        {
            cacheList = new List<T>();
        }

        /// <summary>
        /// Add a object to the non-current list
        /// </summary>
        /// <param name="objectToAdd">The object to add</param>
        public void Add(T objectToAdd)
        {
            // Add a object to the not current list
            lock (_lock)
            {
                cacheList.Add(objectToAdd);
            }
        }
        /// <summary>
        /// Will return the current list and then remove the content from the cache
        /// </summary>
        /// <returns></returns>
        public List<T> GetCachedData()
        {
            lock (_lock)
            {
                List<T> temp = cacheList;

                cacheList = null;
                cacheList = new List<T>();
                // cacheList.Capacity = 0; // Clear the refrence to underlying array

                return temp;
            }
        }
        /// <summary>
        /// Will force a clearing of the cache. Will remove data from current cache
        /// </summary>
        public void ForceClear()
        {
            cacheList.Clear();
        }
    }
}
