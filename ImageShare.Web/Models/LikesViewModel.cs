using ImageShare.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageShare.Web.Models
{
    public class LikesViewModel
    {
        public IEnumerable<Image> Images { get; set; }
        public User User { get; set; }
    }
}