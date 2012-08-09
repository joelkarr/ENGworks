using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CDWKS.BXC.Taco.Web.Models
{
    [Serializable]
    [XmlRoot("Pumps")]
    public class PumpResults
    {
        [XmlArray("results")]
        [XmlArrayItem("pump", typeof(Pump))]
        public Pump[] results { get; set; }
        
        public PumpResults()
        {
            results = new Pump[0];
        }
    }

    [Serializable]
    public class Pump
    {
        #region API Result XML

        //        <pump>

        //    <line_key></line_key>
        //    <curve_id></curve_id>
        //    <model_no></model_no>
        //    <rpm></rpm>
        //    <imp_dia></imp_dia>
        //    <hp></hp>
        //    <flow></flow>
        //    <head></head>
        //    <eff></eff>
        //    <nol_hp></nol_hp>
        //    <npsh></npsh>
        //    <suct_disch></suct_disch>
        //</pump>

        #endregion

        [XmlElement("PreviewImage")]
        public string PreviewImage { get; set; }

        [XmlElement("IsENGworksAvailable")]
        public bool isENGworksAvailable { get; set; }
        [XmlElement("line_key")]
        public string line_key { get; set; }

        [XmlElement("curve_id")]
        public string curve_id { get; set; }

        [XmlElement("model_no")]
        [DisplayName("Model")]
        public string model_no { get; set; }

        [XmlElement("rpm")]
        [DisplayName("Speed")]
        public string rpm { get; set; }

        [XmlElement("imp_dia")]
        [DisplayName("Diameter")]
        public string imp_dia { get; set; }

        [XmlElement("hp")]
        public string hp { get; set; }

        [XmlElement("flow")]
        public string flow { get; set; }

        [XmlElement("head")]
        public string head { get; set; }

        [XmlElement("eff")]
        public string eff { get; set; }

        [XmlElement("nol_hp")]
        public string nol_hp { get; set; }

        [XmlElement("npsh")]
        public string npsh { get; set; }

        [XmlElement("suct_disch")]
        [DisplayName("Impeller")]
        public string suct_disch { get; set; }

        public string QueryString { get; set; }

        public string DownloadGuid { get; set; }
    }


}