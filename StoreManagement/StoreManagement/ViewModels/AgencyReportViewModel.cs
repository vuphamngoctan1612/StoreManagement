using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System;

namespace StoreManagement.ViewModels
{
    class AgencyReportViewModel : BaseViewModel
    {
        private string uid;
        public HomeWindow HomeWindow { get; set; }
        public ICommand LoadSalesReportCommand { get; set; }
        public ICommand LoadDebtsReportCommand { get; set; }
        public ICommand SwitchCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }
        public ICommand InitCommand { get; set; }
        public ICommand CheckDateCommand { get; set; }
        public string CustomFormat { get; set; }

        public AgencyReportViewModel()
        {
            LoadSalesReportCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadSalesReport(para));
            LoadDebtsReportCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadDebtsReport(para));
            GetUidCommand = new RelayCommand<Button>((para) => true, (para) => uid = para.Uid);
            SwitchCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Switch(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Search(para));
            InitCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Init(para));
            CheckDateCommand = new RelayCommand<HomeWindow>((para) => true, (para) => CheckDate(para));
        }

        private void CheckDate(HomeWindow para)
        {
            if (DateTime.Compare(DateTime.Now, (DateTime)para.Date.SelectedDate) < 0)
            {
                CustomMessageBox.Show("Could not select the month at the future!", "Notify", MessageBoxButton.OK, MessageBoxImage.Warning);
                para.Date.Text = DateTime.Now.ToString();
            }    
        }

        public void Init(HomeWindow para)
        {
            para.Date.Text = DateTime.Now.ToString();
            para.comboBoxReport.SelectedIndex = 0;
            para.cardSalesReport.Visibility = Visibility.Visible;
            List<Agency> agencies = DataProvider.Instance.DB.Agencies.ToList<Agency>();
            para.stkSalesReport.Children.Clear();
            int check = 0;
            int checkDebt = 0;
            foreach (Agency agency in agencies)
            {
                long? total = 0;
                long? dept = 0;
                int count = 0;
                double totalDebtThisMonth = GetTotalThisMonth(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
                List<Invoice> invoices = new List<Invoice>();
                SalesReportUC salesReportUC = new SalesReportUC();
                salesReportUC.Width = 1070;
                salesReportUC.Height = 45;
                salesReportUC.txtNo.Text = agency.ID.ToString();
                salesReportUC.txtAgency.Text = agency.Name;
                invoices = agency.Invoices.ToList();
                foreach (Invoice invoice in invoices)
                {
                    try
                    {
                        if (invoice.Checkout.Value.Month == DateTime.Now.Month && invoice.Checkout.Value.Year == DateTime.Now.Year)
                        {
                            total += invoice.Total;
                            dept += invoice.Debt;
                            count++;
                        }
                    }
                    catch { }
                }
                salesReportUC.txtNumberOfBills.Text = count.ToString();
                salesReportUC.txtTotal.Text = ConvertToString(total);
                salesReportUC.txtRatio.Text = (100 * (double)total / (double)(totalDebtThisMonth + 1)).ToString("0.00") + "%";
                para.stkSalesReport.Children.Add(salesReportUC);
                para.scrollSales.Visibility = Visibility.Visible;
                check++;
                //
                long? deptDebt = 0;
                int countDebt = 0;
                List<Invoice> invoicesDebt = new List<Invoice>();
                DebtReportUC debtReportUC = new DebtReportUC();
                List<Invoice> invoiceDebtAfter = new List<Invoice>();
                debtReportUC.Height = 45;
                debtReportUC.Width = 1070;
                debtReportUC.txtNo.Text = agency.ID.ToString();
                debtReportUC.txtAgency.Text = agency.Name;
                invoicesDebt = DataProvider.Instance.DB.Invoices.Where(x => x.AgencyID == agency.ID).ToList();
                foreach (Invoice invoice in invoicesDebt)
                {
                    try
                    {
                        if (invoice.Checkout.Value.Month == DateTime.Now.Month && invoice.Checkout.Value.Year == DateTime.Now.Year)
                        {
                            deptDebt += invoice.Debt;
                            countDebt++;
                            invoiceDebtAfter.Add(invoice);
                        }
                    }
                    catch { }
                }
                checkDebt++;
                if (invoiceDebtAfter.Count != 0)
                {
                    debtReportUC.txtOriginalDebt.Text = ConvertToString(invoiceDebtAfter.First().Debt);
                    debtReportUC.txtCostOverrun.Text = ConvertToString(deptDebt - invoiceDebtAfter.First().Debt);
                }
                else
                {
                    debtReportUC.txtOriginalDebt.Text = "0";
                    debtReportUC.txtCostOverrun.Text = "0";
                }
                debtReportUC.txtTotal.Text = ConvertToString(dept);

                para.stkDebtReport.Children.Add(debtReportUC);
            }
        }

        private void Search(HomeWindow para)
        {
            this.HomeWindow = para;
            foreach (SalesReportUC control in this.HomeWindow.stkSalesReport.Children)
            {
                if (!control.txtAgency.Text.ToLower().Contains(this.HomeWindow.txtSearchReport.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
            foreach (DebtReportUC control in this.HomeWindow.stkDebtReport.Children)
            {
                if (!control.txtAgency.Text.ToLower().Contains(this.HomeWindow.txtSearchReport.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
        }
        private void Switch(HomeWindow para)
        {
            if (para.comboBoxReport.SelectedIndex == 0)
            {
                para.cardTitleSalesReport.Visibility = System.Windows.Visibility.Visible;
                para.cardSalesReport.Visibility = System.Windows.Visibility.Visible;
                para.cardDebtAgencyReport.Visibility = System.Windows.Visibility.Hidden;
                para.scrollSales.Visibility = System.Windows.Visibility.Visible;
                para.scrollDebt.Visibility = System.Windows.Visibility.Hidden;
            }
            if (para.comboBoxReport.SelectedIndex == 1)
            {

                para.cardTitleSalesReport.Visibility = System.Windows.Visibility.Visible;
                para.cardSalesReport.Visibility = System.Windows.Visibility.Hidden;
                para.cardDebtAgencyReport.Visibility = System.Windows.Visibility.Visible;
                para.scrollSales.Visibility = System.Windows.Visibility.Hidden;
                para.scrollDebt.Visibility = System.Windows.Visibility.Visible;
            }

        }
        public void LoadDebtsReport(HomeWindow para)
        {
            this.HomeWindow = para;
            List<Agency> agencies = DataProvider.Instance.DB.Agencies.ToList<Agency>();
            this.HomeWindow.stkDebtReport.Children.Clear();
            int check = 0;
            foreach (Agency agency in agencies)
            {
                long? dept = 0;
                int count = 0;
                List<Invoice> invoices = new List<Invoice>();
                List<Invoice> invoicesAfter = new List<Invoice>();
                DebtReportUC debtReportUC = new DebtReportUC();
                debtReportUC.Height = 45;
                debtReportUC.Width = 1070;
                debtReportUC.txtNo.Text = agency.ID.ToString();
                debtReportUC.txtAgency.Text = agency.Name;
                invoices = DataProvider.Instance.DB.Invoices.Where(x => x.AgencyID == agency.ID).ToList();
                
                foreach (Invoice invoice in invoices)
                {
                    try
                    {
                        if (invoice.Checkout.Value.Month == para.Date.SelectedDate.Value.Month && invoice.Checkout.Value.Year == para.Date.SelectedDate.Value.Year)
                        {
                            dept += invoice.Debt;
                            count++;
                            invoicesAfter.Add(invoice);
                        }
                    }
                    catch { }
                }
                check++;
                if (invoicesAfter.Count != 0)
                {
                    debtReportUC.txtOriginalDebt.Text = ConvertToString( invoicesAfter.First().Debt);
                    debtReportUC.txtCostOverrun.Text = ConvertToString(dept - invoicesAfter.First().Debt);
                }
                else
                {
                    debtReportUC.txtOriginalDebt.Text = "0";
                    debtReportUC.txtCostOverrun.Text = "0";
                }
                debtReportUC.txtTotal.Text = ConvertToString(dept);

                this.HomeWindow.stkDebtReport.Children.Add(debtReportUC);
            }
        }
        public void LoadSalesReport(HomeWindow para)
        {
            this.HomeWindow = para;
            List<Agency> agencies = DataProvider.Instance.DB.Agencies.ToList<Agency>();
            this.HomeWindow.stkSalesReport.Children.Clear();
            int check = 0;
            foreach (Agency agency in agencies)
            {
                long? total = 0;
                long? dept = 0;
                int count = 0;
                double totalDebtThisMonth = GetTotalThisMonth(para.Date.SelectedDate.Value.Month.ToString(), para.Date.SelectedDate.Value.Year.ToString());
                List<Invoice> invoices = new List<Invoice>();
                SalesReportUC salesReportUC = new SalesReportUC();
                salesReportUC.Width = 1070;
                salesReportUC.Height = 45;
                salesReportUC.txtNo.Text = agency.ID.ToString();
                salesReportUC.txtAgency.Text = agency.Name;
                invoices = agency.Invoices.ToList();
                foreach (Invoice invoice in invoices)
                {
                    try
                    {
                        if (invoice.Checkout.Value.Month == para.Date.SelectedDate.Value.Month && invoice.Checkout.Value.Year == para.Date.SelectedDate.Value.Year)
                        {
                            total += invoice.Total;
                            dept += invoice.Debt;
                            count++;
                        }
                    }
                    catch { }
                }
                salesReportUC.txtNumberOfBills.Text = count.ToString();
                salesReportUC.txtTotal.Text = ConvertToString(total);
                salesReportUC.txtRatio.Text = (100 * (double)total / (double)(totalDebtThisMonth + 1)).ToString("0.00") + "%";
                this.HomeWindow.stkSalesReport.Children.Add(salesReportUC);
                check++;
            }
        }

        private double GetTotalThisMonth(string month, string year)
        {
            
            double total = 0;
            try
            {
                string query1 = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} ", month, year);
                Int64 temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query1).ToList().First();
                total = (double)temp;
                return total;
            }
            catch
            {
                return total;
            }
        }
    }


}
