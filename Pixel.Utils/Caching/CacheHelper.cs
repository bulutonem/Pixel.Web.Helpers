using System.Web;

namespace Pixel.Utils.Caching
{
    public class CacheHelper
    {
        private static readonly System.Web.Caching.Cache Cache;

        static CacheHelper()
        {
            Cache = HttpRuntime.Cache;
        }

        public CacheHelper(ICacheProvider source)
        {
            
        }

    }
}