using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool isLogin = false;

        public ICommand LoginWindowCommand { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLogin { get => isLogin; set => isLogin = value; }

        public LoginViewModel()
        {
            LoginWindowCommand = new RelayCommand<object>((para) => true, (para) => Login(para));
        }

        public void Login(object para)
        {

        }
    }
}
