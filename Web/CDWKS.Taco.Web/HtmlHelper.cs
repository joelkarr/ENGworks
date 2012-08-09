using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public class EmailAttribute :RegularExpressionAttribute
    {
        private const string defaultErrorMessage = "'{0}' must be a valid email address";

        public EmailAttribute() :
            base("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$")
        { }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(defaultErrorMessage, name);
        }

        protected override ValidationResult IsValid(object value,
                                                ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!base.IsValid(value))
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }
    public static class Helpers
    {
        public static MvcHtmlString ActionTableRow(this HtmlHelper html, string action, object routeValues,
                                                string[] tableCellValues, string cellClass)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);
            var rowBuilder = new StringBuilder();
            foreach(var cellValue in tableCellValues)
            {
                // build the <td> tag
                var cell = new TagBuilder("td");
                cell.AddCssClass(cellClass);
                // build the <a> tag
                var anchorBuilder = new TagBuilder("a");
                anchorBuilder.MergeAttribute("style", "display:block; width:100%;");
                anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
                anchorBuilder.InnerHtml = cellValue;
                cell.InnerHtml = anchorBuilder.ToString(TagRenderMode.Normal);;
                rowBuilder.Append(cell.ToString(TagRenderMode.Normal));
            }
            // build the <tr> tag
            var row = new TagBuilder("tr") {InnerHtml = rowBuilder.ToString()};
            return MvcHtmlString.Create(row.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string queryString, string imagePath, string alt, string imgClass)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.AddCssClass(imgClass);
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            var imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues) + queryString);
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            var anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }
}