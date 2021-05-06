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
        private string imageFileName;
        public ICommand SelectImageCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand PasswordConfirmChangedCommand { get; set; }
    
        public ICommand OpenHomeWinDowCommand { get; set; }
        private bool isSignUp;
        private bool isExisted;
        public bool IsSignUp { get => isSignUp; set => isSignUp = value; }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private string userName;
        public string UserName { get => userName; set { userName = value; OnPropertyChanged(); } }
        private string passwordConfirm;
        public string PasswordConfirm { get => passwordConfirm; set { passwordConfirm = value; OnPropertyChanged(); } }
        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand<SignUpWindow>((parameter) => true, (parameter) => SignUp(parameter));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingPassword(parameter));
            PasswordConfirmChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingConfirmPassword(parameter));
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
        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.password);
        }
        public void EncodingConfirmPassword(PasswordBox parameter)
        {
            this.passwordConfirm = parameter.Password;
            this.passwordConfirm = MD5Hash(this.passwordConfirm);
        }
       
        public void SignUp(SignUpWindow parameter)
        {
            isExisted = false;
            isSignUp = false;
            if (parameter == null)
            {
                return;
            }
           
            //// Check username
            //if (string.IsNullOrEmpty(parameter.txtUsername.Text) || Account.Instance.(parameter.txtUsername.Text))
            //{
            //    parameter.txtUsername.Focus();
            //    parameter.txtUsername.Text = "";
            //    return;
            //}
            //Check password
            if (String.IsNullOrEmpty(parameter.pwbPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPassword.Focus();
                return;
            }
            if (String.IsNullOrEmpty(parameter.pwbPasswordConfirm.Text))
            {
                MessageBox.Show("Vui lòng xác thực mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            if (parameter.pwbPassword.Text != parameter.pwbPasswordConfirm.Text)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (imageFileName == null)
            {
                MessageBox.Show("Vui lòng thêm hình ảnh", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //SQLConnection connection = new SQLConnection();
            //if (!Regex.IsMatch(parameter.txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            //{
            //    parameter.txtUsername.Focus();
            //    return;
            //}
            string queryAccount = "select* from account";
            List<Account> accounts = DataProvider.Instance.DB.Accounts.SqlQuery(queryAccount).ToList();
            foreach (Account acc in accounts)
            {
                if (parameter.txtUsername.Text == acc.Username)
                {
                    isExisted = true;
                    break;
                }
            }
            if (isExisted)
            {
                MessageBox.Show("Tài khoản đã tồn tại! Vui lòng nhập tài khoản khác", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

               
                byte[] imgByteArr;
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);                

                Account acc = new Account();
                acc.Username = parameter.txtUsername.Text;
                acc.Password = MD5Hash(parameter.pwbPassword.Text);
                acc.DisplayName = parameter.displayname.Text;
                acc.Image = imgByteArr;
               

                DataProvider.Instance.DB.Accounts.Add(acc);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch 
            {
               
            }
            finally
            {
                parameter.Close();
            }


            //int idAccount = Account.Instance.SetNewID();
            //if (idAccount != -1)
            //{
            //    Account newAccount = new Account(idAccount, parameter.txtUsername.Text.ToString(), password, imageFileName);
            //    AccountDAL.Instance.AddIntoDB(newAccount);
            //    selectedEmployee.IdAccount = idAccount;
            //    if (EmployeeDAL.Instance.UpdateIdAccount(selectedEmployee))
            //    {
            //        MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //        isSignUp = true;
            //        parameter.cboSelectEmployee.Text = "";
            //        parameter.txtUsername.Text = null;
            //        parameter.pwbPassword.Password = "";
            //        parameter.pwbPasswordConfirm.Password = "";
            //    }
            //}
            //Account newAccount = new Account(idAccount, parameter.txtUsername.Text.ToString(), password);
            //AccountDAL.Instance.AddIntoDB(newAccount);
            //selectedEmployee.IdAccount = idAccount;
            //if (EmployeeDAL.Instance.UpdateIdAccount(selectedEmployee))

            //{
            //    MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //    isSignUp = true;

            //    parameter.txtUsername.Text = null;
            //    parameter.pwbPassword.Text = "";
            //    parameter.pwbPasswordConfirm.Text = "";
            //    parameter.displayname.Text = "";


            //}
            //HomeWindow HoMe = new HomeWindow();
            //HoMe.ShowDialog();
        }
        }
    }

//}
//}
