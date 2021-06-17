using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;

namespace StoreManagement.ViewModels
{
    public class StoreViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }
        public int PageNumber { get; set; }
        public List<TypeOfAgency> ListType
        {
            get; set;
        }
        public List<Agency> ListStores;
        public List<District> ListDistrict;
        public ICommand LoadStoreOnWindowCommand { get; set; }
        public ICommand NextPageStoresCommand { get; set; }
        public ICommand BackPageStoresCommand { get; set; }
        public ICommand OpenAddStoreWindowCommand { get; set; }
        public ICommand OpenEditStoreWindowCommand { get; set; }
        public ICommand SaveStoreCommand { get; set; }
        public ICommand DeleteAgencyCommand { get; set; }
        public ICommand CloseStoreWindowCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }
        public ICommand ChangeWayShowAgencyCommand { get; set; }
        public ICommand EditAgencyCommand { get; set; }
        public ICommand OpenAddDistrictCommand { get; set; }

        public StoreViewModel()
        {
            this.ListDistrict = DataProvider.Instance.DB.Districts.ToList();
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();
            PageNumber = 1;
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();
            LoadStoreOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Load3Stores(para, this.ListStores, PageNumber));
            NextPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) => {
                PageNumber = LoadNextPage(PageNumber);
                ReLoad3Stores(this.HomeWindow, this.ListStores, PageNumber);
            });
            BackPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) => {
                PageNumber = LoadBackPage(PageNumber);
                ReLoad3Stores(this.HomeWindow, this.ListStores, PageNumber);
            });
            OpenAddStoreWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddStoreWindow());
            OpenEditStoreWindowCommand = new RelayCommand<StoreControlUC>((para) => true, (para) => OpenEditStoreWindow(para.txbID.Text));
            CloseStoreWindowCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => para.Close());
            SaveStoreCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => SaveStore(para));
            DeleteAgencyCommand = new RelayCommand<AgencyControlUC>((para) => true, (para) => DeleteStore(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchAgency(para));
            ChangeWayShowAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ChangeWayShowAgency(para));
            EditAgencyCommand = new RelayCommand<TextBlock>((para) => true, (para) => OpenEditStoreWindow(para.Text));
            OpenAddDistrictCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => OpenAddDistrictWindow(para));
        }

        private void OpenAddDistrictWindow(AddStoreWindow para)
        {
            AddDistrictWindow window = new AddDistrictWindow();

            string query = "SELECT * FROM District";
            int fakeID = DataProvider.Instance.DB.Districts.SqlQuery(query).Count() + 1;

            window.txtID.Text = fakeID.ToString();
            window.ShowDialog();
            if (window.isSucceed)
            {
                LoadListDistrict(para);
            }
        }

        private void LoadListDistrict(AddStoreWindow para)
        {
            this.ListDistrict = DataProvider.Instance.DB.Districts.ToList();

            para.cbDistrict.ItemsSource = this.ListDistrict;
            para.cbDistrict.SelectedValuePath = "Name";
            para.cbDistrict.DisplayMemberPath = "Name";
        }

        private void ChangeWayShowAgency(HomeWindow para)
        {
            if (para.cbbStore_Store.SelectedIndex == 0)
            {
                LoadListAgency();
                para.grdListStore_Store.Visibility = Visibility.Visible;
                para.grdList3Store_Store.Visibility = Visibility.Hidden;
            }
            else
            {
                Load3Stores(this.HomeWindow, ListStores, PageNumber);
                para.grdListStore_Store.Visibility = Visibility.Hidden;
                para.grdList3Store_Store.Visibility = Visibility.Visible;
            }
        }

        private void LoadListAgency()
        {
            this.HomeWindow.stkStore_Store.Children.Clear();
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();

            foreach (Agency agency in ListStores)
            {
                AgencyControlUC item = new AgencyControlUC();

                item.txtID.Text = agency.ID.ToString();
                item.txtName.Text = agency.Name;
                item.txtPhone.Text = agency.PhoneNumber;
                item.cbbSpecies.Text = agency.TypeOfAgency.ToString();
                item.txtAddress.Text = agency.Address;
                item.txtDistrict.Text = agency.District;
                item.txtDebt.Text = SeparateThousands(agency.Debt.ToString());
                item.Height = 45;

                this.HomeWindow.stkStore_Store.Children.Add(item);
            }
        }

        private void SearchAgency(HomeWindow para)
        {
            if (para.grdListStore_Store.Visibility == Visibility.Visible)
            {
                foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                {
                    if (!control.txtName.Text.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text))
                    {
                        control.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                int loadPos = 0;
                int i = 0;
                int pos = 0;

                string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";

                this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();

                if (String.IsNullOrEmpty(this.HomeWindow.txtSearchAgency.Text))
                {
                    Load3Stores(para, this.ListStores, PageNumber);
                }
                else
                {
                    para.grdStoreFisrt.Children.Clear();
                    para.grdStoreSecond.Children.Clear();
                    para.grdStoreThird.Children.Clear();

                    //hiển thị ds theo phân trang(number page)
                    while (i < 3)
                    {
                        for (pos = loadPos; pos < this.ListStores.Count; pos++)
                        {
                            if (this.ListStores[pos].Name.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text.ToLower()))
                            {
                                i++;
                                loadPos = pos + 1;
                                int typeA = int.Parse(ListStores[pos].TypeOfAgency.ToString());
                                TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.ID == typeA).First();

                                StoreControlUC uc = new StoreControlUC();
                                uc.Height = 350;
                                uc.Width = 280;
                                uc.txbID.Text = ListStores[pos].ID.ToString();
                                uc.AgencyName.Text = ListStores[pos].Name.ToString();
                                uc.txbAgencyPhone.Text = ListStores[pos].PhoneNumber.ToString();
                                uc.txbAgencyDate.Text = ListStores[pos].CheckIn.Value.ToShortDateString();
                                uc.txbAgencyPosition.Text = ListStores[pos].Address.ToString();
                                uc.txbAgencyType.Text = type.Name.ToString();

                                switch (i - 1)
                                {
                                    case 0:
                                        para.grdStoreFisrt.Children.Add(uc);
                                        break;
                                    case 1:
                                        para.grdStoreSecond.Children.Add(uc);
                                        break;
                                    case 2:
                                        para.grdStoreThird.Children.Add(uc);
                                        break;
                                }
                            }
                        }
                        if (pos == this.ListStores.Count)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void DeleteStore(AgencyControlUC para)
        {
            Agency store = new Agency();
            int id = int.Parse(para.txtID.Text);
            store = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

            store.IsDelete = true;
            DataProvider.Instance.DB.Agencies.AddOrUpdate(store);
            DataProvider.Instance.DB.SaveChanges();

            District district = DataProvider.Instance.DB.Districts.Where(x => x.Name == store.District).First();
            district.NumberAgencyInDistrict -= 1;

            DataProvider.Instance.DB.Districts.AddOrUpdate(district);
            DataProvider.Instance.DB.SaveChanges();

            this.HomeWindow.stkStore_Store.Children.Remove(para);
        }

        private void SaveStore(AddStoreWindow para)
        {
            string oldDistrict = "";
            StreamReader sr = new StreamReader("../../cache.txt");
            string cache = sr.ReadToEnd();
            sr.Close();

            string district = "";

            string[] rulesSetting = cache.Split(' ');

            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
                para.txtName.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.cbbSpecies.Text))
            {
                para.cbbSpecies.Focus();
                para.cbbSpecies.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.cbDistrict.Text))
            {
                para.cbDistrict.Focus();
                para.cbDistrict.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtAddress.Text))
            {
                para.txtAddress.Focus();
                para.txtAddress.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtPhone.Text) || para.txtPhone.Text.Length != 10)
            {
                para.txtPhone.Focus();
                para.txtPhone.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtEmail.Text))
            {
                para.txtEmail.Focus();
                para.txtEmail.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.dpCheckin.Text))
            {
                para.dpCheckin.Focus();
                para.dpCheckin.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtDebt.Text))
            {
                para.txtDebt.Focus();
                para.txtDebt.Text = "";
                return;
            }

            district = para.cbDistrict.Text;
            int? number = DataProvider.Instance.DB.Districts.Where(x => x.Name == district).First().NumberAgencyInDistrict;

            if (para.Title == "Edit agency")
            {
                int id = int.Parse(para.txtID.Text.ToString());
                Agency agency = DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

                oldDistrict = agency.District;
                if (district != agency.District)
                {
                    if (int.Parse(rulesSetting[1]) <= number)
                    {
                        MessageBox.Show("Number of agency in this district is full");
                        return;
                    }
                }
            }
            else
            {
                if (int.Parse(rulesSetting[1]) <= number)
                {
                    MessageBox.Show("Number of agency in this district is full");
                    return;
                }
            }            

            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.cbbSpecies.Text).First();

            if (type.MaxOfDebt < ConvertToNumber(para.txtDebt.Text))
            {
                MessageBox.Show("Debt limit.");
                return;
            }
            try
            {
                Agency item = new Agency();
                item.ID = int.Parse(para.txtID.Text);
                item.Name = para.txtName.Text;
                item.PhoneNumber = para.txtPhone.Text;
                item.Address = para.txtAddress.Text;
                item.District = district;
                item.Debt = ConvertToNumber(para.txtDebt.Text);
                item.CheckIn = DateTime.Parse(para.dpCheckin.Text);
                item.TypeOfAgency = type.ID;
                item.Email = para.txtEmail.Text;
                item.IsDelete = false;

                if (para.Title == "Add agency")
                {
                    District updateDistrict = DataProvider.Instance.DB.Districts.Where(x => x.Name == district).First();
                    updateDistrict.NumberAgencyInDistrict += 1;

                    DataProvider.Instance.DB.Districts.AddOrUpdate(updateDistrict);
                    DataProvider.Instance.DB.SaveChanges();
                }
                else
                {
                    District updateDistrict = DataProvider.Instance.DB.Districts.Where(x => x.Name == oldDistrict).First();
                    updateDistrict.NumberAgencyInDistrict -= 1;
                    DataProvider.Instance.DB.Districts.AddOrUpdate(updateDistrict);
                    DataProvider.Instance.DB.SaveChanges();

                    updateDistrict = DataProvider.Instance.DB.Districts.Where(x => x.Name == district).First();
                    updateDistrict.NumberAgencyInDistrict += 1;
                    DataProvider.Instance.DB.Districts.AddOrUpdate(updateDistrict);
                    DataProvider.Instance.DB.SaveChanges();
                }    
                DataProvider.Instance.DB.Agencies.AddOrUpdate(item);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (this.HomeWindow.grdList3Store_Store.Visibility == Visibility.Visible)
                {
                    PageNumber = 1;
                    Load3Stores(this.HomeWindow, this.ListStores, PageNumber);
                }
                else
                {
                    LoadListAgency();
                }
                para.Close();
            }
        }

        private void OpenEditStoreWindow(String para)
        {
            Agency store = new Agency();
            int id = int.Parse(para);
            store = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();
            int pos = 0;
            int typeA = int.Parse(store.TypeOfAgency.ToString());
            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.ID == typeA).First();

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();
            AddStoreWindow wd = new AddStoreWindow();
            LoadListDistrict(wd);
            wd.txtID.Text = store.ID.ToString();
            wd.txtName.Text = store.Name.ToString();
            wd.txtAddress.Text = store.Address.ToString();
            wd.txtEmail.Text = store.Email.ToString();
            wd.dpCheckin.Text = store.CheckIn.ToString();
            wd.txtPhone.Text = store.PhoneNumber.ToString();
            wd.cbDistrict.Text = store.District.ToString();

            wd.txtName.SelectionStart = wd.txtName.Text.Length;
            wd.txtAddress.SelectionStart = wd.txtAddress.Text.Length;
            wd.txtPhone.SelectionStart = wd.txtPhone.Text.Length;
            wd.txtEmail.SelectionStart = wd.txtEmail.Text.Length;

            for (int i = 0; i < wd.cbbSpecies.Items.Count; i++)
            {
                wd.cbbSpecies.SelectedIndex = i;
                if (wd.cbbSpecies.Text.ToString() == type.Name)
                    pos = i;
            }

            wd.cbbSpecies.SelectedIndex = pos;
            wd.txtDebt.Text = SeparateThousands(store.Debt.ToString());
            wd.Title = "Edit agency";
            wd.txtDebt.IsEnabled = false;
            wd.dpCheckin.IsEnabled = false;
            wd.ShowDialog();
        }

        private void OpenAddStoreWindow()
        {
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();
            AddStoreWindow wd = new AddStoreWindow();
            LoadListDistrict(wd);
            try
            {
                string query = "SELECT * FROM Agency";

                Agency store = DataProvider.Instance.DB.Agencies.SqlQuery(query).Last();
                wd.txtID.Text = (store.ID + 1).ToString();
            }
            catch
            {
                wd.txtID.Text = "1";
            }
            finally
            {
                wd.ShowDialog();
            }
        }

        private void Load3Stores(HomeWindow para, List<Agency> listStores, int pageNumber)
        {
            this.HomeWindow = para;

            para.cbbStore_Store.SelectedIndex = 1;

            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";

            this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();

            //hiển thị ds theo phân trang(number page)
            for (int i = 0; i < 3; i++)
            {
                int pos = (pageNumber - 1) * 3 + i;

                if (pos == this.ListStores.Count)
                    break;

                int typeA = int.Parse(ListStores[pos].TypeOfAgency.ToString());
                TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.ID == typeA).First();

                StoreControlUC uc = new StoreControlUC();
                uc.Height = 350;
                uc.Width = 250;
                uc.txbID.Text = ListStores[pos].ID.ToString();
                uc.AgencyName.Text = ListStores[pos].Name.ToString();
                uc.txbAgencyPhone.Text = ListStores[pos].PhoneNumber.ToString();
                uc.txbAgencyDate.Text = ListStores[pos].CheckIn.Value.ToShortDateString();
                uc.txbAgencyPosition.Text = ListStores[pos].Address.ToString();
                uc.txbAgencyType.Text = type.Name.ToString();

                switch (i)
                {
                    case 0:
                        para.grdStoreFisrt.Children.Add(uc);
                        break;
                    case 1:
                        para.grdStoreSecond.Children.Add(uc);
                        break;
                    case 2:
                        para.grdStoreThird.Children.Add(uc);
                        break;
                }
            }
        }

        private int LoadBackPage(int pageNumber)
        {
            if (pageNumber > 1)
                return pageNumber - 1;
            else
                return pageNumber;
        }

        private void ReLoad3Stores(HomeWindow para, List<Agency> listStores, int pageNumber)
        {
            para.grdStoreFisrt.Children.Clear();
            para.grdStoreSecond.Children.Clear();
            para.grdStoreThird.Children.Clear();

            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";

            this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();

            //hiển thị ds theo phân trang(number page)
            for (int i = 0; i < 3; i++)
            {
                int pos = (pageNumber - 1) * 3 + i;

                if (pos == this.ListStores.Count)
                    break;

                int typeA = int.Parse(ListStores[pos].TypeOfAgency.ToString());
                TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.ID == typeA).First();

                StoreControlUC uc = new StoreControlUC();
                uc.Height = 350;
                uc.Width = 250;
                uc.txbID.Text = ListStores[pos].ID.ToString();
                uc.AgencyName.Text = ListStores[pos].Name.ToString();
                uc.txbAgencyPhone.Text = ListStores[pos].PhoneNumber.ToString();
                uc.txbAgencyDate.Text = ListStores[pos].CheckIn.Value.ToShortDateString();
                uc.txbAgencyPosition.Text = ListStores[pos].Address.ToString();
                uc.txbAgencyType.Text = type.Name.ToString();

                switch (i)
                {
                    case 0:
                        para.grdStoreFisrt.Children.Add(uc);
                        break;
                    case 1:
                        para.grdStoreSecond.Children.Add(uc);
                        break;
                    case 2:
                        para.grdStoreThird.Children.Add(uc);
                        break;
                }
            }
        }

        private int LoadNextPage(int pageNumber)
        {
            int countPage;

            if (ListStores.Count % 3 != 0)
            {
                countPage = (ListStores.Count / 3 + 1);
            }
            else
            {
                countPage = ListStores.Count / 3;
            }

            if (pageNumber < countPage)
                return pageNumber + 1;
            else
                return pageNumber;
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
    }
}