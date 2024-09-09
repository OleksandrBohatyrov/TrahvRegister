using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Drawing;
using TrajvRegister10.Models;
using System.Data.Entity;
using PayPal.Api;
using System.Text;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace Penalty.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        [HttpPost]
        [Authorize]
        public ActionResult PayWithPayPal(int id)
        {
            var fine = db.Penalty.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }

            var payPalService = new PayPalService();

            // Генерация ссылки для успешной оплаты с передачей id
            var returnUrl = Url.Action("PaymentSuccess", "Home", new { id = fine.Id }, Request.Url.Scheme);
            var cancelUrl = Url.Action("PaymentCancel", "Home", new { id = fine.Id }, Request.Url.Scheme);

            var payment = payPalService.CreatePayment(fine.Sum, returnUrl, cancelUrl);

            var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url").href;
            return Redirect(approvalUrl);
        }


        // Метод, обрабатывающий успешную оплату
        public ActionResult PaymentSuccess(int? id)
        {
            if (id == null)
            {
                // Логирование или обработка ошибки, если параметр отсутствует
                Console.WriteLine("ID штрафа не был передан.");
                return RedirectToAction("Index");
            }

            var fine = db.Penalty.FirstOrDefault(f => f.Id == id);
            if (fine != null)
            {
                db.Penalty.Remove(fine);
                db.SaveChanges();
                Console.WriteLine($"Штраф с ID {id} был успешно оплачен и удалён.");
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChangeLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                // Устанавливаем язык сессии
                HttpCookie langCookie = new HttpCookie("lang", lang)
                {
                    Expires = DateTime.Now.AddYears(1)
                };
                Response.Cookies.Add(langCookie);
            }

            return Redirect(Request.UrlReferrer.ToString()); // Возвращаем пользователя на предыдущую страницу
        }





        public ActionResult ExportToExcel()
        {
            var fines = db.Penalty.ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Fines");

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Car Number";
                worksheet.Cells[1, 3].Value = "Sum";
                worksheet.Cells[1, 4].Value = "Date";
                worksheet.Cells[1, 5].Value = "Name";
                worksheet.Cells[1, 6].Value = "User Email";
                worksheet.Cells[1, 7].Value = "Velocity";
                worksheet.Cells[1, 8].Value = "Is Paid";

                // Стилизация заголовков
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);  // Задаем цвет фона
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);  // Границы вокруг заголовков
                }

                // Добавление данных в таблицу
                for (int i = 0; i < fines.Count; i++)
                {
                    var fine = fines[i];
                    worksheet.Cells[i + 2, 1].Value = fine.Id;
                    worksheet.Cells[i + 2, 2].Value = fine.CarNumber;
                    worksheet.Cells[i + 2, 3].Value = fine.Sum;
                    worksheet.Cells[i + 2, 4].Value = fine.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = fine.Name;
                    worksheet.Cells[i + 2, 6].Value = fine.UserEmail;
                    worksheet.Cells[i + 2, 7].Value = fine.Velocity;
                    worksheet.Cells[i + 2, 8].Value = fine.IsPaid ? "Yes" : "No";

                    // Добавление границ к каждой ячейке с данными
                    for (int j = 1; j <= 8; j++)
                    {
                        worksheet.Cells[i + 2, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);  // Границы
                        worksheet.Cells[i + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;  // Центрирование текста
                    }
                }

                // Автоподбор ширины колонок
                worksheet.Cells.AutoFitColumns();

                // Генерация файла Excel
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Fines.xlsx");
            }
        }
            public ActionResult PaymentCancel()
        {
            // Перенаправление на список штрафов при отмене платежа
            return RedirectToAction("Index");
        }

        // Метод, обрабатывающий отмену платежа
        public ActionResult PaymentCancel(int id)
        {
            // Логика обработки отмены платежа
            ViewBag.Message = "Оплата была отменена.";
            return RedirectToAction("Fines");
        }

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
                // Рассчитываем сумму штрафа на основе скорости
                penalty.CalculateSumma();

                var user = db.Users.FirstOrDefault(u => u.Email == penalty.UserEmail);
                if (user != null)
                {
                    // Отправляем email прямо из контроллера
                    SendEmail(penalty.UserEmail, "Ваш штраф",
                              $"Tere, kell {penalty.Date} rikute kiirusepiirangut ({penalty.Velocity}), nii et te trahv {penalty.Sum}. Maksmiseks on teil 2 kuud");
                }

                db.Penalty.Add(penalty);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(penalty);
        }

        // Вспомогательный метод для отправки email
        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587; // Порт для TLS
                WebMail.EnableSsl = true;
                WebMail.UserName = "pinkod2222@gmail.com";  // Ваш email
                WebMail.Password = "mxnz nsgd kdia ijcp";      // Пароль приложения
                WebMail.From = "pinkod2222@gmail.com";      // Отправитель

                // Отправляем email
                WebMail.Send(toEmail, subject, body);

                Console.WriteLine("Письмо успешно отправлено!");
            }
            catch (SmtpException smtpEx)
            {
                // SMTP ошибки
                Console.WriteLine($"Ошибка SMTP: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                // Любые другие ошибки
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
            }
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
          
                fine.CalculateSumma();

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
       
    }
}
