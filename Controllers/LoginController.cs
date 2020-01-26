using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rezervigo.Models;
using RezervigoData;
using RezervigoData.Models;
using Microsoft.AspNetCore.Routing;
using System.Security.Cryptography;
using System.Text;

namespace Rezervigo.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAdmin _admins;

        public LoginController(ILogger<LoginController> logger, IAdmin admins)
        {
            _logger = logger;
            _admins = admins;
        }

        public IActionResult Index(string error)
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostLogin()
        {
            var username = HttpContext.Request.Form["loginUsername"];
            var password = HttpContext.Request.Form["loginPassword"];
            var admin = _admins.GetByName(username);
            if (admin != null)
            {
                if (admin.Password == getMd5Hash(password))
                {
                    var show_admin = new AdminView
                    {
                        Id = admin.Id,
                        Name = admin.Name,
                        Password = admin.Password,
                    };
                    return Content("Hello, " + show_admin.Name + ". You are connected!");
                }
                else
                {
                    TempData["error"] = "The password for "+admin.Name+" is incorect!";
                    return RedirectToAction("Index", new RouteValueDictionary(
                        new { controller = "Login", action = "Index" }));
                }
            }
            else
            { 
                TempData["error"] = "This username does not exist in our database!";
                return RedirectToAction("Index", new RouteValueDictionary(
                    new { controller = "Login", action = "Index"}));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        static string getMd5Hash(string input)
        { // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create(); // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes // and create a string.
            StringBuilder sBuilder = new StringBuilder(); // Loop through each byte of the hashed data // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }//function to convert string to md5 --->needs to be put inside a helper class
    }
}
