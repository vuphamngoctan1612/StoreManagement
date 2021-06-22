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
            SaveRulesProduct_SettingCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SaveRulesProduct_Setting(para));
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

            string[] rules = this.cache.Split(' ');
            if (this.ListType.Count >= int.Parse(rules[0]))
            {
                MessageBox.Show("Over the limit in Setting");
                return;
            }
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

            for (int i = 0; i < count-1 ; i++)
            {
                this.HomeWindow.stkListType_Setting.Children.RemoveAt(0);
            }

            para.Close();
            LoadSettingWindow(this.HomeWindow);
        }

        private void DeleteType(TypeOfAgencyUC para)
        {
            MessageBoxResult mes = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo);

            if (mes != MessageBoxResult.Yes)
            {
                return;
            }

            List<Agency> list = DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).ToList();

            int stt = int.Parse(para.txbSTT.Text);

            foreach (Agency item in list)
            {
                if (item.TypeOfAgency == this.ListType[stt-1].ID)
                {
                    MessageBox.Show("You must delete all agencies of this type");
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
            wd.txtDebt.Text = SeparateThousands(this.ListType[stt-1].MaxOfDebt.ToString());

            wd.txtName.SelectionStart = wd.txtName.Text.Length;
            wd.txtDebt.SelectionStart = wd.txtDebt.Text.Length;

            wd.ShowDialog();
        }

        private void SaveRulesProduct_Setting(HomeWindow para)
        {
            this.HomeWindow = para;

            StreamReader sr = new StreamReader("../../cache.txt");
            this.cache = sr.ReadToEnd();
            sr.Close();

            string[] rulesSetting = this.cache.Split(' ');

            if (string.IsNullOrEmpty(para.txtNumberProduct_Setting.Text))
            {
                para.txtNumberType_Setting.Focus();
                return;
            }

            if (string.IsNullOrEmpty(para.txtNumberUnit_Setting.Text))
            {
                para.txtNumberAgencyinDistrict_Setting.Focus();
                return;
            }

            if (para.txtNumberProduct_Setting.Text == rulesSetting[2] && para.txtNumberUnit_Setting.Text == rulesSetting[3])
            {
                MessageBox.Show("Setting is not change...");
                return;
            }

            int countProduct = DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).Count();

            if (countProduct > int.Parse(para.txtNumberProduct_Setting.Text))
            {
                string show = string.Format("Number of product must be greater than or equal to than {0}", countProduct);
                MessageBox.Show(show);
                return;
            }

            int limit = LimitOfUnit();

            if (limit > int.Parse(para.txtNumberUnit_Setting.Text))
            {
                string show = string.Format("Number of unit must be greater than or equal to than {0}", limit);
                MessageBox.Show(show);
                return;
            }

            string newCache = string.Format("{0} {1} {2} {3}", rulesSetting[0], rulesSetting[1], para.txtNumberProduct_Setting.Text, para.txtNumberUnit_Setting.Text);

            string fileName = @"../../cache.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.AppendAllText(fileName, newCache);
            this.cache = newCache;

            MessageBox.Show("Success");
        }

        private void SaveRulesType_Setting(HomeWindow para)
        {
            this.HomeWindow = para;

            StreamReader sr = new StreamReader("../../cache.txt");
            this.cache = sr.ReadToEnd();
            sr.Close();

            string[] rulesSetting = this.cache.Split(' ');

            if (string.IsNullOrEmpty(para.txtNumberType_Setting.Text))
            {
                para.txtNumberType_Setting.Focus();
                return;
            }

            if (string.IsNullOrEmpty(para.txtNumberAgencyinDistrict_Setting.Text))
            {
                para.txtNumberAgencyinDistrict_Setting.Focus();
                return;
            }

            if (para.txtNumberType_Setting.Text == rulesSetting[0] && para.txtNumberAgencyinDistrict_Setting.Text == rulesSetting[1])
            {
                MessageBox.Show("Setting is not change...");
                return;
            }

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();

            if (this.ListType.Count > int.Parse(para.txtNumberType_Setting.Text))
            {
                string show = string.Format("Number of type must be greater than or equal to than {0}", this.ListType.Count);
                MessageBox.Show(show);
                return;
            }

            int limit = LimitOfAgencyinDistrict();

            if (limit > int.Parse(para.txtNumberAgencyinDistrict_Setting.Text))
            {
                string show = string.Format("Number of agency in district must be greater than or equal to than {0}", limit);
                MessageBox.Show(show);
                return;
            }

            string newCache = string.Format("{0} {1} {2} {3}", para.txtNumberType_Setting.Text, para.txtNumberAgencyinDistrict_Setting.Text, rulesSetting[2], rulesSetting[3]);

            string fileName = @"../../cache.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.AppendAllText(fileName, newCache);
            this.cache = newCache;

            MessageBox.Show("Success");
        }

        private void LoadSettingWindow(HomeWindow para)
        {
            this.HomeWindow = para;
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();

            StreamReader sr = new StreamReader("../../cache.txt");
            this.cache = sr.ReadToEnd();
            sr.Close();

            string[] rulesSetting = this.cache.Split(' ');

            para.txtNumberType_Setting.Text = rulesSetting[0];
            para.txtNumberAgencyinDistrict_Setting.Text = rulesSetting[1];
            para.txtNumberProduct_Setting.Text = rulesSetting[2];
            para.txtNumberUnit_Setting.Text = rulesSetting[3];

            para.txtNumberType_Setting.SelectionStart = para.txtNumberType_Setting.Text.Length;
            para.txtNumberAgencyinDistrict_Setting.SelectionStart = para.txtNumberAgencyinDistrict_Setting.Text.Length;
            para.txtNumberProduct_Setting.SelectionStart = para.txtNumberProduct_Setting.Text.Length;
            para.txtNumberUnit_Setting.SelectionStart = para.txtNumberUnit_Setting.Text.Length;

            for (int i = 0; i < this.ListType.Count; i++)
            {
                TypeOfAgencyUC uc = new TypeOfAgencyUC();

                uc.txbSTT.Text = (i + 1).ToString();
                uc.txbName.Text = this.ListType[i].Name;
                uc.txbDebt.Text = SeparateThousands(this.ListType[i].MaxOfDebt.ToString());

                para.stkListType_Setting.Children.Add(uc);
            }

            Button bt = new Button();
            bt = (Button)para.stkListType_Setting.Children[0];
            para.stkListType_Setting.Children.RemoveAt(0);
            para.stkListType_Setting.Children.Add(bt);
        }

        private String SeparateThousands(String txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                ulong valueBefore = ulong.Parse(ConvertToNumber(txt).ToString(), System.Globalization.NumberStyles.AllowThousands);
                txt = String.Format(culture, "{0:N0}", valueBefore);
            }
            return txt;
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

        private int LimitOfUnit()
        {
            int max = 0;

            List<Product> list = DataProvider.Instance.DB.Products.ToList();

            var results = DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).Select(x => x.Unit).Distinct().ToList();

            return results.Count;
        }
    }
}
