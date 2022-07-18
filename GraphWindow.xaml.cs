using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveChartsCore;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace MapalProjectFab4
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        private static MySqlCommand cmd = new MySqlCommand();
        public SeriesCollection series1 = new SeriesCollection();
        public SeriesCollection sc = new SeriesCollection();
        public GraphWindow()
        {
            InitializeComponent();
            


            DataContext = this;
        }


          
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public Func<ChartPoint, string> PointLabel { get; set; }

        /// <summary>
        /// loading the piechart with data 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="chartpoint"></param>
        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            LiveCharts.Wpf.PieChart chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;
            }

            PieSeries selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;

        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
           

        }
        
        private void AmountChart_Loaded(object sender, RoutedEventArgs e)
        {
           
        }


        /// <summary>
        /// loading the charts 
        /// </summary>
        public void Load()
        {
            AmountChart.Series = series1;
            Histo.Series =sc;
        }

        /// <summary>
        /// close window button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                e.Cancel = true;
               
                
                Hide();

            }
            else
            {
                e.Cancel = true;

            }
        }
        private void Histo_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
