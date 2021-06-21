using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StoreManagement.Views;
using StoreManagement.ViewModels;
using System.Windows.Controls;
using StoreManagement.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreManagement.Validations;

namespace StoreManagement.ViewModels
{
    class SignUpViewModel : BaseViewModel
    {
        private bool isSucceed = false;
        public bool IsSucceed { get => isSucceed; set => isSucceed = value; }

        private string imageFileName;

        public ICommand SelectImageCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand<SignUpWindow>((parameter) => true, (parameter) => SignUp(parameter));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            CloseWindowCommand = new RelayCommand<SignUpWindow>((para) => true, (para) => CloseWindow(para));
        }

        private void ChooseImage(Grid para)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                para.Background = imageBrush;
                if (para.Children.Count > 1)
                {
                    para.Children.Remove(para.Children[0]);
                    para.Children.Remove(para.Children[0]);
                }
            }
        }

        public void SignUp(SignUpWindow parameter)
        {
            if (parameter == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(parameter.displayname.Text))
            {
                CustomMessageBox.Show("Please enter your display name!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.displayname.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                CustomMessageBox.Show("Please enter your username!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Please enter your password!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Please enter your confirm password!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            if (parameter.grdImage.Background == null)
            {
                CustomMessageBox.Show("Please select your avatar", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parameter.pwbPassword.Password != parameter.pwbPasswordConfirm.Password)
            {
                CustomMessageBox.Show("Password does not match!", "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }

            string displayName = parameter.displayname.Text;
            string username = parameter.txtUsername.Text;
            string password = MD5Hash(parameter.pwbPassword.Password);
            byte[] imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);

            if (DataProvider.Instance.DB.Accounts.Where(p=>p.Username == username).Count() == 0)
            {
                Account account = new Account();
                account.DisplayName = displayName;
                account.Username = username;
                account.Password = password;
                account.Image = imgByteArr;

                DataProvider.Instance.DB.Accounts.Add(account);
                DataProvider.Instance.DB.SaveChanges();
                this.IsSucceed = true;

                CustomMessageBox.Show("Successful account registration!", "Notify", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                this.IsSucceed = false;
                CustomMessageBox.Show("Username already exists! Please enter other account", "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.txtUsername.Focus();
                return;
            }
            
            //finally
            if (this.IsSucceed)
            {
                parameter.Close();
            }

            //if (imageFileName == null)
            //{
            //    imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
            //}
            //else
            //{
            //    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            //}
        }

        private void CloseWindow(SignUpWindow para)
        {
            para.Close();
        }
    }
}