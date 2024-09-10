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
        public string UserEmail { get; set; }  

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
    }
}
