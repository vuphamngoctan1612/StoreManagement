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

        private int type = 0;

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

        private ObservableCollection<string> itemSourceTime2 = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceTime2
        {
            get => itemSourceTime2;
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
        public ICommand SelectedPeriodChangedCommand { get; set; }
        public ICommand SelectedTimeChangeCommand { get; set; }
        public ICommand SelectedYearChangeCommand { get; set; }
        public ICommand LoadSalesResult { get; set; }

        public ReportViewModel()
        {
            //LoadTop3AgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadTop3Agency(para));
            InitColumnChartCommand = new RelayCommand<HomeWindow>((para) => true, (para) => InitColumnChart(para));
            SelectedTypeChangeCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cboSelectTypeOfChartIndex_Changed(para));
            LoadSalesResult = new RelayCommand<HomeWindow>((para) => true, (para) => LoadSales(para));
            SelectedPeriodChangedCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cbbPeriodSelectedIndex_Changed(para));
            SelectedTimeChangeCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cbbTimeSelectedIndex_Changed(para));
            SelectedYearChangeCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cbbYearSelectedIndex_Changed(para));
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
                if (((int)(100 * sumInvoicesTotal / sumInvoicesTotalYesterday) - 100).ToString().First() == '-')
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
            if (para.cboSelectTypeOfChart.SelectedIndex == 0)
            {
                this.LoadChartByAgency();
                if (para.cboSelectPeriod.SelectedIndex != 2)
                {
                    para.cboSelectYear.Visibility = System.Windows.Visibility.Visible;
                }
                type = 0;
            }
            if (para.cboSelectTypeOfChart.SelectedIndex == 1)
            {
                this.LoadChartByProduct();
                if (para.cboSelectPeriod.SelectedIndex != 2)
                {
                    para.cboSelectYear.Visibility = System.Windows.Visibility.Visible;
                }
                type = 1;
            }
            if (para.cboSelectTypeOfChart.SelectedIndex == 2)
            {
                string month = DateTime.Now.Month.ToString();
                string year = DateTime.Now.Year.ToString();
                para.cboSelectYear.Visibility = System.Windows.Visibility.Hidden;
                this.LoadChartByMonth(month, year);
                type = 2;
            }    
        }

        private void cbbYearSelectedIndex_Changed(HomeWindow para)
        {
            if (para.cboSelectYear.SelectedIndex != -1)
            {
                string currentMonth;
                if (para.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    currentMonth = tmp[1];
                } else
                {
                    currentMonth = DateTime.Now.Month.ToString();
                }
                string[] tmpyear = this.HomeWindow.cboSelectYear.SelectedValue.ToString().Split(' ');
                string selectedYear = tmpyear[1];
                string currenYear = DateTime.Now.Year.ToString();
                int quarter = this.HomeWindow.cboSelectTime.SelectedIndex + 1;
                if (type == 0)
                {
                    this.LoadChartByAgencyAndYearMonth(currentMonth, selectedYear);
                }
                else if (type == 1)
                {
                    this.LoadChartByProductYearMonth(currentMonth, selectedYear);
                }
            }
        }

        private void cbbPeriodSelectedIndex_Changed(HomeWindow para)
        {
            this.ItemSourceTime.Clear();
            this.ItemSourceTime2.Clear();
            if (para.cboSelectPeriod.SelectedIndex == 0)    //theo thang
            {
                string[] MonthInYear = this.GetMonthInYear(DateTime.Now.Year.ToString());
                //int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < MonthInYear.Length; i++)
                {
                    this.ItemSourceTime.Add(string.Format("Tháng {0}", MonthInYear[i].ToString()));
                }
                string[] Year = this.GetYear();
                for (int i = 0; i < Year.Length; i++)
                {
                    this.ItemSourceTime2.Add(string.Format("Năm {0}", Year[i].ToString()));
                }
                if (type != 2)
                    para.cboSelectYear.Visibility = System.Windows.Visibility.Visible;
            }
            else if(para.cboSelectPeriod.SelectedIndex == 1) //theo quy
            {
                if (type == 2)
                {
                    string[] Year = this.GetYear();
                    for (int i = 0; i < Year.Length; i++)
                    {
                        this.ItemSourceTime.Add(string.Format("Năm {0}", Year[i].ToString()));
                        this.ItemSourceTime2.Add(string.Format("Năm {0}", Year[i].ToString()));
                    }
                }
                else
                {
                    string[] Year = this.GetYear();
                    for (int i = 0; i < Year.Length; i++)
                    {
                        this.ItemSourceTime2.Add(string.Format("Năm {0}", Year[i].ToString()));
                    }
                    for(int i = 1; i < 5; i++)
                    {

                        this.ItemSourceTime.Add(string.Format("Quý {0}", i.ToString()));
                    }
                    para.cboSelectYear.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                string[] Year = this.GetYear();
                for (int i = 0; i < Year.Length; i++)
                {
                    this.ItemSourceTime.Add(string.Format("Năm {0}", Year[i].ToString()));
                }
                if(type != 2)
                    para.cboSelectYear.Visibility = System.Windows.Visibility.Hidden;
            }    
            
        }

        private void cbbTimeSelectedIndex_Changed(HomeWindow para)
        {
            this.HomeWindow = para;
            if (this.HomeWindow.cboSelectPeriod.SelectedIndex == 0) // theo thang
            {
                if (this.HomeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string currentMonth = tmp[1];
                    string currenYear = DateTime.Now.Year.ToString();
                    if (type == 0)
                    {
                        this.LoadChartByAgencyAndYearMonth(currentMonth, currenYear);
                    }
                    else if (type == 1)
                    {
                        this.LoadChartByProductYearMonth(currentMonth, currenYear);
                    } 
                    else
                        this.LoadChartByMonth(currentMonth, currenYear);
                }
            }
            else if (this.HomeWindow.cboSelectPeriod.SelectedIndex == 1) //theo quy
            {
                if (this.HomeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    string currenYear = DateTime.Now.Year.ToString();
                    int quarter = this.HomeWindow.cboSelectTime.SelectedIndex + 1;
                    if (type == 0)
                    {
                        this.LoadChartByAgencyAndYearQuater(quarter.ToString(), currenYear);
                    } 
                    else if (type == 1)
                    {
                        this.LoadChartByProductYearQuarter(quarter.ToString(), currenYear);
                    } 
                    else
                        this.LoadChartByQuarter(selectedYear);
                }
            }
            else // theo nam => 12 thang
            {
                if (this.HomeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    if (type == 0)
                    {
                        this.LoadChartByAgencyAndYear(selectedYear);
                    }    
                    else if (type == 1)
                    {
                        this.LoadChartByProductYear(selectedYear);
                    }    
                    else
                        this.LoadChartByYear(selectedYear);
                }
            }
            
        }

        private void InitColumnChart(HomeWindow para)
        {
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();

            para.cboSelectTypeOfChart.IsEnabled = true;

            para.cboSelectTypeOfChart.Text = "Total And Debt";

            para.cboSelectPeriod.Text = "Theo tháng";

            para.cboSelectTime.Text = "Tháng " + month;

            para.cboSelectYear.Visibility = System.Windows.Visibility.Hidden;

            LoadChartByMonth(month, year);
        }



        #region Load Chart
        public void LoadChartByAgency()
        {
            string currenYear = DateTime.Now.Year.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByMonth(currentMonth, currenYear)
                }
            };
            Labels = this.GetTop5AgencyByMonth(currentMonth, currenYear);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByAgencyAndYear(string year)
        {
            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByYear(year)
                }
            };
            Labels = this.GetTop5AgencyByYear(year);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByAgencyAndYearMonth(string month, string year)
        {
            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByMonth(month, year)
                }
            };
            Labels = this.GetTop5AgencyByMonth(month, year);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByAgencyAndYearQuater(string quarter, string year)
        {
            AxisXTitle = "Agency";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5AgenciesByQuarter(quarter, year)
                }
            };
            Labels = this.GetTop5AgencyByQuarter(quarter, year);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByProduct()
        {
            string currenYear = DateTime.Now.Year.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            AxisXTitle = "Prodcut";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5ProductsByMonth(currentMonth, currenYear)
                }
            };
            Labels = this.GetTop5ProductByMonth(currentMonth, currenYear);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByProductYearMonth(string month, string year)
        {
            AxisXTitle = "Prodcut";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5ProductsByMonth(month, year)
                }
            };
            Labels = this.GetTop5ProductByMonth(month, year);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByProductYear(string year)
        {
            AxisXTitle = "Prodcut";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5ProductsByYear(year)
                }
            };
            Labels = this.GetTop5ProductByYear(year);
            Formatter = value => ConvertToString(value);
        }
        public void LoadChartByProductYearQuarter(string quarter, string year)
        {
            AxisXTitle = "Prodcut";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalOfTop5ProductsByQuarter(quarter, year)
                }
            };
            Labels = this.GetTop5ProductByQuarter(quarter, year);
            Formatter = value => ConvertToString(value);
        }
        private void LoadChartByMonth(string month, string year)
        {
            AxisXTitle = "Day";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = this.GetTotalByMonth(month, year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByMonth(month, year)
                },
                new ColumnSeries
                {
                    Title = "Cost",
                    Values = this.GetCostByMonth(month, year)
                }
            };
            Labels = this.GetDayInMonth(month, year);
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
                },
                new ColumnSeries
                {
                    Title  = "Cost",
                    Values = this.GetCostByYear(year)
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
                },
                new ColumnSeries
                {
                    Title  = "Cost",
                    Values = this.GetCostByQuarter(year)
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
                    "GROUP BY DAY(CHECKOUT) " +
                    "UNION " +
                    "SELECT DAY(CHECKIN) AS DAY FROM StockReceipt " +
                    "WHERE MONTH(CHECKIN) = {0} AND YEAR(CHECKIN) = {1} " +
                    "GROUP BY DAY(CHECKIN)", month, year);
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
                string query = string.Format("SELECT MONTH(CHECKOUT) AS DAY FROM Invoice " +
                    "WHERE YEAR(CHECKOUT) = {0} " +
                    "GROUP BY MONTH(CHECKOUT) " +
                    "UNION " +
                    "SELECT MONTH(CHECKIN) AS DAY FROM StockReceipt " +
                    "WHERE YEAR(CHECKIN) = {0} " +
                    "GROUP BY MONTH(CHECKIN)", year);
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
                    "GROUP BY DATEPART(QUARTER, CHECKOUT) " +
                    "UNION " +
                    "SELECT DATEPART(QUARTER, CHECKIN) AS QUARTER FROM StockReceipt " +
                    "WHERE YEAR(CHECKIN) = {0} " +
                    "GROUP BY DATEPART(QUARTER, CHECKIN)", year);
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

        private ChartValues<double> GetCostByMonth(string month, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(TOTAL) AS TOTAL FROM StockReceipt " +
                    "WHERE MONTH(CHECKIN) = {0} AND YEAR(CHECKIN) = {1} " +
                    "GROUP BY DAY(CHECKIN)", month, year);

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
        private ChartValues<double> GetCostByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(TOTAL) AS TOTAL FROM StockReceipt " +
                    "WHERE YEAR(CHECKIN) = {0} " +
                    "GROUP BY MONTH(CHECKIN)", year);

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
        private ChartValues<double> GetCostByQuarter(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(TOTAL) AS TOTAL FROM StockReceipt " +
                    "WHERE YEAR(CHECKIN) = {0} " +
                    "GROUP BY DATEPART(QUARTER, CHECKIN)", year);

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
        private ChartValues<double> GetTotalByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Total) AS TOTAL FROM Invoice " +
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
        private ChartValues<double> GetTotalByQuarter(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temp = new List<Int64>();

            try
            {
                string query = string.Format("SELECT SUM(Total) AS TOTAL FROM Invoice " +
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
        #endregion
        #region For Top 5
        private string[] GetTop5AgencyByMonth(string month, string year)
        {
            List<Agency> agencies = new List<Agency>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Agency.ID FROM Agency " +
                "JOIN Invoice ON Agency.ID = Invoice.AgencyID " +
                "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                "GROUP BY Agency.ID " +
                "ORDER BY SUM(INVOICE.TOTAL) DESC", month, year);
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
        private string[] GetTop5AgencyByYear(string year)
        {
            List<Agency> agencies = new List<Agency>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Agency.ID FROM Agency " +
                "JOIN Invoice ON Agency.ID = Invoice.AgencyID " +
                "WHERE YEAR(CHECKOUT) = {0} " +
                "GROUP BY Agency.ID " +
                "ORDER BY SUM(INVOICE.TOTAL) DESC", year);
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
        private string[] GetTop5AgencyByQuarter(string year, string quarter)
        {
            List<Agency> agencies = new List<Agency>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Agency.ID FROM Agency " +
                "JOIN Invoice ON Agency.ID = Invoice.AgencyID " +
                "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} " +
                "GROUP BY Agency.ID " +
                "ORDER BY SUM(INVOICE.TOTAL) DESC", year, quarter);
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
        private string[] GetTop5ProductByMonth(string month, string year)
        {
            List<Product> agencies = new List<Product>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Product.ID FROM Product " +
                "JOIN InvoiceInfo ON Product.ID = InvoiceInfo.ProductID " +
                "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                "GROUP BY Product.ID " +
                "ORDER BY SUM(InvoiceInfo.TOTAL) DESC", month, year);
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
        private string[] GetTop5ProductByYear(string year)
        {
            List<Product> agencies = new List<Product>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Product.ID FROM Product " +
                "JOIN InvoiceInfo ON Product.ID = InvoiceInfo.ProductID " +
                "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                "WHERE YEAR(CHECKOUT) = {0} " +
                "GROUP BY Product.ID " +
                "ORDER BY SUM(InvoiceInfo.TOTAL) DESC",year);
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
        private string[] GetTop5ProductByQuarter(string year, string quarter)
        {
            List<Product> agencies = new List<Product>();
            List<string> res = new List<string>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 5 Product.ID FROM Product " +
                "JOIN InvoiceInfo ON Product.ID = InvoiceInfo.ProductID " +
                "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} " +
                "GROUP BY Product.ID " +
                "ORDER BY SUM(InvoiceInfo.TOTAL) DESC", year, quarter);
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
        private ChartValues<Double> GetTotalOfTop5AgenciesByMonth(string month, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                                            "group by AgencyID " +
                                            "order by Total DESC ", month, year);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} ", month, year);
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
        private ChartValues<Double> GetTotalOfTop5AgenciesByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE YEAR(CHECKOUT) = {0} " +
                                            "group by AgencyID " +
                                            "order by Total DESC ", year);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE YEAR(CHECKOUT) = {0} ", year);
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
        private ChartValues<Double> GetTotalOfTop5AgenciesByQuarter(string quarter, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} " +
                                            "group by AgencyID " +
                                            "order by Total DESC ", year, quarter);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(Total) as Total from Invoice " +
                                            "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} ", year, quarter);
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
        private ChartValues<Double> GetTotalOfTop5ProductsByMonth(string month, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select sum(InvoiceInfo.Total) as total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                            "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} " +
                                            "group by ProductID " +
                                            "order by Total DESC ", month, year);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(InvoiceInfo.Total) as Total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                             "WHERE MONTH(CHECKOUT) = {0} AND YEAR(CHECKOUT) = {1} ", month, year);
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
        private ChartValues<Double> GetTotalOfTop5ProductsByYear(string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select sum(InvoiceInfo.Total) as total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                            "WHERE YEAR(CHECKOUT) = {0} " +
                                            "group by ProductID " +
                                            "order by Total DESC ", year);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(InvoiceInfo.Total) as Total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                             "WHERE YEAR(CHECKOUT) = {0} ", year);
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
        private ChartValues<Double> GetTotalOfTop5ProductsByQuarter(string quarter, string year)
        {
            ChartValues<double> res = new ChartValues<double>();
            List<Int64> temps = new List<Int64>();
            Int64 total = 0;
            try
            {
                string query = string.Format("select sum(InvoiceInfo.Total) as total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                            "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} " + 
                                            "group by ProductID " +
                                            "order by Total DESC ", year, quarter);
                temps = DataProvider.Instance.DB.Database.SqlQuery<Int64>(query).ToList();

                string query1 = string.Format("select  sum(InvoiceInfo.Total) as Total from InvoiceInfo " +
                                            "JOIN Invoice ON Invoice.ID = InvoiceInfo.InvoiceID " +
                                             "WHERE YEAR(CHECKOUT) = {0} AND DATEPART(QUARTER, CHECKOUT) = {1} ", year, quarter);
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
