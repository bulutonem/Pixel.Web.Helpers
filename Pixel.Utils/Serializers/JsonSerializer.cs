using Newtonsoft.Json;

namespace Pixel.Utils.Serializers
{
    public class JsonSerializer
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}