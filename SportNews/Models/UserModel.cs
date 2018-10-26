using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class UserModel
    {
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string roles { get; set; }
        public string password { get; set; }
        public string created_date { get; set; }

    }
}