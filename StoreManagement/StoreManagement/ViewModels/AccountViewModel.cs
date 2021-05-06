using System;
using System.Collections.Generic;
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


        public HomeWindow HomeWindow { get; set; }
        private string imageFileName;
        private string username;
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand UpdateAccountCommand { get; set; }
        public ICommand LoadAccountOnWindowCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand ChooseImgAccountCommand { get; set; }

        public AccountViewModel()
        {
            username = "Na";
            
            UpdateAccountCommand = new RelayCommand<HomeWindow>((para) => true, (para) => UpdateAccount(para));
            LoadAccountOnWindowCommand = new RelayCommand<HomeWindow>(para => true, para => LoadAccount(para));
            ChooseImgAccountCommand = new RelayCommand<Grid>(para => true, para => ChooseImg(para));
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
            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
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


            if (string.IsNullOrEmpty(para.txtNewPassword.Text))
            {
                para.txtNewPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtNewPasswordAgain.Text))
            {
                para.txtNewPasswordAgain.Focus();
                return;
            }

            if (para.txtNewPassword.Text != para.txtNewPasswordAgain.Text)
            {
                para.txtNewPassword.Focus();
                return;
            }

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

                if (para.txtNewPassword.Text == acc.Password && para.txtName.Text == acc.DisplayName)
                {
                    MessageBox.Show("Thông tin không thay đổi");
                    return;
                }
                {

                    acc.DisplayName = para.txtName.Text;
                    acc.Password = para.txtNewPassword.Text;
                    acc.Image = imgByteArr;
                    acc.Location = para.txtLocation.Text;
                    acc.PhoneNumber = para.txtPhoneNumber.Text;

                    DataProvider.Instance.DB.Accounts.AddOrUpdate(acc);
                    DataProvider.Instance.DB.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MessageBox.Show("Cập nhật thành công.");
            }

        }
        private void LoadAccount(HomeWindow para)
        {
            this.HomeWindow = para;

            //this.HomeWindow.Main.Children.Clear();
            //string query = "SELECT " + username + " FROM Acount";
            Account account = DataProvider.Instance.DB.Accounts.FirstOrDefault(x => x.Username == this.username);
           
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(account.Image);

            para.txtName.Text = account.DisplayName;
            para.txtPassword.Text = account.Password;
            para.txtLocation.Text = account.Location;
            para.txtPhoneNumber.Text = account.PhoneNumber;

            para.grdImageAccount.Background = imageBrush;
            
        }
    }
}
       