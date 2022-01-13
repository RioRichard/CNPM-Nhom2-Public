using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace TeyvatCafe
{
    public class PaginationHelper : TagHelper
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public string UrlPage { get; set; }
        public int MaItem { get; set; }


        void Pagination(string URLPage,int CurrentPage,int TotalPage, StringBuilder sb, int MaItem)
        {
            
            sb.Append("<ul class=\"pagination\">");
            sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{URLPage}/{MaItem}/1\">First</a></li>");
            int firstI = CurrentPage;
            int lastI = CurrentPage+2;
            if (CurrentPage > 1)
            {
                firstI = CurrentPage - 1;
                lastI = CurrentPage + 1;

            }
            if (CurrentPage+2 >= TotalPage)
            {
                if (TotalPage <= 3)
                    firstI = 1;
                else
                    firstI = TotalPage-3;
                lastI = TotalPage;
            }
            

            for (int i = firstI; i <= lastI; i++)
            {
                if(i==CurrentPage)
                    sb.Append($"<li class=\"page-item active\"><a class=\"page-link\" href=\"{URLPage}/{MaItem}/{i}\">{i}</a></li>");
                else
                    sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{URLPage}/{MaItem}/{i}\">{i}</a></li>");

            }
            sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{URLPage}/{MaItem}/{TotalPage}\">Last</a></li>");
            sb.Append("</ul>");
            sb.Append("</nav>");

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";

            StringBuilder sb = new StringBuilder();
            Pagination(UrlPage, CurrentPage, TotalPage, sb, MaItem);
            output.Content.SetHtmlContent(sb.ToString());
        }

    }

}