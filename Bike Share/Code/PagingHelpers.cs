using BikeShare.ViewModels;
using System;
using System.Web.Mvc;

namespace BikeShare.Views
{
    /// <summary>
    /// Creates pagination controls
    /// </summary>
    public static class PagingHelpers
    {
        /// <summary>
        /// Generates the html for pagination links.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pagingInfo">Information to support pagination.</param>
        /// <param name="pageUrl">Base url for constructing page links.</param>
        /// <returns>MvcHtmlString to insert into the page markup.</returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PageInfo pagingInfo, Func<int, string> pageUrl)
        {
            if (pagingInfo.TotalItems <= pagingInfo.ItemsPerPage)
            {
                return new MvcHtmlString("");
            }
            TagBuilder outerList = new TagBuilder("ul");
            outerList.AddCssClass("pagination");
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("li");
                TagBuilder link = new TagBuilder("a");
                link.MergeAttribute("href", pageUrl(i));
                link.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("active");
                }
                tag.InnerHtml = link.ToString();
                outerList.InnerHtml = outerList.InnerHtml + "\n" + tag.ToString();
            }

            return MvcHtmlString.Create(outerList.ToString());
        }
    }
}