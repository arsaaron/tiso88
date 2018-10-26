using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportNews.Models
{
    public class ContentModel
    {
        public int Stt { get; set; }
        public long news_id { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Description")]
        public string descp { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Content")]
        public string content { get; set; }

        [Display(Name = "Source")]
        public string source { get; set; }

        [Display(Name = "Date")]
        public string created_date { get; set; }

        [Display(Name = "Image")]
        public string imageLnk { get; set; }

        [Display(Name = "Category")]
        public long category_id { get; set; }
        public List<Category> catList { get; set; }
        public string cat_name { get; set; }

        [Display(Name = "Hot")]
        public int hot { get; set; }

        [Display(Name = "Link gốc")]
        public string origin_url { get; set; }
        public DateTime prior { get; set; }
    }

    public class Category
    {
        public long category_id { get; set; }
        public string category_name { get; set; }
        public string created_date { get; set; }

    }

    public class newslist
    {
        public List<ContentModel> ctLst;
    }

    public class SelectListItemHelper
    {
        public static IEnumerable<SelectListItem> GetHotList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Thường", Value = "0"},
                new SelectListItem{Text = "Hot", Value = "1"}

            };
            return items;
        }
    }
}