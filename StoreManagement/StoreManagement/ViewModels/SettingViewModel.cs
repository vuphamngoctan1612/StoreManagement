using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StoreManagement.ViewModels
{
    class SettingViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }
        public int PageNumber { get; set; }
        public List<TypeOfAgency> ListType
        {
            get; set;
        }
        public string cache;
        public ICommand LoadSettingWindowCommand { get; set; }
        public ICommand SaveRulesType_SettingCommand { get; set; }
        public ICommand SaveRulesProduct_SettingCommand { get; set; }
        public ICommand EditTypeCommand { get; set; }
        public ICommand DeleteTypeCommand { get; set; }
        public ICommand SaveStoreCommand { get; set; }
        public ICommand CloseStoreWindowCommand { get; set; }
        public ICommand OpenAddTypeCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }

        public SettingViewModel()
        {
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();
            cache = "";

            LoadSettingWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadSettingWindow(para));
            SaveRulesType_SettingCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SaveRulesType_Setting(para));
            EditTypeCommand = new RelayCommand<TypeOfAgencyUC>((para) => true, (para) => EditType(para));
            DeleteTypeCommand = new RelayCommand<TypeOfAgencyUC>((para) => true, (para) => DeleteType(para));
            SaveStoreCommand = new RelayCommand<AddTypeOfAgencyWindow>((para) => true, (para) => SaveStore(para));
            CloseStoreWindowCommand = new RelayCommand<AddTypeOfAgencyWindow>((para) => true, (para) => para.Close());
            OpenAddTypeCommand = new RelayCommand<Object>((para) => true, (para) => OpenAddType());
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
        }

        private void OpenAddType()
        {
            AddTypeOfAgencyWindow wd = new AddTypeOfAgencyWindow();
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();

            try
            {
                wd.txtID.Text = (this.ListType.Last().ID + 1).ToString();
            }
            catch
            {
                wd.txtID.Text = "1";
            }
            finally
            {
                wd.txtName.Text = null;
                wd.txtDebt.Text = null;
                wd.ShowDialog();
            }
        }

        private void SaveStore(AddTypeOfAgencyWindow para)
        {
            if (String.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
                para.txtName.Text = "";
                return;
            }
            if (String.IsNullOrEmpty(para.txtDebt.Text))
            {
                para.txtDebt.Focus();
                para.txtDebt.Text = "";
                return;
            }

            int id = int.Parse(para.txtID.Text);
            TypeOfAgency item = new TypeOfAgency();
            item.ID = id;
            item.Name = para.txtName.Text;
            item.MaxOfDebt = ConvertToNumber(para.txtDebt.Text);

            DataProvider.Instance.DB.TypeOfAgencies.AddOrUpdate(item);
            DataProvider.Instance.DB.SaveChanges();

            int count = this.HomeWindow.stkListType_Setting.Children.Count;

            for (int i = 0; i < count - 1; i++)
            {
                this.HomeWindow.stkListType_Setting.Children.RemoveAt(0);
            }

            para.Close();
            LoadSettingWindow(this.HomeWindow);
        }

        private void DeleteType(TypeOfAgencyUC para)
        {
            MessageBoxResult mes = CustomMessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo);

            if (mes != MessageBoxResult.Yes)
            {
                return;
            }

            List<Agency> list = DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).ToList();

            int stt = int.Parse(para.txbSTT.Text);

            foreach (Agency item in list)
            {
                if (item.TypeOfAgency == this.ListType[stt - 1].ID)
                {
                    CustomMessageBox.Show("You must delete all agencies of this type!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            TypeOfAgency type = this.ListType[stt - 1];
            DataProvider.Instance.DB.TypeOfAgencies.Remove(type);
            DataProvider.Instance.DB.SaveChanges();

            int count = this.HomeWindow.stkListType_Setting.Children.Count;

            for (int i = 0; i < count - 1; i++)
            {
                this.HomeWindow.stkListType_Setting.Children.RemoveAt(0);
            }
            LoadSettingWindow(this.HomeWindow);
        }

        private void EditType(TypeOfAgencyUC para)
        {
            int stt = int.Parse(para.txbSTT.Text);

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();

            AddTypeOfAgencyWindow wd = new AddTypeOfAgencyWindow();
            wd.txtID.Text = this.ListType[stt - 1].ID.ToString();
            wd.txtName.Text = this.ListType[stt - 1].Name;
            wd.txtDebt.Text = SeparateThousands(this.ListType[stt - 1].MaxOfDebt.ToString());

            wd.txtName.SelectionStart = wd.txtName.Text.Length;
            wd.txtDebt.SelectionStart = wd.txtDebt.Text.Length;

            wd.ShowDialog();
        }

        private void SaveRulesType_Setting(HomeWindow para)
        {
            int limit= LimitOfAgencyinDistrict();
            int count = DataProvider.Instance.DB.Settings.First().NumberStoreInDistrict;

            if (string.IsNullOrEmpty(para.txtNumberAgencyinDistrict_Setting.Text))
            {
                para.txtNumberAgencyinDistrict_Setting.Text = "";
                para.txtNumberAgencyinDistrict_Setting.Focus();
                return;
            }

            if (ConvertToNumber(para.txtNumberAgencyinDistrict_Setting.Text) == count)
            {
                CustomMessageBox.Show("Setting is not change...!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (limit > ConvertToNumber(para.txtNumberAgencyinDistrict_Setting.Text))
            {
                string show = string.Format("Number of agency in district must be greater than or equal to than {0}!", limit);
                CustomMessageBox.Show(show, "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Setting setting = DataProvider.Instance.DB.Settings.Where(x => x.ID == 1).First();
            setting.NumberStoreInDistrict = (int)ConvertToNumber(para.txtNumberAgencyinDistrict_Setting.Text);
            DataProvider.Instance.DB.Settings.AddOrUpdate(setting);
            DataProvider.Instance.DB.SaveChanges();
            CustomMessageBox.Show("Success", "Notify", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void LoadSettingWindow(HomeWindow para)
        {
            this.HomeWindow = para;
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();

            for (int i = 0; i < this.ListType.Count; i++)
            {
                TypeOfAgencyUC uc = new TypeOfAgencyUC();

                uc.txbSTT.Text = (i + 1).ToString();
                uc.txbName.Text = this.ListType[i].Name;
                uc.txbDebt.Text = SeparateThousands(this.ListType[i].MaxOfDebt.ToString());

                para.stkListType_Setting.Children.Add(uc);
            }


            int count = DataProvider.Instance.DB.Settings.First().NumberStoreInDistrict;
            para.txtNumberAgencyinDistrict_Setting.Text = ConvertToString(count);
            para.txtNumberAgencyinDistrict_Setting.SelectionStart = para.txtNumberAgencyinDistrict_Setting.Text.Length;

            Button bt = new Button();
            bt = (Button)para.stkListType_Setting.Children[0];
            para.stkListType_Setting.Children.RemoveAt(0);
            para.stkListType_Setting.Children.Add(bt);
        }

        public void Reload(HomeWindow para)
        {
            this.HomeWindow = para;

            int count = DataProvider.Instance.DB.Settings.First().NumberStoreInDistrict;
            para.txtNumberAgencyinDistrict_Setting.Text = ConvertToString(count);
            para.txtNumberAgencyinDistrict_Setting.SelectionStart = para.txtNumberAgencyinDistrict_Setting.Text.Length;
        }

        private int LimitOfAgencyinDistrict()
        {
            int max = 0;

            List<Agency> listAgency = DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).ToList();

            var results = DataProvider.Instance.DB.Agencies.Select(x => x.District).Distinct().ToList();

            foreach (string item in results)
            {
                int count = 0;
                foreach (Agency agency in listAgency)
                {
                    if (agency.District == item)
                        count++;
                }
                if (count > max)
                    max = count;
            }

            return max;
        }
    }
}
