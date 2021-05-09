﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreManagement.Models;
using StoreManagement.Views;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StoreManagement.Resources.UserControls;
using Microsoft.Win32;
using System.Windows;
using System.Data.Entity.Migrations;

namespace StoreManagement.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private bool isExisted;
        public HomeWindow HomeWindow { get; set; }
        private string imageFileName;
        private string username;
      
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand UpdateAccountCommand { get; set; }
        public ICommand LoadAccountOnWindowCommand { get; set; }
        public ICommand UsernameChecker { get; set; } //Check xem tên sắp đổi có bị trùng không
        public ICommand SaveCommand { get; set; }
        public ICommand ChooseImgAccountCommand { get; set; }


        public AccountViewModel()
        {
            UpdateAccountCommand = new RelayCommand<HomeWindow>((para) => true, (para) => UpdateAccount(para));
            UsernameChecker = new RelayCommand<HomeWindow>((para) => true, para => NameChecker(para));
            LoadAccountOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadAccount(para));
            ChooseImgAccountCommand = new RelayCommand<Grid>(para => true, para => ChooseImg(para));
        }
        private void NameChecker(HomeWindow para)
        {
            if (string.IsNullOrEmpty(para.txt_Account_Name.Text))
            {
                para.txt_Account_Name.Focus();
                MessageBox.Show("Vui lòng nhập tên để kiểm tra!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string queryAccount = "select* from account";
            List<Account> accounts = DataProvider.Instance.DB.Accounts.SqlQuery(queryAccount).ToList();
            foreach (Account acc in accounts)
            {
                if (para.txt_Account_Name.Text == acc.Username)
                {
                    isExisted = true;
                    break;
                }
            }

            if (isExisted)
            {
                MessageBox.Show("Tên này đã tồn tại, vui lòng nhập tên khác", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                isExisted = false; //trả về false để kiếm tra tên nhập kế tiếp
                para.txt_Account_Name.Clear();//Clear data txtName để user nhập tên mới
                para.txt_Account_Name.Focus();
                return;
            }
            else
            {
                MessageBox.Show("Tên hợp lệ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ChooseImg(Grid para)
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

        private void UpdateAccount(HomeWindow para)
        {
            if (string.IsNullOrEmpty(para.txt_Account_Name.Text))
            {
                para.txt_Account_Name.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txt_Account_Location.Text))
            {
                para.txt_Account_Location.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txt_Account_PhoneNumber.Text))
            {
                para.txt_Account_PhoneNumber.Focus();
                return;
            }



            if (string.IsNullOrEmpty(para.txt_Account_NewPassword.Text))
            {
                para.txt_Account_NewPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txt_Account_RetypeNewPassword.Text))
            {
                para.txt_Account_RetypeNewPassword.Focus();
                return;
            }

            if (para.txt_Account_NewPassword.Text != para.txt_Account_RetypeNewPassword.Text)
            {
                para.txt_Account_NewPassword.Focus();
                return;
            }

            para.Title = "Update info account";
            //delete image
            /* para..Background = imageBrush;
             if (para.grdImage.Children.Count > 1)
             {
                 para.grdImage.Children.Remove(para.grdImage.Children[0]);
                 para.grdImage.Children.Remove(para.grdImage.Children[1]);
             }
            */
            //para.ShowDialog();

            

            byte[] imgByteArr;
            if (imageFileName == null)
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
            }
            else
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            }

            try
            {
                Account acc = new Account();
                acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == this.username).First();

                if (para.txt_Account_NewPassword.Text == acc.Password && para.txt_Account_Name.Text == acc.DisplayName)
                {
                    MessageBox.Show("Thông tin không thay đổi");
                    return;
                }else
                {

                    acc.DisplayName = para.txt_Account_Name.Text;
                    acc.Password = para.txt_Account_NewPassword.Text;
                    acc.Image = imgByteArr;
                    // acc.Location = para.txtLocation.Text;
                    //acc.PhoneNumber = para.txtPhoneNumber.Text;

                    DataProvider.Instance.DB.Accounts.AddOrUpdate(acc);
                    DataProvider.Instance.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thành công.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void LoadAccount(HomeWindow para)
        {
            this.HomeWindow = para;

            //this.HomeWindow.Main.Children.Clear();
            //string query = "SELECT " + username + " FROM Acount;
           
            Account account = DataProvider.Instance.DB.Accounts.FirstOrDefault(x => x.Username == this.username);
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(account.Image);

            para.txt_Account_Name.Text = account.DisplayName;
            //para.txtPassword.Text = account.Password;  ////Khi load thông tin không nên load luôn mật khẩu
            //para.txtLocation.Text = account.Location;
            //para.txtPhoneNumber.Text = account.PhoneNumber;

            para.grdImageAccount.Background = imageBrush;
        }

    }
}



    

       