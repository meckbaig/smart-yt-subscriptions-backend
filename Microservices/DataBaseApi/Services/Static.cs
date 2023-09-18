using DataBaseApi.Models;
using System.Reflection;
using System.Text.Json;

namespace DataBaseApi.Services
{
    public static class Static
    {
        public static void DeserializeSafely<T>(this string jsonString, ref T result)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(jsonString);
            foreach (var prop in result.GetType().GetProperties())
            {
                try
                {
                    string propName = prop.Name[0].ToString().ToUpper() + prop.Name.Remove(0, 1);
                    if (propName != "Item")
                    {
                        MethodInfo method = typeof(Static).GetMethod(nameof(Static.GetSafely));
                        MethodInfo generic = method.MakeGenericMethod(prop.PropertyType);
                        var res = generic.Invoke(null, new object[] { json, propName });

                        //if (res != default && res.ToString() != "")
                        (result as BasicClass)[propName] = res;
                    }
                }
                catch (Exception)
                { }
            }
        }

        public static T? GetSafely<T>(this JsonElement jElement, string name)
        {
            try
            {
                var property = jElement.GetProperty(name[0].ToString().ToLower() + name.Substring(1));
                T result = default(T);
                try
                {
                    //if (typeof(T) == typeof(Int32))
                    //    result = (T)Convert.ToInt32(property);
                    //else
                    result = JsonSerializer.Deserialize<T>(property);
                }
                catch (Exception)
                {
                    result = (T)Convert.ChangeType(property.ToString(), typeof(T));
                }
                return result;
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T temporal = list[j];
                list[j] = list[i];
                list[i] = temporal;
            }
            return list;
        }

        //public static void GetSafely<T>(this JsonElement jElement, string name, out T result)
        //{
        //    try
        //    {
        //        var property = jElement.GetProperty(name);
        //        result = JsonSerializer.Deserialize<T>(property);
        //    }
        //    catch (Exception)
        //    {
        //        result = default(T);
        //    }
        //}

    }
}
