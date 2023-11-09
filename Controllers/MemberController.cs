using Microsoft.AspNetCore.Mvc;
using recipe_in_home.Models;

namespace recipe_in_home.Controllers
{
	public class MemberController : Controller
	{
		List<members> list;
		member_management mem;
		public MemberController()
		{
			String connString = "Server = " + "" +
								";Database = " + "" +
								";port = " + "" +
								";User = " + "" +
								";password = " + "";
			mem = new member_management(connString);
		}
		public IActionResult Index()
		{
			list = mem.Getmember();
			return View(list);
		}
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Createpost(IFormCollection form)
        {
            var member_name = form["member_name"].ToString();
            var member_birth = form["member_birth"].ToString();
            var member_id = form["member_id"].ToString();
            var member_pw = form["member_pw"].ToString();
            var member_gender = Convert.ToChar(form["member_gender"]);
            var member_job = form["member_job"].ToString();
            int result = mem.Insertmem(member_name, member_birth,member_id,member_pw,member_gender,member_job);
            TempData["result"] = result;
            return View();
        }
        
        public IActionResult Update(string id)
        {
            var std = mem.Selectmem(id);
            return View(std);
        }

        public IActionResult Updatepost(IFormCollection form)
        { 
            var member_id = form["member_id"].ToString();
            var member_pw = form["member_pw"].ToString();
            var member_job = form["member_job"].ToString();
            int result = mem.Updatemem( member_id, member_pw, member_job);
            TempData["result"] = result;
            return View();
        }
        public IActionResult Details(string id)
        {
            var std = mem.Selectmem(id);
            return View(std);
        }

        public IActionResult Delete(string id)
        {
            var std = mem.Selectmem(id);
            return View(std);
        }

        public IActionResult Deletepost(string id) //확인 후 제거
        {
            int result = mem.Deletemem(id);
            TempData["result"] = result;
            return View();
        }
        public IActionResult Login(IFormCollection form)
        {
            var submitid = form["id"].ToString();
            var submitpw = form["pw"].ToString();
            return View();
        }
    }
}
