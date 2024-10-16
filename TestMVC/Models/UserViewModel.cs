namespace TestMVC.Models
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
