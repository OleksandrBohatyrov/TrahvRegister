using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;

namespace TrajvRegister10.Models
{
    [Table("Fine")]
    public class Fine
    {
        public int Id { get; set; }
        public string CarNumber { get; set; }
        public decimal Sum { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public bool IsPaid { get; set; }


        [Required]
        public string UserEmail { get; set; }  // Email пользователя

        public int Velocity { get; set; }

        public void CalculateSumma()
        {
            if (Velocity <= 50)
            {
                Sum = 50;
            }
            else if (Velocity > 50 && Velocity <= 70)
            {
                Sum = 100;
            }
            else if (Velocity > 70 && Velocity <= 90)
            {
                Sum = 200;
            }
            else
            {
                Sum = 400;
            }
        }

        //public void SendMessage()
        //{
        //    try
        //    {
        //        // Настройки для WebMail
        //        System.Web.Helpers.WebMail.SmtpServer = "smtp.gmail.com";
        //        System.Web.Helpers.WebMail.SmtpPort = 587;  // Порт для TLS
        //        System.Web.Helpers.WebMail.EnableSsl = true;
        //        System.Web.Helpers.WebMail.UserName = "nepridumalnazvaniepocht@gmail.com";  // Ваш email
        //        System.Web.Helpers.WebMail.Password = "rnlt mfvn ftjb usxu";  // Пароль приложения
        //        System.Web.Helpers.WebMail.From = "nepridumalnazvaniepocht@gmail.com";  // Отправитель

        //        // Определение переменных для отправки письма
        //        string message1 = "Trahv infromatsion " + CarNumber;
        //        string subject = message1;
        //        string body = $"Tere {Name}, Auto number: {CarNumber}. Rikute kiirusepiirangut ({Velocity}), nii et te trahv {Sum}€. " +
        //                      $"Trahvi kuupäev: {Date.Year}.{Date.Month}.{Date.Day}. Maksmiseks on teil 2 kuud.";

        //        // Отправка письма с использованием WebMail
        //        System.Web.Helpers.WebMail.Send(UserEmail, subject, body);

        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка ошибок
        //        Console.WriteLine("Failed to send email. Error: " + ex.Message);
        //    }
        //}

    }
}
