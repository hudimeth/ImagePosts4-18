using ImagePosts.Data;
using ImagePosts4_18.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace ImagePosts4_18.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; initial catalog=imagePosts; integrated security=true;";
        private IWebHostEnvironment _webHostEnvironment;
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            var imageName = $"{Guid.NewGuid()}-{image.FileName}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageName);
            using var fs = new FileStream(imagePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var db = new ImageDb(_connectionString);
            int imageId = db.AddImage(imageName, password);
            var vm = new ImageUploadViewModel
            {
                ImageId = imageId,
                ImagePassword = password
            };
            return View(vm);
        }
        public IActionResult ViewImage(int id)
        {
            var vm = new ViewImageViewModel();
            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }
            if (!HasPermissionToView(id))
            {
                vm.HasPermissionToView = false;
                vm.Image = new Image { Id = id };
            }
            else
            {
                vm.HasPermissionToView = true;
                var db = new ImageDb(_connectionString);
                vm.Image = db.GetImageById(id);
                db.UpdateViews(vm.Image);
            }
            return View(vm);
        }
        private bool HasPermissionToView(int id)
        {
            var allowedImagesIds = HttpContext.Session.Get<List<int>>("allowedimagesids");
            if(allowedImagesIds == null)
            {
                return false;
            }
            return allowedImagesIds.Contains(id);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var db = new ImageDb(_connectionString);
            var image = db.GetImageById(id);
            if(password != image.Password)
            {
                TempData["message"] = "Invalid Password";
            }
            else
            {
                var allowedImagesIds = HttpContext.Session.Get<List<int>>("allowedimagesids");
                if(allowedImagesIds == null)
                {
                    allowedImagesIds = new List<int>();
                }
                allowedImagesIds.Add(id);
                HttpContext.Session.Set("allowedimagesids", allowedImagesIds);
            }
            return Redirect($"/home/viewimage?id={id}");
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}