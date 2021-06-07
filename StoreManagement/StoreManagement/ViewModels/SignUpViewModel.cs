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

namespace StoreManagement.ViewModels
{
    class SignUpViewModel : BaseViewModel
    {
        private bool isSucceed = false;
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
                    para.Children.Remove(para.Children[1]);
                }
            }
        }

        public void SignUp(SignUpWindow parameter)
        {
            if (parameter == null)
            {
                return;
            }
            if (String.IsNullOrEmpty(parameter.displayname.Text))
            {
                MessageBox.Show("Vui lòng nhập tên ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.displayname.Focus();
                return;
            }
            if (String.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUsername.Focus();
                return;
            }
            if (String.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPassword.Focus();
                return;
            }
            if (String.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                MessageBox.Show("Vui lòng xác thực mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }

            if (parameter.pwbPassword.Password != parameter.pwbPasswordConfirm.Password)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                byte[] imgByteArr;
                if (imageFileName == null)
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
                }
                else
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                }

                Account acc = new Account();
                acc.Username = parameter.txtUsername.Text;
                acc.Password = MD5Hash(parameter.pwbPassword.Password);
                acc.DisplayName = parameter.displayname.Text;
                acc.Image = imgByteArr;
                DataProvider.Instance.DB.Accounts.Add(acc);
                DataProvider.Instance.DB.SaveChanges();
                this.isSucceed = true;
            }
            catch
            {
                MessageBox.Show("Tài khoản đã tồn tại! Vui lòng nhập tài khoản khác", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUsername.Focus();
                return;
            }
            finally
            {
                if (this.isSucceed)
                {
                    parameter.Close();
                }
            }
        }

        private void CloseWindow(SignUpWindow para)
        {
            para.Close();
        }
    }
}