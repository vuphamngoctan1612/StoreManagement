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
            SignUpWindow window = new SignUpWindow();

            window.ShowDialog();
        }

        void Login(LoginWindow parameter)
        {
            if (parameter == null)
            {
                return;
            }
            //check username
            if (String.IsNullOrEmpty(parameter.txtUser.Text))
            {
                CustomMessageBox.Show("Please enter your username!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUser.Focus();
                return;
            }
            //check password
            if (String.IsNullOrEmpty(parameter.txtPassword.Password))
            {
                CustomMessageBox.Show("Please enter your password!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtPassword.Focus();
                return;
            }

            string codedPassword = MD5Hash(parameter.txtPassword.Password);
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
                parameter.txtPassword.Password = "";
                parameter.Show();
            }
            else
            {
                CustomMessageBox.Show("Username or password is not valid!", "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
