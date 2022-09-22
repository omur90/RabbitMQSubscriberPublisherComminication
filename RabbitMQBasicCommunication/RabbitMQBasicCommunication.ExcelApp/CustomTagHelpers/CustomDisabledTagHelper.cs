using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NuGet.Packaging;
using System.Text.Encodings.Web;

namespace RabbitMQBasicCommunication.ExcelApp.CustomTagHelpers
{
    [HtmlTargetElement("a")]
    public class CustomDisabledTagHelper : TagHelper
    {
       
        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsDisabled)
            {
                output.AddClass("disabled", HtmlEncoder.Default);

            }
            base.Process(context, output);
        }
    }
}
