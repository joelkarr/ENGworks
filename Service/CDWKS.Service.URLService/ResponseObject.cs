using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CDWKS.Service.URLService
{
[Serializable]
    public class ResponseObject
    {
        [XmlElement("model_no")]
        public string ModelNo { get; set; }
        [XmlElement("RevitFamilyName")]
        public string RevitFamilyName { get; set; }
        [XmlElement("FoundInENGworksDB")]
        public bool FoundInENGworksDB { get; set; }
        [XmlElement("URL")]
        public string URL { get; set; }
        [XmlElement("Message")]
        public string Message { get; set; }
    }

    public class ResponseCollectionObject
    {
        public List<ResponseObject> Objects { get; set; }
    }
}