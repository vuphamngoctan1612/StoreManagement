using LiveCharts;
using LiveCharts.Wpf;
using StoreManagement.Models;
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
                    Values = this.GetTotalByQuarter("2021")
                },
                new ColumnSeries
                {
                    Title  = "Debt",
                    Values = this.GetDebtByQuarter("2021")
                }
            };
            Labels = this.GetQuarterInYear(DateTime.Now.Year.ToString());
            Formatter = value => string.Format("{0:N0}", value);
        }

        private void LoadTop3Agency(HomeWindow para)
        {

        }

        //ReportDAL
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
    }
}
