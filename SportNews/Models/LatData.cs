using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class LatData
    {
        public List<memDt> data { get;set;}
    }

    public class memDt
    {
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string siteName { get; set; }
        public string link { get; set; }
        public string video { get; set; }
    }

    public class ctinfo
    {
        public List<memContent> content { get; set; }
        public memDt meta { get; set; }

    }

    public class detailData
    {
        public ctinfo data { get; set; }
    }

    public class memContent
    {
        public string text { get; set; }
        public string type { get; set; }
        public bool head { get; set; }
        public string link { get; set; }
    }

    public class memContentPr
    {
        public string text { get; set; }
        public string type { get; set; }
        public bool head { get; set; }
        public string link { get; set; }
        public List<string> linkVd { get; set; }
    }

    public class detailDataPr
    {
        public ctinfoPr data { get; set; }
        public List<memDt> relateDt { get; set; }
    }

    public class ctinfoPr
    {
        public List<memContentPr> content { get; set; }
        public memDt meta { get; set; }

    }

}