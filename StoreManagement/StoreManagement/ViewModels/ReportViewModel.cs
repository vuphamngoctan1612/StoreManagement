using LiveCharts;
using LiveCharts.Wpf;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StoreManagement.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }

        private ObservableCollection<string> itemSourceTime = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceTime 
        { 
            get => itemSourceTime; 
            set
            { 
                itemSourceTime = value;
                OnPropertyChanged(); 
            }
        }

        private string axisXTitle;
        public string AxisXTitle
        {
            get => axisXTitle;
            set 
            {
                axisXTitle = value;
                OnPropertyChanged();
            } 
        }

        private string axisYTitle;
        public string AxisYTitle
        {
            get => axisYTitle;
            set
            {
                axisYTitle = value;
                OnPropertyChanged();
            }
        }

        private string[] labels;
        public string[] Labels
        {
            get => labels;
            set
            {
                labels = value;
                OnPropertyChanged();
            }
        }

        private Func<double, string> formatter;
        public Func<double, string> Formatter 
        { 
            get => formatter;
            set 
            {
                formatter = value; 
                OnPropertyChanged();
            } 
        }

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set
            {
                seriesCollection = value;
                OnPropertyChanged();
            }
        }

        public ICommand InitColumnChartCommand { get; set; }
        public ICommand SelectedTypeChangeCommand { get; set; }
        public ICommand LoadSalesResult { get; set; }

        public ReportViewModel()
        {
            //LoadTop3AgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadTop3Agency(para));
            InitColumnChartCommand = new RelayCommand<HomeWindow>((para) => true, (para) => InitColumnChart(para));
            SelectedTypeChangeCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cboSelectTypeOfChartIndex_Changed(para));
            LoadSalesResult = new RelayCommand<HomeWindow>((para) => true, (para) => LoadSales(para));
        }

        public void LoadSales(HomeWindow para)
        {
            double sumInvoicesTotal = 0;
            int countInvoices = 0;
            double sumInvoicesTotalYesterday = 0;
            double sumInvoicesThisMonth = 0;
            double sumInvoicesLastMonth = 0;
            //total today
            List<Int64> tempSums = new List<Int64>();
            try
            {
                tempSums = DataProvider.Instance.DB.Database.SqlQuery<Int64>("select sum(Total) from Invoice " +
                                                                                            "where Checkout = (select CAST(GETDATE() as date))").ToList();
                sumInvoicesTotal = (double)(tempSums.First());
            }
            catch { }
            para.txb_today_total.Text = ConvertToString(sumInvoicesTotal) + " VND";
            //count Invoices
            List<Int32> tempCounts = DataProvider.Instance.DB.Database.SqlQuery<Int32>("select count(ID) from Invoice " +
                                                                                        "where Checkout = (select CAST(GETDATE() as date))").ToList();
            countInvoices = (int)(tempCounts.First());
            para.txb_today_bill.Text = countInvoices.ToString();
            //compare with yesterday
            List<Int64> tempSumsYesterday = new List<Int64>();
            try
            {
                tempSumsYesterday = DataProvider.Instance.DB.Database.SqlQuery<Int64>("select sum(Total) from Invoice " +
                                                                                                    "where Checkout = (select CAST(GETDATE() - 1 as date))").ToList();
                sumInvoicesTotalYesterday = (double)(tempSumsYesterday.First());
            }
            catch { }
            if (sumInvoicesTotalYesterday != 0)
            {
                para.yesterday_compare.Text = ((int)(100 * sumInvoicesTotal / sumInvoicesTotalYesterday) - 100).ToString() + "%";
                if(((int)(100 * sumInvoicesTotal / sumInvoicesTotalYesterday) - 100).ToString().First() == '-')
                {
                    para.yesterday_compare.Foreground = (Brush)new BrushConverter().ConvertFrom("#E3507A");
                }
                para.txb_yesterday_compare.Text = ConvertToString(sumInvoicesTotal - sumInvoicesTotalYesterday) + " VND";
            }
            else
            {
                para.txb_yesterday_compare.Text = ConvertToString(sumInvoicesTotal) + " VND";
                para.yesterday_compare.Text = "There were no bills yesterday";
            }
            //compare with last month
            try
            {
                List<Int64> tempLastMonth = DataProvider.Instance.DB.Database.SqlQuery<Int64>("select sum(Total) from Invoice " +
                                                                                                "where(select month(Checkout) as month) = (select month(GETDATE()) - 1 as month)").ToList();
                sumInvoicesLastMonth = (double)(tempLastMonth.First());
            }
            catch { }
            if (sumInvoicesLastMonth != 0)
            {
                para.month_compare.Text = ((int)(100 * sumInvoicesThisMonth / sumInvoicesLastMonth) - 100).ToString() + "%";
                if (((int)(100 * sumInvoicesThisMonth / sumInvoicesLastMonth) - 100).ToString().First() == '-')
                {
                    para.month_compare.Foreground = (Brush)new BrushConverter().ConvertFrom("#E3507A");
                }
                para.txb_month_compare.Text = ConvertToString(sumInvoicesThisMonth - sumInvoicesLastMonth) + " VND";
            }
            else
            {
                para.txb_month_compare.Text = ConvertToString(sumInvoicesThisMonth) + " VND";
                para.month_compare.Text = "There were no bills last month";
            }
        }

        private void cboSelectTypeOfChartIndex_Changed(HomeWindow para)
        {
            this.ItemSourceTime.Clear();
            if (para.cboSelectTypeOfChart.SelectedIndex == 0)
            {
                this.LoadChartByAgency();
            }    
            if (para.cboSelectTypeOfChart.SelectedIndex == 1)
            {
                this.LoadChartByProduct();
            }    
        }


        private void InitColumnChart(HomeWindow para)
        {
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();

            para.cboSelectTypeOfChart.IsEnabled = true;

            para.cboSelectTypeOfChart.Text = "Agency";

            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByMonth()
                }
            };
            Labels = this.GetTop5AgencyByMonth();
            Formatter = value => ConvertToString(value);
        }

        

        #region Load Chart
        private void LoadChartByAgency()
        {
            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByMonth()
                }
            };
            Labels = this.GetTop5AgencyByMonth();
            Formatter = value => ConvertToString(value);
        }
        private void LoadChartByProduct()
        {
            AxisXTitle = "Prodcut";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5ProductsByMonth()
                }
            };
            Labels = this.GetTop5ProductByMonth();
            Formatter = value => ConvertToString(value);
        }
        private void LoadChartByYear(string year)
        {
            AxisXTitle = "Month";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalByYear(year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByYear(year)
                }
            };
            Labels = this.GetMonthInYear(year);
            Formatter = value => ConvertToString(value);
        }
        private void LoadChartByQuarter(string year)
        {
            AxisXTitle = "Quarter";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalByQuarter(year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByQuarter(year)
                }
            };
            Labels = this.GetQuarterInYear(year);
            Formatter = value => ConvertToString(value);
        }
        #endregion
        #region For Live Chart
        private string[] GetYear()
        {
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            try
            {
                string query = "SELECT YEAR(Invoice.CHECKOUT) AS YEAR FROM Invoice " +
                    "GROUP BY YEAR(Invoice.CHECKOUT)";
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();
                foreach (var item in temp)
                {
                    res.Add(item.ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
        }
        private string[] GetDayInMonth(string month, string year)
        {
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            try
            {
                string query = string.Format("SELECT DAY(CHECKOUT) AS DAY FROM Invoice " +
                    "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                    "GROUP BY DAY(CHECKOUT)", month, year);
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();
                foreach (var item in temp)
                {
                    res.Add(item.ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
        }
        private string[] GetMonthInYear(string year)
        {
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            try
            {
                string query = string.Format("SELECT MONTH(CHECKOUT) AS MONTH FROM Invoice " +
                        "WHERE YEAR(CHECKOUT) = {0} " +
                        "GROUP BY MONTH(CHECKOUT)", year);
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();

                foreach (Int32 item in temp)
                {
                    res.Add(item.ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
        }
        private string[] GetQuarterInYear(string year)
        {
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            try
            {
                string query = string.Format("SELECT DATEPART(QUARTER, CHECKOUT) AS QUARTER FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} " +
                    "GROUP BY DATEPART(QUARTER, CHECKOUT)", year);
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();

                foreach (Int32 item in temp)
                {
                    res.Add(item.ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
        }

        private ChartValues<double> GetDebtByMonth(string month, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Debt) AS TOTAL FROM Invoice " +
                    "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                    "GROUP BY DAY(CHECKOUT)", month, year);

                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        private ChartValues<double> GetDebtByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(DEBT) AS TOTAL FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} " +
                    "GROUP BY MONTH(CHECKOUT)", year);

                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        private ChartValues<double> GetDebtByQuarter(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Debt) AS TOTAL FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} " +
                    "GROUP BY DATEPART(QUARTER, CHECKOUT)", year);

                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }

        private ChartValues<double> GetTotalByMonth(string month, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Total) AS TOTAL FROM Invoice " +
                    "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} AND TOTAL > 0 " +
                    "GROUP BY DAY(CHECKOUT)", month, year);

                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        private ChartValues<double> GetTotalByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Total) AS TOTAL FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} AND TOTAL > 0 " +
                    "GROUP BY MONTH(CHECKOUT)", year);

                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        private ChartValues<double> GetTotalByQuarter(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Total) AS TOTAL FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} AND TOTAL > 0 " +
                    "GROUP BY DATEPART(QUARTER, CHECKOUT)", year);
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                foreach (Int64 item in temp)
                {
                    res.Add((double.Parse(item.ToString())));
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        #endregion
        #region For Top 5
        private string[] GetTop5AgencyByMonth()
        {
            List<Agency> agencies = new List<Agency>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();
            string sMonth = DateTime.Now.Month.ToString();

            string query = string.Format("SELECT TOP 5 Agency.ID FROM Agency " +
                "JOIN Invoice ON Agency.ID = Invoice.AgencyID " +
                "GROUP BY Agency.ID " +
                "ORDER BY SUM(INVOICE.TOTAL) DESC", sMonth);
            try
            {
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();
                foreach (Int32 item in temp)
                {
                    Agency tmp = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == item).First();
                    res.Add(tmp.Name);
                }
                res.Add("Other Agencies");
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            } 
            
        }
        private string[] GetTop5ProductByMonth()
        {
            List<Product> agencies = new List<Product>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();
            string sMonth = DateTime.Now.Month.ToString();

            string query = string.Format("SELECT TOP 5 Product.ID FROM Product " +
                "JOIN InvoiceInfo ON Product.ID = InvoiceInfo.ProductID " +
                "GROUP BY Product.ID " +
                "ORDER BY SUM(InvoiceInfo.TOTAL) DESC", sMonth);
            try
            {
                temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();
                foreach (Int32 item in temp)
                {
                    Product tmp = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == item).First();
                    res.Add(tmp.Name);
                }
                res.Add("Other Products");
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }

        }
        private ChartValues<Double> GetTotalOfTop5AgenciesByMonth()
        {
            string sMonth = DateTime.Now.Month.ToString();
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select  sum(Total) as Total from Invoice " +
                                            "group by AgencyID " +
                                            "order by Total DESC ", sMonth);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(Total) as Total from Invoice ", sMonth);
                Int64 tmp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query1).ToList().First();

                foreach (Int64 temp in temps)
                {
                    res.Add(double.Parse(temp.ToString()));
                    total += temp;
                }
                tmp -= total;
                res.Add(double.Parse(tmp.ToString()));
                return res;
            }
            catch
            {
                return res;
            }
        }
        private ChartValues<Double> GetTotalOfTop5ProductsByMonth()
        {
            string sMonth = DateTime.Now.Month.ToString();
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select sum(Total) as total from InvoiceInfo " +
                                            "group by ProductID " +
                                            "order by Total DESC ", sMonth);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(Total) as Total from InvoiceInfo ", sMonth);
                Int64 tmp = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query1).ToList().First();

                foreach (Int64 temp in temps)
                {
                    res.Add(double.Parse(temp.ToString()));
                    total += temp;
                }
                tmp -= total;
                res.Add(double.Parse(tmp.ToString()));
                return res;
            }
            catch
            {
                return res;
            }
        }
        #endregion
    }
}
