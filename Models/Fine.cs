using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


  

        [Required]
        public string UserEmail { get; set; }  // Email пользователя

        public int Velocity { get; set; }

        public void CalculateSumma()
        {
            int speedOver = Velocity;

            if (speedOver <= 20)
            {
                Sum = 50;
            }
            else if (speedOver <= 40)
            {
                Sum = 100;
            }
            else if (speedOver <= 60)
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
