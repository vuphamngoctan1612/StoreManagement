using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.Windows.Input;
using StoreManagement.Views;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Windows;


namespace StoreManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; }
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand OpenSignUpWindowCommand { get; set; }
        public ICommand OpenHomeWindowCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<LoginWindow>((p) => { return true; }, (p) => { Login(p); });
            CloseCommand = new RelayCommand<LoginWindow>((p) => { return true; }, (p) => { p.Close(); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            OpenSignUpWindowCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) => OpenSignUpWindow(parameter));
            OpenHomeWindowCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) => OpenHomeWindow(parameter));
        }
        //public void OpenSignUpWindow(Window parameter)
        //{
        //    SignUpWindow signUp = new SignUpWindow();
        //    parameter.Show();
        //}
        public void OpenSignUpWindow(LoginWindow parameter)
        {
          SignUpWindow signUp = new SignUpWindow();
            //parameter.WindowStyle = WindowStyle.None;
            signUp.ShowDialog();
            //parameter.WindowStyle = WindowStyle.SingleBorderWindow;
            //parameter.Opacity = 1;
            //parameter.Show();
        }
        private void OpenHomeWindow(LoginWindow parameter)
        {
            HomeWindow homeWindow = new HomeWindow();
            homeWindow.ShowDialog();
        }
        void Login(LoginWindow parameter)
        {
            IsLogin = false;
            if (parameter == null)
            {
                return;
            }
            string queryAccount = "select* from account";

            List<Account> accounts = DataProvider.Instance.DB.Accounts.SqlQuery(queryAccount).ToList();
            //check username
            if (String.IsNullOrEmpty(parameter.txtUser.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUser.Focus();
                return;
            }
            //check password
            if (String.IsNullOrEmpty(parameter.txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtPassword.Focus();
                return;
            }
            foreach (Account acc in accounts)
            {
                if (parameter.txtUser.Text == acc.Username)
                {
                    string codedPassword = MD5Hash(parameter.txtPassword.Text);
                    if (codedPassword == acc.Password)
                    {
                        IsLogin = true;
                    }
                    break;
                }
            }
            if (IsLogin)
            {
                parameter.Hide();
                OpenHomeWindow(parameter);
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại");
            }
            //var accCount = DataProvider.Instance.DB.Accounts.Where(x => x.Username == UserName && x.Password == passEncode).Count();

            //if (accCount > 0)
            //{
            //    IsLogin = true;

            //    parameter.Close();
            //}
            //else
            //{
            //    IsLogin = false;
            //    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            //}



        }

    }





}
