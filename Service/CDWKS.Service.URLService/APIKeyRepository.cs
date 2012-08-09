using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;

namespace CDWKS.Service.URLService
{
    public static class APIKeyRepository
    {
        private const string APIKEYLIST = "APIKeyList";

        private static List<Guid> APIKeys
        {
            get
            {
                // Get from the cache
                // Could also use AppFabric cache for scalability
                var keys = HttpContext.Current.Cache[APIKEYLIST] as List<Guid>;

                if (keys == null)
                    keys = PopulateAPIKeys();

                return keys;
            }
        }

        public static bool IsValidAPIKey(string key)
        {
            // TODO: Implement IsValidAPI Key using your repository

            Guid apiKey;

            // Convert the string into a Guid and validate it
            if (Guid.TryParse(key, out apiKey) && new Guid("bda11d91-7ade-4da1-855d-24adfe39d174") == apiKey)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<Guid> PopulateAPIKeys()
        {
            List<Guid> keyList;

            var dcs = new DataContractSerializer(typeof (List<Guid>));
            HttpServerUtility server = HttpContext.Current.Server;
            using (var fs = new FileStream(server.MapPath("~/App_Data/APIKeys.xml"), FileMode.Open))
            using (
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
            {
                keyList = (List<Guid>) dcs.ReadObject(reader);
            }

            // Save it in the cache
            // Could be saved in AppFabric Cache for scalability across a farm
            HttpContext.Current.Cache[APIKEYLIST] = keyList;

            return keyList;
        }
    }
}