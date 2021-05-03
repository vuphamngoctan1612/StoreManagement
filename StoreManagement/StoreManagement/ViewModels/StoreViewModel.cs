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
        public List<TypeOfAgency> ListType { get; set; }

        public ICommand LoadStoreOnWindowCommand { get; set; }
        public ICommand NextPageStoresCommand { get; set; }
        public ICommand BackPageStoresCommand { get; set; }
        public ICommand OpenAddStoreWindowCommand { get; set; }
        public ICommand OpenEditStoreWindowCommand { get; set; }
        public ICommand SaveStoreCommand { get; set; }
        public ICommand DeleteStoreCommand { get; set; }
        public ICommand CloseStoreWindowCommand { get; set; }
        public ICommand OpenWindowStoreCommand { get; set; }


        public StoreViewModel()
        {

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();
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
            DeleteStoreCommand = new RelayCommand<usStores>((para) => true, (para) => DeleteStore(para));
            OpenWindowStoreCommand = new RelayCommand<CardStoreUC>((para) => true, (para) => OpenWindowStore(para));
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

        private void DeleteStore(usStores para)
        {
            throw new NotImplementedException();
        }

        private void SaveStore(AddStoreWindow para)
        {
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
            if (string.IsNullOrEmpty(para.cbbSpecies.Text))
            {
                para.cbbSpecies.Focus();
                return;
            }

            TypeOfAgency typeOfAgency = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.cbbSpecies.Text).First();

            if (typeOfAgency.MaxOfDebt < int.Parse(para.txtDebt.Text))
            {
                MessageBox.Show("Nợ vượt quá cho phép.");
                return;
            }
            try
            {
                Agency agency = new Agency();
                agency.ID = int.Parse(para.txtID.Text);
                agency.Name = para.txtName.Text;
                agency.PhoneNumber = para.txtPhone.Text;
                agency.Address = para.txtAddress.Text;
                agency.District = para.txtDistrict.Text;
                agency.Debt = ConvertToNumber(para.txtDebt.Text);
                agency.CheckIn = DateTime.Parse(para.dtCheckin.Text);
                agency.TypeOfAgency = typeOfAgency.ID;
                agency.Email = para.txtEmail.Text;

                DataProvider.Instance.DB.Agencies.AddOrUpdate(agency);
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
            //wd.txtSpecies.Text = store.TypeOfAgency.ToString();
            for (int i = 0; i < wd.cbbSpecies.Items.Count; i++)
            {
                wd.cbbSpecies.SelectedIndex = i;
                if (wd.cbbSpecies.Text.ToString() == type.Name)
                    pos = i;
            }
            wd.cbbSpecies.SelectedIndex = pos;
            wd.txtDebt.Text = store.Debt.ToString();
            wd.Title = "Sửa thông tin đại lý";
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
    }
}
