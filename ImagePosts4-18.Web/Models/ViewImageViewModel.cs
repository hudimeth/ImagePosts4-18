using ImagePosts.Data;

namespace ImagePosts4_18.Web.Models
{
    public class ViewImageViewModel
    {
        public bool HasPermissionToView { get; set; }
        public Image Image { get; set; }
        public string Message { get; set; }
    }
}
