using System;
using System.Collections.Specialized;

namespace Pixel.Utils
{
    public class ObjectBinder
    {
        public static T BindObject<T>(NameValueCollection form)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            if (form != null)
            {
                foreach (var prp in obj.GetType().GetProperties())
                {
                    if (!string.IsNullOrEmpty(form[prp.Name]))
                        if (prp.PropertyType == typeof(String) || prp.PropertyType == typeof(string))
                        {
                            prp.SetValue(obj, form[prp.Name]);
                        }
                        else if (form[prp.Name] != null)
                        {
                            Type t = Nullable.GetUnderlyingType(prp.PropertyType) ?? prp.PropertyType;
                            var safeValue = form[prp.Name] == null ? null : Convert.ChangeType(form[prp.Name], t);
                            prp.SetValue(obj, safeValue, null);
                        }
                }
            }
            return obj;
        }

        public static T ConvertObject<T>(object obj)
        {
            Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            var safeValue = obj == null ? null : Convert.ChangeType(obj.ToString(), t);
            return (T)safeValue;
        }
    }
}
