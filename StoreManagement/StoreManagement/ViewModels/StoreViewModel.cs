using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        public ICommand DeleteStoreCommand { get; set; }
        public ICommand CloseStoreWindowCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }
        public ICommand ChangeWayShowAgencyCommand { get; set; }
        public ICommand EditAgencyCommand { get; set; }
        public ICommand ChangeWayChooseDistrictCommand { get; set; }

        public StoreViewModel()
        {
            this.ListDistrict = DataProvider.Instance.DB.Districts.ToList();
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();
            PageNumber = 1;
            LoadStoreOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadStores(para, PageNumber));
            NextPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) =>
            {
                PageNumber = LoadNextPage(PageNumber);
                ReLoadStores(this.HomeWindow, PageNumber);
            });
            BackPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) =>
            {
                PageNumber = LoadBackPage(PageNumber);
                ReLoadStores(this.HomeWindow, PageNumber);
            });
            OpenAddStoreWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddStoreWindow());
            OpenEditStoreWindowCommand = new RelayCommand<StoreControlUC>((para) => true, (para) => OpenEditStoreWindow(para));
            CloseStoreWindowCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => para.Close());
            SaveStoreCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => SaveStore(para));
            DeleteAgencyCommand = new RelayCommand<AgencyControlUC>((para) => true, (para) => DeleteStore(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchAgency(para));
            ChangeWayShowAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ChangeWayShowAgency(para));
            EditAgencyCommand = new RelayCommand<TextBlock>((para) => true, (para) => OpenEditStoreWindow(para.Text));
            ChangeWayChooseDistrictCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => ChangeWayChooseDistrict(para));
        }

        private void ChangeWayChooseDistrict(AddStoreWindow para)
        {
            if (para.cbDistrict.Visibility == Visibility.Visible)
            {
                para.cbDistrict.Visibility = Visibility.Hidden;
                para.txtDistrict.Visibility = Visibility.Visible;
            }
            else
            {
                para.cbDistrict.Visibility = Visibility.Visible;
                para.txtDistrict.Visibility = Visibility.Hidden;
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

        private void OpenWindowStore(CardStoreUC para)
        {
            int id = int.Parse(para.txbID.Text);
            Agency agency = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

            AddStoreWindow window = new AddStoreWindow();
            window.cbbSpecies.IsEditable = true;
            window.btnSave.IsEnabled = false;
            window.txtID.Text = agency.ID.ToString();

            window.txtName.Text = agency.Name;
            window.txtName.SelectionStart = window.txtName.Text.Length;

            window.cbbSpecies.Text = "Type " + agency.TypeOfAgency.ToString();

            window.txtDistrict.Text = agency.District;
            window.txtDistrict.SelectionStart = window.txtDistrict.Text.Length;

            window.txtAddress.Text = agency.Address;
            window.txtAddress.SelectionStart = window.txtAddress.Text.Length;

            window.txtPhone.Text = agency.PhoneNumber;
            window.txtPhone.SelectionStart = window.txtPhone.Text.Length;

            window.txtEmail.Text = agency.Email;
            window.txtEmail.SelectionStart = window.txtEmail.Text.Length;

            window.dtCheckin.Text = agency.CheckIn.ToString();

            window.txtDebt.Text = ConvertToString(agency.Debt);
            window.txtDebt.SelectionStart = window.txtDebt.Text.Length;

            window.ShowDialog();
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
                return;
            }
            if (string.IsNullOrEmpty(para.txtAddress.Text))
            {
                para.txtAddress.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.dtCheckin.Text))
            {
                para.dtCheckin.Focus();
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

            if (para.cbDistrict.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(para.cbDistrict.Text.ToString()))
                {
                    para.cbDistrict.Focus();
                    return;
                }
                else
                {
                    district = para.cbDistrict.Text.ToString();
                }
            }

            if (para.txtDistrict.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(para.txtDistrict.Text))
                {
                    para.txtDistrict.Focus();
                    return;
                }
                else
                {
                    district = para.txtDistrict.Text.ToString();
                    var result = DataProvider.Instance.DB.Districts.Where(x => x.Name == district).ToList();

                    if (result.Count < 1)
                    {
                        if (DataProvider.Instance.DB.Districts.ToList().Count < 20)
                        {
                            District newitem = new District();
                            newitem.Name = district;
                            newitem.NumberAgencyInDistrict = 0;
                            DataProvider.Instance.DB.Districts.Add(newitem);
                            DataProvider.Instance.DB.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Exceed the number of districts limit: 20");
                            return;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(para.txtPhone.Text) || para.txtPhone.Text.Length != 10)
            {
                para.txtPhone.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.cbbSpecies.Text))
            {
                para.cbbSpecies.Focus();
                return;
            }

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
            

            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.txtSpecies.Text).First();

            if (type.MaxOfDebt < ConvertToNumber(para.txtDebt.Text))
            {
                MessageBox.Show("Debit limit.");
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
                item.CheckIn = DateTime.Parse(para.txtCheckin.Text);
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
                LoadStores(this.HomeWindow, 1);
                para.Close();
            }
        }

        private void OpenEditStoreWindow(StoreControlUC para)
        {
            Agency store = new Agency();
            int id = int.Parse(para.txbID.Text);
            store = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();
            int pos =0;
            int typeA = int.Parse(store.TypeOfAgency.ToString());
            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.ID == typeA).First();

            AddStoreWindow wd = new AddStoreWindow();
            wd.txtID.Text = store.ID.ToString();
            wd.txtName.Text = store.Name.ToString();
            wd.txtDistrict.Text = store.District.ToString();
            wd.txtAddress.Text = store.Address.ToString();
            wd.txtEmail.Text = store.Email.ToString();
            wd.dtCheckin.Text = store.CheckIn.ToString();
            wd.txtPhone.Text = store.PhoneNumber.ToString();
            wd.cbDistrict.Visibility = Visibility.Hidden;
            wd.txtDistrict.Visibility = Visibility.Visible;

            wd.txtName.SelectionStart = wd.txtName.Text.Length;
            wd.txtAddress.SelectionStart = wd.txtAddress.Text.Length;
            wd.txtPhone.SelectionStart = wd.txtPhone.Text.Length;
            wd.txtEmail.SelectionStart = wd.txtEmail.Text.Length;
            wd.txtDistrict.SelectionStart = wd.txtDistrict.Text.Length;

            for (int i = 0; i < wd.txtSpecies.Items.Count; i++)
            {
                wd.cbbSpecies.SelectedIndex = i;
                if (wd.cbbSpecies.Text.ToString() == type.Name)
                    pos = i;
            }

            wd.txtSpecies.SelectedIndex = pos;
            wd.txtDebt.Text = SeparateThousands(store.Debt.ToString());
            wd.Title = "Edit agency";
            wd.txtDebt.IsEnabled = false;
            wd.txtCheckin.IsEnabled = false;
            wd.ShowDialog();
        }

        private void OpenAddStoreWindow()
        {
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

        private void LoadStores(HomeWindow para, int pageNumber)
        {
            this.HomeWindow = para;
            //lấy danh ds cửa hàng
            List<Agency> ListStores = DataProvider.Instance.DB.Agencies.ToList<Agency>();

            //hiển thị ds theo phân trang(number page)
            for (int i = 0; i < 3; i++)
            {
                int pos = (pageNumber - 1) * 3 + i;

                if (pos == DataProvider.Instance.DB.Agencies.Count())
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

        private void ReLoadStores(HomeWindow para, int pageNumber)
        {
            //lấy danh ds cửa hàng
            List<Agency> ListStores = DataProvider.Instance.DB.Agencies.ToList<Agency>();

            para.grdStoreFisrt.Children.Clear();
            para.grdStoreSecond.Children.Clear();
            para.grdStoreThird.Children.Clear();

            //hiển thị ds theo phân trang(number page)
            for (int i = 0; i < 3; i++)
            {
                int pos = (pageNumber - 1) * 3 + i;

                if (pos == DataProvider.Instance.DB.Agencies.Count())
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
            List<Agency> ListStores = DataProvider.Instance.DB.Agencies.ToList<Agency>();
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
