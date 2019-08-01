using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BulkUploadPOC
{
    public class ReflectionCache
    {
        private readonly Dictionary<string, List<PropertyInfo>> _reflectiondictionary;

        public ReflectionCache()
        {
            _reflectiondictionary = new Dictionary<string, List<PropertyInfo>>
            {
                { typeof(BulkTest).Name, typeof(BulkTest).GetProperties().ToList() }
            };
        }

        public List<PropertyInfo> GetObjectProperties(string typename)
        {
            if (_reflectiondictionary.ContainsKey(typename))
                return _reflectiondictionary[typename];
            return new List<PropertyInfo>();
        }
    }
}
