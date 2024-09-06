using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using TrajvRegister10.Models;
using System.Data.Entity;

namespace Penalty.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string searchCarNumber = null)
        {
            var penalties = db.Penalty.AsQueryable();

            // Если был введен номер машины для поиска
            if (!string.IsNullOrEmpty(searchCarNumber))
            {
                penalties = penalties.Where(p => p.CarNumber.Contains(searchCarNumber));
            }

            // Незарегистрированные пользователи видят все штрафы
            return View(penalties.ToList());
        }

        [Authorize]
        public ActionResult Fines(string searchCarNumber = null)
        {
            var penalties = db.Penalty.AsQueryable();
            var currentUserEmail = User.Identity.GetUserName();  // Получаем email текущего пользователя

            if (!string.IsNullOrEmpty(searchCarNumber))
            {
                penalties = penalties.Where(p => p.CarNumber.Contains(searchCarNumber));
            }

            // Пользователь видит только свои штрафы
            penalties = penalties.Where(p => p.UserEmail == currentUserEmail);

            return View(penalties.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Fine penalty)
        {
            if (ModelState.IsValid)
            {
                penalty.CalculateSumma();

                // Проверяем, существует ли пользователь с указанным email
                var user = db.Users.FirstOrDefault(u => u.Email == penalty.UserEmail);
                if (user != null)
                {
                    // Если пользователь найден, отправляем ему email
                    E_mail(penalty);
                }

                db.Penalty.Add(penalty);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(penalty);
        }

        public void E_mail(Fine penalty)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == penalty.UserEmail);
            if (user != null)
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "nepridumalnazvaniepocht@gmail.com";
                    WebMail.Password = "rnlt mfvn ftjb usxu";
                    WebMail.From = "nepridumalnazvaniepocht@gmail.com";
                    WebMail.Send(user.Email, "Teil on uus Trahv!", "Tere " + penalty.Name + " Auto number: " + penalty.CarNumber + " Trahv maksa: "
                        + penalty.Sum + "Є" + " Trahvi kuupäev: " + penalty.Date.ToString("yyyy.MM.dd"));
                    ViewBag.Message = "Kiri on saatnud!";
                }
                catch
                {
                    ViewBag.Message = "Mul on kahju! Ei saa kirja saada!!!";
                }
            }
        }
    }
}
