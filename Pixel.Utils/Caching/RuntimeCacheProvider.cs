﻿using System;
using System.Web;

namespace Pixel.Utils.Caching
{
    class RuntimeCacheProvider : ICacheProvider
    {
        private static readonly System.Web.Caching.Cache Cache;
        static RuntimeCacheProvider()
        {
            Cache = HttpRuntime.Cache;
        }

        public RuntimeCacheProvider()
        {
            if (Cache == null)
                throw new NullReferenceException("Cache item can not be null.");
        }
        public object Add(string key, object value, int expireMinutes)
        {
            return Cache.Add(key, value, null, DateTime.Now.AddMinutes(expireMinutes), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, RemovedCallback);
        }

        public T Add<T>(string key, T value, int expireMinutes)
        {
            return (T)Cache.Add(key, value, null, DateTime.Now.AddMinutes(expireMinutes), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, RemovedCallback);
        }

        public object Get(string key)
        {
            return Cache[key];
        }

        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        private void RemovedCallback(string key, object value, System.Web.Caching.CacheItemRemovedReason removedReason)
        {
            Console.WriteLine($"<CACHE REMOVED> : {key} REASON: {removedReason.ToString()}");
        }
    }
}