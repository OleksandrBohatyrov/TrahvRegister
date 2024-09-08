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

        // Отображение всех штрафов (например, для админа)
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string searchCarNumber = null)
        {
            var penalties = db.Penalty.AsQueryable();

            if (!string.IsNullOrEmpty(searchCarNumber))
            {
                penalties = penalties.Where(p => p.CarNumber.Contains(searchCarNumber));
            }

            return View(penalties.ToList());
        }

        // Отображение штрафов для текущего пользователя
        [Authorize]
        public ActionResult Fines(string searchCarNumber = null)
        {
            var currentUserEmail = User.Identity.GetUserName();
            var penalties = db.Penalty.Where(p => p.UserEmail == currentUserEmail);

            if (!string.IsNullOrEmpty(searchCarNumber))
            {
                penalties = penalties.Where(p => p.CarNumber.Contains(searchCarNumber));
            }

            return View(penalties.ToList());
        }

        // Создание штрафа (только для администратора)
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

                var user = db.Users.FirstOrDefault(u => u.Email == penalty.UserEmail);
                if (user != null)
                {
                    E_mail(penalty);
                }

                db.Penalty.Add(penalty);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(penalty);
        }

        // Метод для редактирования штрафа (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var fine = db.Penalty.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Fine fine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fine);
        }

        // Метод для удаления штрафа (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var fine = db.Penalty.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var fine = db.Penalty.Find(id);
            db.Penalty.Remove(fine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Отправка письма пользователю с информацией о штрафе
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

                    WebMail.Send(user.Email,
                                 "Teil on uus Trahv!",
                                 $"Tere {penalty.Name},\n\nAuto number: {penalty.CarNumber}\nTrahvi summa: {penalty.Sum} Є\nKuupäev: {penalty.Date:yyyy.MM.dd}");
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
