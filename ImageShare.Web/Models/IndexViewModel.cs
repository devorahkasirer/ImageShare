using ImageShare.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageShare.Web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Image> Top5Liked { get; set; }
        public IEnumerable<Image> Top5Viewed { get; set; }
        public IEnumerable<Image> Top5Date { get; set; }
        public bool LoggedIn { get; set; }
        public User User { get; set; }
    }
}