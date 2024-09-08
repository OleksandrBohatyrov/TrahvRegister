using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Net;

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

        public void SendMessage()
        {

            try
            {

                var fromAddress = new MailAddress("smtp776@gmail.com", "Politsei");
                var toAddress = new MailAddress(UserEmail, Name);
                const string fromPassword = "qyyd usfw atbw omex";
                string message1 = "Trahv infromatsion " + CarNumber;
                string subject = message1;
                string body = $"Tere, kell {Date} rikute kiirusepiirangut ({Velocity}), nii et te trahv {Sum}. Maksmiseks on teil 2 kuud";
           

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // E.g., smtp.gmail.com for Gmail
                    Port = 587, // 465 for SSL, 587 for TLS
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Create the email message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    // Send the email
                    smtp.Send(message);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }


        }
    }
}
