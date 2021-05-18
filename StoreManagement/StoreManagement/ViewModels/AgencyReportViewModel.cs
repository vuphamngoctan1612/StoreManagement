using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace StoreManagement.ViewModels
{
    class AgencyReportViewModel : BaseViewModel
    {
        private string uid;
        public HomeWindow HomeWindow  { get; set; }
        public ICommand LoadSalesReportCommand { get; set; }
        public ICommand LoadDebtsReportCommand { get; set; }
        public ICommand SwitchCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }
        public string CustomFormat { get; set; }

        public AgencyReportViewModel()
        {
            LoadSalesReportCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadSalesReport(para));
            LoadDebtsReportCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadDebtsReport(para));
            GetUidCommand = new RelayCommand<Button>((para) => true, (para) => uid = para.Uid);
            SwitchCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Switch(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Search(para));
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
            int index = int.Parse(uid);
            switch (index)
            {
                case 1:
                    para.textSalesReport.Visibility = System.Windows.Visibility.Visible;
                    para.textDebtReport.Visibility = System.Windows.Visibility.Hidden;
                    para.cardTitleSalesReport.Visibility = System.Windows.Visibility.Visible;
                    para.cardSalesReport.Visibility = System.Windows.Visibility.Visible;
                    para.cardDebtAgencyReport.Visibility = System.Windows.Visibility.Hidden;
                    para.scrollSales.Visibility = System.Windows.Visibility.Visible;
                    para.scrollDebt.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case 2:
                    para.textSalesReport.Visibility = System.Windows.Visibility.Hidden;
                    para.textDebtReport.Visibility = System.Windows.Visibility.Visible;
                    para.cardTitleSalesReport.Visibility = System.Windows.Visibility.Visible;
                    para.cardSalesReport.Visibility = System.Windows.Visibility.Hidden;
                    para.cardDebtAgencyReport.Visibility = System.Windows.Visibility.Visible;
                    para.scrollSales.Visibility = System.Windows.Visibility.Hidden;
                    para.scrollDebt.Visibility = System.Windows.Visibility.Visible;
                    break;
            } 
                
        }
        private void LoadDebtsReport(HomeWindow para)
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
                DebtReportUC debtReportUC = new DebtReportUC();
                debtReportUC.txtNo.Text = agency.ID.ToString();
                debtReportUC.txtAgency.Text = agency.Name;
                invoices = agency.Invoices.ToList();
                foreach (Invoice invoice in invoices)
                {
                    if (invoice.Checkout.Value.Month == para.Date.SelectedDate.Value.Month && invoice.Checkout.Value.Year == para.Date.SelectedDate.Value.Year)
                    {
                        dept += invoice.Debt;
                        count++;
                    }
                }
                check++;
                debtReportUC.txtOriginalDebt.Text = invoices.First().Debt.ToString();
                debtReportUC.txtCostOverrun.Text = (dept - invoices.First().Debt).ToString();
                debtReportUC.txtTotal.Text = dept.ToString();

                this.HomeWindow.stkDebtReport.Children.Add(debtReportUC);
            }
        }
        private void LoadSalesReport(HomeWindow para)
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
                List<Invoice> invoices = new List<Invoice>();
                SalesReportUC salesReportUC = new SalesReportUC();
                salesReportUC.txtNo.Text = agency.ID.ToString();
                salesReportUC.txtAgency.Text = agency.Name;
                invoices = agency.Invoices.ToList();
                foreach (Invoice invoice in invoices)
                {
                    if (invoice.Checkout.Value.Month == para.Date.SelectedDate.Value.Month && invoice.Checkout.Value.Year == para.Date.SelectedDate.Value.Year)
                    {
                        total += invoice.Total;
                        dept += invoice.Debt;
                        count++;
                    }
                }
                salesReportUC.txtNumberOfBills.Text = count.ToString();
                salesReportUC.txtTotal.Text = total.ToString();
                salesReportUC.txtRatio.Text = (100 * (double)dept / (double)(total + 1)).ToString() + "%";
                this.HomeWindow.stkSalesReport.Children.Add(salesReportUC);
                check++;
            }   
        }
    }


}
