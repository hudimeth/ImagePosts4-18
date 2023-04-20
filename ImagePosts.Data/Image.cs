using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePosts.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
    }
}
