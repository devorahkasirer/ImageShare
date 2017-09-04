using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageShare.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FileName { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateUploaded { get; set; }
        public int Likes { get; set; }
    }
}
