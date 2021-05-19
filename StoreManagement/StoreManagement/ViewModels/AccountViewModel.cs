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
        private string tempUsername;
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand UpdateAccountCommand { get; set; }
        public ICommand LoadAccountOnWindowCommand { get; set; }
        //public ICommand DisplaynameCheckerCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ChooseImgAccountCommand { get; set; }
        //change password window
        public ICommand ChangePassword_SaveCommand { get; set; }
        public ICommand Close_ChangePasswordWindowCommand { get; set; }
        //info account window
        public ICommand InfoAcc_SaveCommand { get; set; }
        public ICommand InfoAcc_CloseWindowCommand { get; set; }
        //home window
        public ICommand ShowProfileCommand { get; set; }
        public ICommand ShowChangePasswordCommand { get; set; }
        public ICommand LogOutCommand { get; set; }

        public AccountViewModel()
        {
            UpdateAccountCommand = new RelayCommand<HomeWindow>((para) => true, (para) => UpdateAccount(para));
            //DisplaynameCheckerCommand = new RelayCommand<HomeWindow>((para) => true, para => NameChecker(para));
            ChangePasswordCommand = new RelayCommand<HomeWindow>((para) => true, para => Account_ChangePassword(para));
            LoadAccountOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadAccount(para));
            ChooseImgAccountCommand = new RelayCommand<Grid>(para => true, para => ChooseImg(para));
            //change password window
            ChangePassword_SaveCommand = new RelayCommand<ChangePasswordWindow>((para) => true, para => ChangePassword(para));
            Close_ChangePasswordWindowCommand = new RelayCommand<ChangePasswordWindow>((para) => true, para => CloseChangePasswordWindow(para));
            //info account window
            InfoAcc_SaveCommand = new RelayCommand<InfoAccountWindow>((para) => true, para => InfoAcc_Save(para));
            InfoAcc_CloseWindowCommand = new RelayCommand<InfoAccountWindow>((para) => true, para => InfoAcc_CloseWindow(para));
            //home window
            ShowProfileCommand = new RelayCommand<HomeWindow>((para) => true, para => ShowProfileAccountWindow(para));
            ShowChangePasswordCommand = new RelayCommand<HomeWindow>((para) => true, para => ShowChangePasswordWindow(para));
            LogOutCommand = new RelayCommand<HomeWindow>((para) => true, para => LogOut(para));
        }

        //home window
        private void LogOut(HomeWindow para)
        {
            para.Close();
        }

        private void ShowChangePasswordWindow(HomeWindow para)
        {
            this.HomeWindow = para;
            ChangePasswordWindow window = new ChangePasswordWindow();
            window.ShowDialog();
        }
        private void ShowProfileAccountWindow(HomeWindow para)
        {
            this.HomeWindow = para;
            InfoAccountWindow window = new InfoAccountWindow();
            ImageBrush imageBrush = new ImageBrush();

            window.txtUsername.Text = CurrentAccount.Username;
            window.txtDisplayName.Text = CurrentAccount.DisplayName;
            window.txtDisplayName.SelectionStart = window.txtDisplayName.Text.Length;

            window.txtLocation.Text = CurrentAccount.Location;
            window.txtLocation.SelectionStart = window.txtLocation.Text.Length;

            window.txtPhoneNumber.Text = CurrentAccount.PhoneNumber;
            window.txtLocation.SelectionStart = window.txtLocation.Text.Length;

            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(CurrentAccount.Image);
            window.grdImage.Background = imageBrush;

            if (window.grdImage.Children.Count != 0)
            {
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
            }

            window.ShowDialog();
        }        
        //info account window
        private void InfoAcc_Save(InfoAccountWindow para)
        {
            if (string.IsNullOrEmpty(para.txtDisplayName.Text))
            {
                para.txtDisplayName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtLocation.Text))
            {
                para.txtLocation.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtPhoneNumber.Text))
            {
                para.txtPhoneNumber.Focus();
                return;
            }

            string username = para.txtUsername.Text;
            string displayname = para.txtDisplayName.Text;
            string location = para.txtLocation.Text;
            string phonenumber = para.txtPhoneNumber.Text;
            byte[] imgByteArr;

            if (imageFileName == null)
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
            }
            else
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            }

            Account account = DataProvider.Instance.DB.Accounts.SingleOrDefault(p => p.Username == username);
            if (account != null)
            {
                account.DisplayName = displayname;
                account.Location = location;
                account.PhoneNumber = phonenumber;
                account.Image = imgByteArr;
                DataProvider.Instance.DB.SaveChanges();

                CurrentAccount.Instance.ConvertAccToCurrentAcc(account);
            }


            para.Close();
        }
        private void InfoAcc_CloseWindow(InfoAccountWindow para)
        {
            para.Close();
        }
        //change password window
        private void ChangePassword(ChangePasswordWindow para)
        {
            if (string.IsNullOrEmpty(para.txtUsername.Text))
            {
                para.txtUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.pwbPassword.Password))
            {
                para.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.pwbNewPassword.Password))
            {
                para.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.pwbConfirmNewPassword.Password))
            {
                para.pwbConfirmNewPassword.Focus();
                return;
            }

            string username = para.txtUsername.Text;
            string password = MD5Hash(para.pwbPassword.Password);

            var checkAcc = DataProvider.Instance.DB.Accounts.Where(p => p.Username == username && p.Password == password).Count();
            if(checkAcc <= 0)
            {
                para.pwbPassword.Focus();
                return;
            }
            else
            {
                if (para.pwbNewPassword.Password != para.pwbConfirmNewPassword.Password)
                {
                    para.pwbConfirmNewPassword.Focus();
                    return;
                }

                Account account = DataProvider.Instance.DB.Accounts.SingleOrDefault(p => p.Username == username);
                string newPassword = MD5Hash(para.pwbNewPassword.Password);
                if (account != null)
                {
                    account.Password = newPassword;
                    DataProvider.Instance.DB.SaveChanges();

                    CurrentAccount.Instance.ConvertAccToCurrentAcc(account);
                }
            }

            para.Close();
        }
        private void CloseChangePasswordWindow(ChangePasswordWindow para)
        {
            para.Close();
        }
        #region Commands_Logic     
        private void ChooseImg(Grid para)
        {
            //this.HomeWindow = parameter;
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
                if (para.Children.Count != 0)
                {
                    para.Children.Remove(para.Children[0]);
                    para.Children.Remove(para.Children[0]);
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
                acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == tempUsername).First();

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
                acc = DataProvider.Instance.DB.Accounts.Where(x => x.Username == tempUsername).First();

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
            //this.HomeWindow = para;
            //tempUsername = para.txt_Account_Username.Text;
            ////string query = "SELECT " + username + " FROM Acount;

            //Account account = DataProvider.Instance.DB.Accounts.FirstOrDefault(x => x.Username == tempUsername);

            //para.txt_Account_Name.Text = account.DisplayName;
            //para.txt_Account_Location.Text = account.Location;
            //para.txt_Account_PhoneNumber.Text = account.PhoneNumber;

            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(account.Image);
            //if (para.grdImageAccount.Children.Count > 1)
            //{
            //    para.grdImageAccount.Children.Remove(para.grdImageAccount.Children[0]);
            //    para.grdImageAccount.Children.Remove(para.icoImageAccount);
            //}
            //para.grdImageAccount.Background = imageBrush;
        }
        #endregion
    }
}