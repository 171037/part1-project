using Microsoft.AspNetCore.Mvc;
using recipe_in_home.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace recipe_in_home.Controllers
{
    public class PostController : Controller
    {
        private List<Post> list;
        private readonly Csharp_Post_services postService;
        private readonly IWebHostEnvironment _env;

        public PostController(IWebHostEnvironment env)
        {
            _env = env;
            string connString = "Server=" + "" +
                                ";Database=" + "" +
                                ";port=" + "" +
                                ";user=" + "" +
                                ";password=" + "";
            postService = new Csharp_Post_services(connString);
        }

        public IActionResult Index()
        {
            list = postService.Getpost();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var post = postService.SelectPost(id);
            return View(post);
        }
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult Createpost(IFormCollection form)
        {
            var member_name = form["member_name"].ToString();
            var title = form["Title"].ToString();
            var content = form["Content"].ToString();
            var imageDataFile = form.Files["ImageData"];

            byte[] imageData = ReadImageAsByteArray(imageDataFile);

            var imagePath = Path.Combine(_env.WebRootPath, "images");

            var fileExtension = Path.GetExtension(imageDataFile.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + fileExtension;
            var filePath = Path.Combine(imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageDataFile.CopyTo(stream);
            }

            int result = postService.InsertPost(member_name, title, content, fileName, imageData);

            TempData["result"] = result;
            return View();
        }
        public IActionResult Edit(int id)
        {
            var post = postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        public IActionResult EditPost(int member_id, IFormCollection form)
        {
            //var Postid = form["Postid"].ToString();
            var member_name = form["member_name"].ToString();
            var title = form["Title"].ToString();
            var content = form["Content"].ToString();
            var imageDataFile = form.Files["ImageData"];

            byte[] imageData = ReadImageAsByteArray(imageDataFile);

            var imagePath = Path.Combine(_env.WebRootPath, "images");

            var fileExtension = Path.GetExtension(imageDataFile.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + fileExtension;
            var filePath = Path.Combine(imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageDataFile.CopyTo(stream);
            }

            int result = postService.UpdatePost(member_id, member_name, title, content, fileName);

            TempData["result"] = result;

            if (result == 1)
            {
                Console.WriteLine("수정 성공");
            }
            else
            {
                Console.WriteLine("수정 실패");
            }

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var Post = postService.GetPostById(id);
            if (Post == null)
            {
                return NotFound();
            }
            return View(Post);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var Post = postService.GetPostById(id);
            if (Post == null)
            {
                return NotFound();
            }

            postService.DeletePost(id);

            return RedirectToAction("Index");
        }
        private string SaveImageAndGetPath(IFormFile imageDataFile)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images");
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageDataFile.FileName);
            var imagePath = Path.Combine(uploads, fileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                imageDataFile.CopyTo(fileStream);
            }

            return imagePath;
        }

        private byte[] ReadImageAsByteArray(IFormFile file)
        {
            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    return stream.ToArray();
                }
            }
            return null; // 혹은 다른 처리를 수행하거나 예외를 throw할 수 있습니다.
        }

    }
}





