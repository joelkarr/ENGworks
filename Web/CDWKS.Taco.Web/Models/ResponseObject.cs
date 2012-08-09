using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CDWKS.BXC.Taco.Web.Models
{
    [Serializable]
    public class ResponseObject
    {
        [XmlElement("model_no")]
        public string model_no { get; set; }
        [XmlElement("RevitFamilyName")]
        public string RevitFamilyName { get; set; }
        [XmlElement("FoundInENGworksDB")]
        public bool FoundInENGworksDB { get; set; }
        [XmlElement("URL")]
        public string URL { get; set; }
        [XmlElement("Message")]
        public string Message { get; set; }
    }
    [Serializable]
    [XmlRoot("responsecollectionobject")]
    public class ResponseCollectionObject
    {
        [XmlArray("objects")]
        [XmlArrayItem("responseobject", typeof(ResponseObject))]
        public List<ResponseObject> Objects { get; set; }
    }
}