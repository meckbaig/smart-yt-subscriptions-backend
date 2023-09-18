using Newtonsoft.Json;
using System.Reflection;

namespace DataBaseApi.Models
{
    public class BasicClass
    {
        public object this[string propertyName]
        {
            get
            {
                System.Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                System.Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
        public string ToJsonString(string[] exclude = null)
        {
            
            if (this == null) 
                return "";
            Dictionary<string, string> properties = new Dictionary<string, string>();  
            foreach (var prop in this.GetType().GetProperties())
            {
                try
                {
                    if (prop.Name != "Item" 
                        && exclude?.FirstOrDefault(s => s.ToLower().Equals(prop.Name.ToLower())) == null 
                        && !(prop.PropertyType.BaseType.Name == nameof(BasicClass)))
                    {
                        if (this[prop.Name] != null)
                        {
                            if (this[prop.Name] is DateTime)
                            {
                                //var settings = new JsonSerializerSettings {
                                //    DateTimeZoneHandling = DateTimeZoneHandling.Local };
                                properties.Add(prop.Name[0].ToString().ToLower() + prop.Name.Remove(0, 1),
                                    JsonConvert.SerializeObject(this[prop.Name]/*, settings*/).Replace("\"", ""));
                            }
                            else
                                properties.Add(prop.Name[0].ToString().ToLower() + prop.Name.Remove(0, 1),
                                    this[prop.Name].ToString());
                        }
                            
                        else
                            properties.Add(prop.Name[0].ToString().ToLower() + prop.Name.Remove(0, 1), "");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            string result = JsonConvert.SerializeObject(properties);
            result = result.Replace("\"[", "[").Replace("]\"", "]").Replace("\\\"", "\"").Replace("\\\"", "\"");
            return result;
        }
    }
}
