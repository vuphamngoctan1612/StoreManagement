using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement
{
    public class CurrentAccount
    {
        private static CurrentAccount instance;

        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string DisplayName { get; set; }
        public static byte[] Image { get; set; }
        public static string Location { get; set; }
        public static string PhoneNumber { get; set; }

        public static CurrentAccount Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new CurrentAccount();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public CurrentAccount()
        {
            
        }

        public void ConvertAccToCurrentAcc(string username)
        {
            Account account = new Account();
            account = DataProvider.Instance.DB.Accounts.Where(p => p.Username == username).First();

            Username = account.Username;
            Password = account.Password;
            DisplayName = account.DisplayName;
            Image = account.Image;
            Location = account.Location;
            PhoneNumber = account.PhoneNumber;
        }
        public void ConvertAccToCurrentAcc(Account account)
        {            
            Username = account.Username;
            Password = account.Password;
            DisplayName = account.DisplayName;
            Image = account.Image;
            Location = account.Location;
            PhoneNumber = account.PhoneNumber;
        }
    }
}
