using LiveCharts;
using LiveCharts.Wpf;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
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

        public ICommand LoadTop3AgencyCommand { get; set; }
        public ICommand InitColumnChartCommand { get; set; }

        public ReportViewModel()
        {
            LoadTop3AgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadTop3Agency(para));
            InitColumnChartCommand = new RelayCommand<HomeWindow>((para) => true, (para) => InitColumnChart(para));
        }

        private void InitColumnChart(HomeWindow para)
        {
            AxisXTitle = "Days";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenue",
                    Values = this.GetTotalByMonth("3", "2021")
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByMonth("3", "2021")
                }
            };
            Labels = this.GetDayInMonth("3", "2021");
            Formatter = value => string.Format("{0:N0}", value);
        }

        private void LoadTop3Agency(HomeWindow para)
        {
            this.HomeWindow = para;
            List<Agency> agencies = this.GetTop3AgencyByMonth(DateTime.Now.Month.ToString());
            int count = 1;
            foreach (Agency item in agencies)
            {
                StoresHomeUC control = new StoresHomeUC();
                control.tbNameStore.Text = item.Name;
                control.tbRanking.Text = string.Format("Top {0}", count);
                control.Margin = new System.Windows.Thickness(100, 10, 80, 10);
                if (count == 1)
                {
                    control.recBG.Fill = (Brush)new BrushConverter().ConvertFrom("#FF8E8E");
                    control.tbRanking.Foreground = (Brush)new BrushConverter().ConvertFrom("#D03131");
                }
                if (count == 2)
                {
                    control.recBG.Fill = (Brush)new BrushConverter().ConvertFrom("#AFF6E4");
                    control.tbRanking.Foreground = (Brush)new BrushConverter().ConvertFrom("#31D0AD");
                }
                if (count == 3)
                {
                    control.recBG.Fill = (Brush)new BrushConverter().ConvertFrom("#FEEFDA");
                    control.tbRanking.Foreground = (Brush)new BrushConverter().ConvertFrom("#DCC613");
                }

                this.HomeWindow.wpBody_Main_TopAgency.Children.Add(control);
                count++;
            }
        }

        #region Load Chart
        private void LoadChartByMonth(string month, string year)
        {
            AxisXTitle = "Day";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenue",
                    Values = this.GetTotalByMonth(month, year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByMonth(month, year)
                }
            };
            Labels = this.GetDayInMonth(month, year);
            Formatter = value => string.Format("{0:N0}", value);
        }
        private void LoadChartByYear(string year)
        {
            AxisXTitle = "Month";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenue",
                    Values = this.GetTotalByYear(year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByYear(year)
                }
            };
            Labels = this.GetMonthInYear(year);
            Formatter = value => string.Format("{0:N0}", value);
        }
        private void LoadChartByQuarter(string year)
        {
            AxisXTitle = "Quarter";
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenue",
                    Values = this.GetTotalByQuarter(year)
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByQuarter(year)
                }
            };
            Labels = this.GetQuarterInYear(year);
            Formatter = value => string.Format("{0:N0}", value);
        }
        #endregion
        #region For Live Chart
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
                string query = string.Format("SELECT SUM(Total) FROM InvoiceInfo " +
                    "JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID " +
                    "WHERE MONTH(Checkout) = {0} AND YEAR(CHECKOUT) = {1} " +
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
                string query = string.Format("SELECT SUM(Total) FROM InvoiceInfo " +
                    "JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID " +
                    "WHERE YEAR(CHECKOUT) = {0} " +
                    "GROUP BY MONTH(Checkout)", year);

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
                string query = string.Format("SELECT SUM(Total) FROM InvoiceInfo " +
                    "JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID " +
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
        #region For Top 3
        private List<Agency> GetTop3AgencyByMonth(string month)
        {
            List<Agency> res = new List<Agency>();
            List<Int32> temp = new List<Int32>();

            string query = string.Format("SELECT TOP 3 Agency.ID FROM Agency " +
                "JOIN Invoice ON Agency.ID = Invoice.AgencyID " +
                "WHERE MONTH(CHECKOUT) = {0} " +
                "GROUP BY Agency.ID " +
                "ORDER BY SUM(INVOICE.TOTAL) DESC", month);
            temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();
            foreach (Int32 item in temp)
            {
                Agency tmp = (Agency)DataProvider.Instance.DB.Agencies.Where(x => x.ID == item).First();
                res.Add(tmp);
            }

            return res;
        }
        #endregion
    }
}
