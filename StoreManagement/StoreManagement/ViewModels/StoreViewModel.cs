using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FastMember;
using Microsoft.Win32;
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
        public ICommand ExportExcelStoreCommand { get; set; }
        public ICommand CheckDateCommand { get; set; }
        public ICommand SearchByCommand { get; set; }
        public ICommand SearchDebtAgencyCommand { get; set; }

        public StoreViewModel()
        {
            this.ListDistrict = DataProvider.Instance.DB.Districts.ToList();
            this.ListType = DataProvider.Instance.DB.TypeOfAgencies.ToList();
            PageNumber = 1;
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListStores = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList();
            LoadStoreOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) =>
            {
                para.cbbStore_Store.SelectedIndex = 0;
                para.cbbSearch.SelectedIndex = 0;
                ChangeWayShowAgency(para);
            });
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
            ExportExcelStoreCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ExportExcelStore(para));
            CheckDateCommand = new RelayCommand<AddStoreWindow>((para) => true, (para) => CheckDate(para));
            SearchByCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchByAgency(para));
            SearchDebtAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchDebtAgency(para));
        }

        private void SearchDebtAgency(HomeWindow para)
        {
            switch (para.cbbSearch.SelectedIndex)
            {
                case 3:
                    if (para.txtSearchDebtAgency.Text != "")
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            long debt = ConvertToNumber(control.txtDebt.Text);
                            if (debt < ConvertToNumber(this.HomeWindow.txtSearchDebtAgency.Text))
                            {
                                control.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                control.Visibility = Visibility.Visible;
                            }
                        }
                    else
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            control.Visibility = Visibility.Visible;
                        }
                    break;
                case 4:
                    if (para.txtSearchDebtAgency.Text != "")
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            long debt = ConvertToNumber(control.txtDebt.Text);
                            if (debt > ConvertToNumber(this.HomeWindow.txtSearchDebtAgency.Text))
                            {
                                control.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                control.Visibility = Visibility.Visible;
                            }
                        }
                    else
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            control.Visibility = Visibility.Visible;
                        }
                    break;
            }
        }

        private void SearchByAgency(HomeWindow para)
        {
            para.txtSearchAgency.Clear();
            para.txtSearchDebtAgency.Clear();

            foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
            {
                control.Visibility = Visibility.Visible;
            }
            switch (para.cbbSearch.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    para.txtSearchAgency.Visibility = Visibility.Visible;
                    para.txtSearchDebtAgency.Visibility = Visibility.Hidden;
                    break;
                case 3:
                case 4:
                    para.txtSearchAgency.Visibility = Visibility.Hidden;
                    para.txtSearchDebtAgency.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void CheckDate(AddStoreWindow para)
        {
            if (DateTime.Compare(DateTime.Now, (DateTime)para.dpCheckin.SelectedDate) < 0)
            {
                CustomMessageBox.Show("Could not select the month at the future!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                para.dpCheckin.Text = DateTime.Now.ToString();
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private void ExportExcelStore(HomeWindow para)
        {
            this.ListStores = DataProvider.Instance.DB.Agencies.Where(x => x.IsDelete == false).ToList();

            if (this.ListStores.Count == 0)
            {
                CustomMessageBox.Show("List agency is empty!", "Notify", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                object misValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                application.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                Microsoft.Office.Interop.Excel.Range cellRange;
                DataTable data = new DataTable();

                //using (var reader = ObjectReader.Create(this.ListStores))
                //{
                //    data.Load(reader);
                //}

                data = ToDataTable<Agency>(this.ListStores);
                data.Columns.Remove("district1");
                data.Columns.Remove("isdelete");
                data.Columns.Remove("invoices");
                data.Columns.Remove("typeofagency1");
                data.Columns.Remove("receipts");

                worksheet = application.Worksheets.Add(misValue, misValue, misValue, misValue);
                worksheet.Name = "Agency";
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = data.Columns[i].ColumnName;
                }
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = data.Rows[i][j].ToString();
                    }
                }
                cellRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[data.Rows.Count + 1, data.Columns.Count]];
                cellRange.EntireColumn.AutoFit();
                workbook.SaveAs(sfd.FileName);
                workbook.Close();
                application.Quit();
            }
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

        public void ChangeWayShowAgency(HomeWindow para)
        {
            if (para.cbbStore_Store.SelectedIndex == 0)
            {
                LoadListAgency(para);
                para.grdListStore_Store.Visibility = Visibility.Visible;
                para.grdList3Store_Store.Visibility = Visibility.Hidden;
                if (para.cbbSearch.Items.Count == 3)
                {
                    para.cbbSearch.Items.Add("Debt - Upper");
                    para.cbbSearch.Items.Add("Debt - Lower");
                }
                para.cbbSearch.SelectedIndex = 0;
            }
            else
            {
                Load3Stores(this.HomeWindow, PageNumber);
                para.grdListStore_Store.Visibility = Visibility.Hidden;
                para.grdList3Store_Store.Visibility = Visibility.Visible;
                if (para.cbbSearch.Items.Count == 5)
                {
                    para.cbbSearch.Items.Remove("Debt - Upper");
                    para.cbbSearch.Items.Remove("Debt - Lower");
                }
                para.cbbSearch.SelectedIndex = 0;
            }
        }

        private void LoadListAgency(HomeWindow para)
        {
            this.HomeWindow = para;
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
            switch (para.cbbSearch.SelectedIndex)
            {
                case 0:
                    if (para.cbbStore_Store.SelectedIndex == 0)
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
                            Load3Stores(para, PageNumber);
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
                                        uc.Width = 250;
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
                    break;
                case 1:
                    if (para.cbbStore_Store.SelectedIndex == 0)
                    {
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            if (!control.cbbSpecies.Text.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text))
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
                            Load3Stores(para, PageNumber);
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
                                    if (this.ListStores[pos].TypeOfAgency.ToString().ToLower().Contains(this.HomeWindow.txtSearchAgency.Text.ToLower()))
                                    {
                                        i++;
                                        loadPos = pos + 1;
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
                    break;
                case 2:
                    if (para.cbbStore_Store.SelectedIndex == 0)
                    {
                        foreach (AgencyControlUC control in HomeWindow.stkStore_Store.Children)
                        {
                            if (!control.txtDistrict.Text.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text))
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
                            Load3Stores(para, PageNumber);
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
                                    if (this.ListStores[pos].District.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text.ToLower()))
                                    {
                                        i++;
                                        loadPos = pos + 1;
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
                    break;

            }
        }

        private void DeleteStore(AgencyControlUC para)
        {
            if (ConvertToNumber(para.txtDebt.Text) > 0)
            {
                CustomMessageBox.Show("Debt must be equal 0!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult res = CustomMessageBox.Show("Are you sure?", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
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
        }

        private void SaveStore(AddStoreWindow para)
        {
            string oldDistrict = "";

            string district = "";

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
            int setting = DataProvider.Instance.DB.Settings.First().NumberStoreInDistrict;

            if (para.Title == "Edit agency")
            {
                int id = int.Parse(para.txtID.Text.ToString());
                Agency agency = DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

                oldDistrict = agency.District;
                if (district != agency.District)
                {
                    if (setting <= number)
                    {
                        CustomMessageBox.Show("Number of agency in this district is full!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            else
            {
                if (setting <= number)
                {
                    CustomMessageBox.Show("Number of agency in this district is full!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            TypeOfAgency type = (TypeOfAgency)DataProvider.Instance.DB.TypeOfAgencies.Where(x => x.Name == para.cbbSpecies.Text).First();

            if (type.MaxOfDebt < ConvertToNumber(para.txtDebt.Text))
            {
                CustomMessageBox.Show("Debt limit!", "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CustomMessageBox.Show(ex.Message, "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.HomeWindow.grdList3Store_Store.Visibility == Visibility.Visible)
                {
                    PageNumber = 1;
                    Load3Stores(this.HomeWindow, PageNumber);
                }
                else
                {
                    LoadListAgency(this.HomeWindow);
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
            wd.grdDebt.Visibility = Visibility.Visible;
            wd.grdEdit.Visibility = Visibility.Visible;
            wd.grdSave.Visibility = Visibility.Hidden;
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
                wd.txtDebt.Text = "0";
                wd.ShowDialog();
            }
        }

        public void Load3Stores(HomeWindow para, int pageNumber)
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

        //private String SeparateThousands(String txt)
        //{
        //    if (!string.IsNullOrEmpty(txt))
        //    {
        //        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        //        ulong valueBefore = ulong.Parse(ConvertToNumber(txt).ToString(), System.Globalization.NumberStyles.AllowThousands);
        //        txt = String.Format(culture, "{0:N0}", valueBefore);
        //    }
        //    return txt;
        //}
    }
}