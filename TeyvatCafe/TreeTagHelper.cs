using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text;
using TeyvatCafe.Models;

namespace TeyvatCafe
{
    public class TreeTagHelper : TagHelper
    {
        public List<Category> list { get; set; }

        static void Parse(List<Category> categories, StringBuilder sb)
        {

            sb.Append($"<div class=\"collapse\" id=\"collapseExample{categories[0].FatherCategory}\" style=\"positon:relative;\" >");
            sb.Append("<ul class=\"list-group \" > ");
            ParseTree(categories, sb);
            sb.Append("</ul> ");
            sb.Append("</div>");
        }

        static void ParseTree(List<Category> categories, StringBuilder sb)
        {

            foreach (var item in categories)
            {
                sb.Append("<li class=\"list-group-item\" style=\"positon:absolute;\">");


                sb.Append($"<a href =\"/category/{item.IDCategory}\" >{item.CategoryName}</a>");

                if (item.ChildCategory != null)
                {
                    sb.Append($"<a class=\"btn btn-primary\" data-toggle=\"collapse\" href=\"#collapseExample{item.IDCategory}\"  style=\"float:right;\">");
                    sb.Append("v");
                    sb.Append($"</a>");
                    Parse(item.ChildCategory, sb);
                }


                sb.Append("</li>");
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.SetAttribute("class", "list-group");
            output.Attributes.SetAttribute("style", "positon:relative;");

            StringBuilder sb = new StringBuilder();
            ParseTree(list, sb);
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
