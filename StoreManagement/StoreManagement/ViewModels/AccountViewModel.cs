using System;
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
        //private bool isExisted;
        public HomeWindow HomeWindow { get; set; }
        private string imageFileName;
        private string username;
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand UpdateAccountCommand { get; set; }
        public ICommand LoadAccountOnWindowCommand { get; set; }
        //public ICommand DisplaynameCheckerCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ChooseImgAccountCommand { get; set; }
        public AccountViewModel()
        {
            UpdateAccountCommand = new RelayCommand<HomeWindow>((para) => true, (para) => UpdateAccount(para));
            //DisplaynameCheckerCommand = new RelayCommand<HomeWindow>((para) => true, para => NameChecker(para));
            ChangePasswordCommand = new RelayCommand<HomeWindow>((para) => true, para => Account_ChangePassword(para));
            LoadAccountOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadAccount(para));
            ChooseImgAccountCommand = new RelayCommand<Grid>(para => true, para => ChooseImg(para));
        }
        #region Commands_Logic
     
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
                }
                try
                {
                    Account acc = new Account();
                    acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == this.username).First();
                    acc.Image = Converter.Instance.ConvertImageToBytes(imageFileName);
                    DataProvider.Instance.DB.Accounts.AddOrUpdate(acc);
                    DataProvider.Instance.DB.SaveChanges();
                    MessageBox.Show("Cập nhật avatar thành công.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Account_ChangePassword(HomeWindow para)
        {
            if (string.IsNullOrEmpty(para.txt_Account_Password.Text))
            {
                para.txt_Account_Password.Focus();
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
            if (para.txt_Account_NewPassword.Text == para.txt_Account_Password.Text)
            {
                para.txt_Account_NewPassword.Focus();
                MessageBox.Show("Vui lòng nhập mật khẩu khác với mật khẩu hiện tại.");
                return;
            }
            if (para.txt_Account_NewPassword.Text != para.txt_Account_RetypeNewPassword.Text)
            {
                para.txt_Account_NewPassword.Focus();
                MessageBox.Show("Nhập lại mật khẩu chưa trùng khớp vui lòng nhập lại.");
                return;
            }
            try
            {
                Account acc = new Account();
                acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == this.username).First();

                if (para.txt_Account_Password.Text != acc.Password)
                {
                    MessageBox.Show("Mật khẩu hiện tại chưa đúng vui lòng nhập lại.");
                    return;
                }
                else
                {
                    acc.Password = para.txt_Account_RetypeNewPassword.Text;
                    DataProvider.Instance.DB.Accounts.AddOrUpdate(acc);
                    DataProvider.Instance.DB.SaveChanges();
                    MessageBox.Show("Thay đổi mật khẩu thành công.");
                    para.txt_Account_Password.Clear();
                    para.txt_Account_NewPassword.Clear();
                    para.txt_Account_RetypeNewPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            para.Title = "Update info account";
            try
            {
                Account acc = new Account();
                acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == this.username).First();

                if (para.txt_Account_Name.Text == acc.DisplayName && para.txt_Account_Location.Text == acc.Location && para.txt_Account_PhoneNumber.Text == acc.PhoneNumber)
                {
                    MessageBox.Show("Thông tin không thay đổi");
                    return;
                }
                else
                {
                    acc.DisplayName = para.txt_Account_Name.Text;
                    acc.Location = para.txt_Account_Location.Text;
                    acc.PhoneNumber = para.txt_Account_PhoneNumber.Text;
                    DataProvider.Instance.DB.Accounts.AddOrUpdate(acc);
                    DataProvider.Instance.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin thành công.");
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
            this.username = para.txbUsername.Text;
            //string query = "SELECT " + username + " FROM Acount;

            Account account = DataProvider.Instance.DB.Accounts.FirstOrDefault(x => x.Username == this.username);

            para.txt_Account_Name.Text = account.DisplayName;
            para.txt_Account_Location.Text = account.Location;
            para.txt_Account_PhoneNumber.Text = account.PhoneNumber;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(account.Image);
            if (para.grdImageAccount.Children.Count > 1)
            {
                para.grdImageAccount.Children.Remove(para.grdImageAccount.Children[0]);
                para.grdImageAccount.Children.Remove(para.icoImageAccount);
            }
            para.grdImageAccount.Background = imageBrush;
        }
        #endregion
    }
}