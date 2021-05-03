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
        public ICommand LoadStoreOnWindowCommand { get; set; }
        public ICommand NextPageStoresCommand { get; set; }
        public ICommand BackPageStoresCommand { get; set; }
        public ICommand OpenAddStoreWindowCommand { get; set; }
        public ICommand OpenEditStoreWindowCommand { get; set; }
        public ICommand SaveStoreCommand { get; set; }
        public ICommand DeleteStoreCommand { get; set; }
        public ICommand CloseStoreWindowCommand { get; set; }


        public StoreViewModel()
        {

            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList<TypeOfAgency>();
            PageNumber = 1;
            LoadStoreOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadStores(para, PageNumber));
            NextPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) => {
                PageNumber = LoadNextPage(PageNumber);
                ReLoadStores(this.HomeWindow, PageNumber);
            });
            BackPageStoresCommand = new RelayCommand<HomeWindow>((para) => true, (para) => {
                PageNumber = LoadBackPage(PageNumber);
                ReLoadStores(this.HomeWindow, PageNumber);
            });
            OpenAddStoreWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddStoreWindow());
            OpenEditStoreWindowCommand = new RelayCommand<StoreControlUC>((para) => true, (para) => OpenEditStoreWindow(para));
            CloseStoreWindowCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => para.Close());
            SaveStoreCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => SaveStore(para));
            DeleteStoreCommand = new RelayCommand<usStores>((para) => true, (para) => DeleteStore(para));
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

            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.txtSpecies.Text).First();

            if (type.MaxOfDebt < int.Parse(para.txtDebt.Text))
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
                item.Debt = int.Parse(para.txtDebt.Text);
                item.CheckIn = DateTime.Parse(para.txtCheckin.Text);
                item.TypeOfAgency = type.ID;
                item.Email = para.txtEmail.Text;

                DataProvider.Instance.DB.Agencies.AddOrUpdate(item);
                DataProvider.Instance.DB.SaveChanges();
            } catch (Exception ex)
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
            wd.txtCheckin.Text = store.CheckIn.ToString();
            wd.txtPhone.Text = store.PhoneNumber.ToString();
            //wd.txtSpecies.Text = store.TypeOfAgency.ToString();
            for (int i = 0; i < wd.txtSpecies.Items.Count; i++)
            {
                wd.txtSpecies.SelectedIndex = i;
                if (wd.txtSpecies.Text.ToString() == type.Name)
                    pos = i;
            }
            wd.txtSpecies.SelectedIndex = pos;
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
