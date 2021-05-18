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


        public ICommand LoginCommand { get; set; }

        public ICommand OpenSignUpWindowCommand { get; set; }


        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<LoginWindow>((p) => { return true; }, (p) => { Login(p); });


            OpenSignUpWindowCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) => OpenSignUpWindow(parameter));

        }

        public void OpenSignUpWindow(LoginWindow parameter)
        {
            SignUpWindow SignUp = new SignUpWindow();

            SignUp.Show();

        }

        void Login(LoginWindow parameter)
        {

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

            string codedPassword = MD5Hash(parameter.txtPassword.Text);
            var checkACC = DataProvider.Instance.DB.Accounts.Where(x => x.Username == parameter.txtUser.Text && x.Password == codedPassword).Count();
            if (checkACC > 0)
            {
                HomeWindow homeWindow = new HomeWindow();
                CurrentAccount.Instance.ConvertAccToCurrentAcc(parameter.txtUser.Text);
                parameter.Hide();

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(CurrentAccount.Image);
                homeWindow.grdAcc_Image.Background = imageBrush;
                homeWindow.menu_Acc_DisplayName.Header = CurrentAccount.DisplayName;
                if (homeWindow.grdAcc_Image.Children.Count != 0)
                {
                    homeWindow.grdAcc_Image.Children.Remove(homeWindow.grdAcc_Image.Children[0]);
                }

                homeWindow.ShowDialog();
                parameter.Close();

            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

    }





}
