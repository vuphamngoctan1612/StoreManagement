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

namespace StoreManagement.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {


        public HomeWindow HomeWindow { get; set; }
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand UpdateAccountCommand { get; set; }
        public ICommand LoadAccountCommand { get; set; }

        public ICommand SaveCommand { get; set; }


        public AccountViewModel()
        {

            UpdateAccountCommand = new RelayCommand<HomeWindow>((para) => true, (para) => UpdateAccount(para));
            LoadAccountCommand = new RelayCommand<HomeWindow>(para => true, para => LoadAccount(para));

        }

        private void UpdateAccount(HomeWindow para)
        {
            Account account = new Account();
            int id = 1;
            account = (Account)DataProvider.Instance.DB.Accounts.Where(x => x.ID == id).First();

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(account.Image);


            ///HomeWindow.txtName.Text = account.ID.ToString();

            para.txtName.Text = account.Username;
            para.txtName.SelectionStart = para.txtName.Text.Length;

            para.txtLocation.Text = account.Location;
            para.txtLocation.SelectionStart = para.txtLocation.Text.Length;

            para.txtPhoneNumber.Text = account.PhoneNumber;
            para.txtPhoneNumber.SelectionStart = para.txtPhoneNumber.Text.Length;

            para.txtPassword.Text = account.Password;
            para.txtPassword.SelectionStart = para.txtPassword.Text.Length;

            para.txtNewPassword.Text = account.Password;
            para.txtNewPassword.SelectionStart = para.txtNewPassword.Text.Length;

            para.txtNewPasswordAgain.Text = account.Password;
            para.txtNewPasswordAgain.SelectionStart = para.txtNewPasswordAgain.Text.Length;

            para.Title = "Update info account";
            para.grdBody_AccountSetting.Background = imageBrush;
            /*if (para.grd.Children.Count > 1)
            {
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
                window.grdImage.Children.Remove(window.grdImage.Children[1]);
            }*/

            para.ShowDialog();
        }
        private void LoadAccount(HomeWindow para)
        {
            this.HomeWindow = para;
            this.HomeWindow.Main.Children.Clear();
            string username = "";
            //string query = "SELECT " + username + " FROM Acount";
            Account account = DataProvider.Instance.DB.Accounts.FirstOrDefault(x => x.Username == username);

            usAccountSetting accountSetting = new usAccountSetting();
            accountSetting.tbName.Text = account.Username;
            
        }
    }
}
       