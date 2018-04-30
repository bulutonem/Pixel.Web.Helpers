namespace Pixel.Utils.Caching
{
    public interface ICacheProvider
    {
        object Add(string key, object value, int expireMinutes);
        T Add<T>(string key, T value, int expireMinutes);
        object Get(string key);
        T Get<T>(string key);
        void Remove(string key);
    }
}