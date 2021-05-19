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

        public StoreViewModel()
        {

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();
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
                item.txtSpecies.Text = agency.TypeOfAgency.ToString();
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
            Agency store= new Agency();
            int id = int.Parse(para.txtID.Text);
            store = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

            store.IsDelete = true;
            DataProvider.Instance.DB.Agencies.AddOrUpdate(store);
            DataProvider.Instance.DB.SaveChanges();

            this.HomeWindow.stkStore_Store.Children.Remove(para);
        }

        private void SaveStore(AddStoreWindow para)
        {
            StreamReader sr = new StreamReader("../../cache.txt");
            string cache = sr.ReadToEnd();
            sr.Close();

            string[] rulesSetting = cache.Split(' ');

            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtAddress.Text))
            {
                para.txtAddress.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtCheckin.Text))
            {
                para.txtCheckin.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtDebt.Text))
            {
                para.txtDebt.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtEmail.Text))
            {
                para.txtEmail.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtDistrict.Text))
            {
                para.txtDistrict.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtPhone.Text) || para.txtPhone.Text.Length != 10)
            {
                para.txtPhone.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtSpecies.Text))
            {
                para.txtSpecies.Focus();
                return;
            }

            if (DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).Where(x => x.District == para.txtDistrict.Text).ToList().Count <= 0)
            {
                var results = DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).Select(x => x.District).Distinct().ToList();
                if (results.Count >= 20)
                {
                    MessageBox.Show("Exceed the number of districts limit");
                    return;
                }
            }

            if (!CheckConditionNumberAgency(rulesSetting[1], para) && para.Title == "Thêm đại lý")
            {
                MessageBox.Show("Number of agency in this district is full");
                return;
            }

            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.txtSpecies.Text).First();

            if (type.MaxOfDebt < ConvertToNumber(para.txtDebt.Text))
            {
                MessageBox.Show("Nợ vượt quá cho phép.");
                return;
            }
            try
            {
                Agency item = new Agency();
                item.ID = int.Parse(para.txtID.Text);
                item.Name = para.txtName.Text;
                item.PhoneNumber = para.txtPhone.Text;
                item.Address = para.txtAddress.Text;
                item.District = para.txtDistrict.Text;
                item.Debt = ConvertToNumber(para.txtDebt.Text);
                item.CheckIn = DateTime.Parse(para.txtCheckin.Text);
                item.TypeOfAgency = type.ID;
                item.Email = para.txtEmail.Text;
                item.IsDelete = false;

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

            AddStoreWindow wd = new AddStoreWindow();
            wd.txtID.Text = store.ID.ToString();
            wd.txtName.Text = store.Name.ToString();
            wd.txtDistrict.Text = store.District.ToString();
            wd.txtAddress.Text = store.Address.ToString();
            wd.txtEmail.Text = store.Email.ToString();
            wd.txtCheckin.Text = store.CheckIn.ToString();
            wd.txtPhone.Text = store.PhoneNumber.ToString();

            for (int i = 0; i < wd.txtSpecies.Items.Count; i++)
            {
                wd.txtSpecies.SelectedIndex = i;
                if (wd.txtSpecies.Text.ToString() == type.Name)
                    pos = i;
            }

            wd.txtSpecies.SelectedIndex = pos;
            wd.txtDebt.Text = SeparateThousands(store.Debt.ToString());
            wd.Title = "Sửa thông tin đại lý";
            wd.txtDebt.IsEnabled = false;
            wd.txtCheckin.IsEnabled = false;
            wd.ShowDialog();
        }

        private void OpenAddStoreWindow()
        {
            AddStoreWindow wd = new AddStoreWindow();

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
                uc.Width = 280;
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
                uc.Width = 280;
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

        private bool CheckConditionNumberAgency(string rule, AddStoreWindow para)
        {
            int count = 0;
            string district = para.txtDistrict.Text;

            count = DataProvider.Instance.DB.Agencies.Where(x => x.District == district).Where(x => x.IsDelete == false).Count();

            if (count >= int.Parse(rule))
            {
                return false;
            }
            return true;
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
