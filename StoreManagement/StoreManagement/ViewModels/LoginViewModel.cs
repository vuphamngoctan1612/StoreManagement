using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool isLogin = false;

        public ICommand LoginCommand { get; set; }

        public string Username { get => this.Username; set { Username = value; OnPropertyChanged(); } }
        public string Password { get => this.Password; set { Password = value; OnPropertyChanged(); } }
        public bool IsLogin { get => isLogin; set => isLogin = value; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<object>((para) => true, (para) => Login(para));
        }

        public void Login(object para)
        {
            if (para == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.Username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string password = MD5Hash(this.Password);
            var countAcc = DataProvider.Instance.DB.ACCOUNTs.Where(p => p.USERNAME == this.Username && p.PASSWORD == password).Count();

            if(countAcc > 0)
            {
                IsLogin = true;                
            }
            else
            {
                IsLogin = false;
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }
    }
}
