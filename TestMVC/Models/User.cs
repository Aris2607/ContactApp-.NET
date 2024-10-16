using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
