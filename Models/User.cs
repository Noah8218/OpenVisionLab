using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Image ProfilePicture { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthdate { get; set; }    

        public User()
        { 

        }
        public User Login(string username, string pass)
        {
            var user = GetUsers().Find(x => x.Username == username && x.Password == pass);
            return user;
        }

        private List< User> GetUsers()
        {
           var UsersList = new List<User>();
           UsersList.Add(new User
           {
               Id = 1,
               Username = "admin",
               Password = "admin",
               FirstName = "Rummer John",
               LastName = "Cranel",
               //ProfilePicture=Properties.Resources.RummerProfile,
               Email = "rummerCranel@gmail.com",
               PhoneNumber="16861125 9634",
               Birthdate = new DateTime(1999, 10, 18)
           });

           UsersList.Add(new User
           {
               Id = 2,
               Username = "arely",
               Password = "arely123",
               FirstName = "Arely",
               LastName = "Smith",
               //ProfilePicture=Properties.Resources.AralyProfile,
               Email = "Are@gmail.com",
               PhoneNumber = "16850718 4565",
               Birthdate = new DateTime(2002, 04, 27)
           });

           return UsersList;
        }

    }
}
