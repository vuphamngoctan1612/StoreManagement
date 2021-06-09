﻿using LiveCharts;
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

        public ICommand LoadTop3AgencyCommand { get; set; }
        public ICommand InitColumnChartCommand { get; set; }
        public ICommand SelectedPeriodChangedCommand { get; set; }
        public ICommand SelectedTimeChangeCommand { get; set; }

        public ReportViewModel()
        {
            LoadTop3AgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadTop3Agency(para));
            InitColumnChartCommand = new RelayCommand<HomeWindow>((para) => true, (para) => InitColumnChart(para));
            SelectedPeriodChangedCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cbbPeriodSelectedIndex_Changed(para));
            SelectedTimeChangeCommand = new RelayCommand<HomeWindow>((para) => true, (para) => cbbTimeSelectedIndex_Changed(para));
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

                    this.LoadChartByMonth(currentMonth, currenYear);
                }
            }
            else if (this.HomeWindow.cboSelectPeriod.SelectedIndex == 1) //theo quy
            {
                if (this.HomeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];

                    this.LoadChartByQuarter(selectedYear);
                }
            }
            else // theo nam => 12 thang
            {
                if (this.HomeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    string[] tmp = this.HomeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];

                    this.LoadChartByYear(selectedYear);
                }
            }
        }

        private void cbbPeriodSelectedIndex_Changed(HomeWindow para)
        {
            this.ItemSourceTime.Clear();
            if (para.cboSelectPeriod.SelectedIndex == 0)    //theo thang
            {
                string[] MonthInYear = this.GetMonthInYear(DateTime.Now.Year.ToString());
                //int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < MonthInYear.Length; i++)
                {
                    this.ItemSourceTime.Add(string.Format("Tháng {0}", MonthInYear[i].ToString()));
                }
            }
            else // theo nam, quy
            {
                string[] Year = this.GetYear();
                for (int i = 0; i < Year.Length; i++)
                {
                    this.ItemSourceTime.Add(string.Format("Năm {0}", Year[i].ToString()));
                }
            }
        }

        private void InitColumnChart(HomeWindow para)
        {
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();

            para.cboSelectPeriod.IsEnabled = true;
            para.cboSelectTime.IsEnabled = true;

            para.cboSelectPeriod.Text = "Theo tháng";
            para.cboSelectTime.Text = "Tháng " + month;

            AxisXTitle = "Days";
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
                }
            };
            Labels = this.GetDayInMonth(month, year);
            Formatter = value => ConvertToString(value);
        }

        private void LoadTop3Agency(HomeWindow para)
        {
            this.HomeWindow = para;
            //List<Agency> agencies = this.GetTop3AgencyByMonth(DateTime.Now.Month.ToString());
            List<Agency> agencies = this.GetTop3AgencyByMonth("4");
            int count = 1;
            foreach (Agency item in agencies)
            {
                CardStoreUC control = new CardStoreUC();
                control.txbID.Text = item.ID.ToString();
                control.tbNameStore.Text = item.Name;
                control.tbRanking.Text = string.Format("Top {0}", count);
                control.Margin = new System.Windows.Thickness(100, 10, 100, 0);
                if (count == 1)
                {
                    control.bdBG.Background = (Brush)new BrushConverter().ConvertFrom("#FF8E8E");
                    control.tbRanking.Foreground = (Brush)new BrushConverter().ConvertFrom("#D03131");
                }
                if (count == 2)
                {
                    control.bdBG.Background = (Brush)new BrushConverter().ConvertFrom("#AFF6E4");
                    control.tbRanking.Foreground = (Brush)new BrushConverter().ConvertFrom("#31D0AD");
                }
                if (count == 3)
                {
                    control.bdBG.Background = (Brush)new BrushConverter().ConvertFrom("#DCC613");
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
