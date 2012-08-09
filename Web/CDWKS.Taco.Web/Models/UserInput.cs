using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CDWKS.BXC.Taco.Web.Models
{

    public class UserInput
    {
        [Required]
        [DisplayName("Flow Rate (gpm)*")]
        public decimal FlowRate { get; set; }
        [Required]
        [DisplayName("Total Head (ft)*")]
        public decimal TotalHead { get; set; }
        [DisplayName("Min Eff")]
        public decimal MinEff { get; set; }
        [Required]
        [DisplayName("Pump Types")]
        public List<PumpTypes> PumpTypes { get; set; }
        [Required]
        [DisplayName("Pump Speeds")]
        public PumpSpeeds PumpSpeeds {get;set;}
        [Required]
        [DisplayName("Engineering Units")]
        public EngineeringUnits EngineeringUnits { get; set; }
        [Required]
        [DisplayName("Motor")]
        public SyncSpeedOptions Motor { get; set; }

        public string ToQueryString { get
        {
            return "?FlowRate=" + FlowRate + "&TotalHead=" + TotalHead + "&MinEff=" + MinEff
                + "&PumpTypes=" + (PumpTypes != null ? String.Join(",", PumpTypes.ToArray()) : String.Empty) + "&PumpSpeeds=" + PumpSpeeds
                   + "&EngineeringUnits=" + EngineeringUnits + "&Motor=" + Motor;
        } }
        
    }

    
    #region Enums

    public enum EngineeringUnits
    {
        [Description("English")]
        English,
        [Description("Metric")]
        Metric
    }

    public enum SyncSpeedOptions
    {
        [Description("50hz")]
        _50hz,
        [Description("60hz")]
        _60hz
    }
    public enum PumpTypes
    {
        [Description("1600 Series")]
        Series1600,
        [Description("1900 Series")]
        Series1900,
        [Description("FI/CI Series")]
        FICISeries,
        [Description("GFI Intl")]
        GFIIntl,
        [Description("GSVS/GSVC Intl")]
        GSVSGSVCIntl,
        [Description("GTA Intl")]
        GTAIntl,
        [Description("KS Series")]
        KSSeries,
        [Description("KS/KV Series")]
        KSKVSeries,
        [Description("SCX1700 Series")]
        SCX1700Series,
        [Description("TA Series")]
        TASeries,
        [Description("VT Series")]
        VTSeries

    }
    public enum PumpSpeeds
    {
        [Description("ALL")]
        All,
        [Description("1160")]
        s1160,
        [Description("1760")]
        s1760,
        [Description("3500")]
        s3500

    }

    #endregion

 
}

public static class HtmlExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;

        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(
        this HtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression
    )
    {
        var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        var t = metaData.ModelType;
        var names = Enum.GetNames(metaData.ModelType);
        var sb = new StringBuilder();
        var first = true;
        foreach (var name in names)
        {
            var id = string.Format(
                "{0}_{1}_{2}_Checked=true",
                htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                metaData.PropertyName,
                name
            );
            var radio = htmlHelper.RadioButtonFor(expression, name, new { @id = id }).ToHtmlString();
            if (first)
            {
                first = false;
                radio = htmlHelper.RadioButtonFor(expression, name, new {@id = id, @checked = true}).ToHtmlString();
            }

            sb.AppendFormat(
                "<span>{0}</span> {1}",
                HttpUtility.HtmlEncode(GetDescription((Enum)Enum.Parse(t,name))),
                radio
            );
           
        }
        return MvcHtmlString.Create(sb.ToString());
    }
}