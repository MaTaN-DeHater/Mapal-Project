using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml;
using LiveCharts;
using LiveCharts.Wpf;
using SeriesCollection = LiveCharts.SeriesCollection;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace MapalProjectFab4
{


    public partial class MainWindow : System.Windows.Window
    {


        public static MySqlConnection conn = new MySqlConnection();
        private static MySqlCommand cmd = new MySqlCommand();
        GraphWindow graph = new GraphWindow();
        public string ope = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open the mainwindow checks the connection status and filling the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("Select * FROM mapaldata", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds, "LoadDataBinding");
            dataGrid1.DataContext = ds;
            if ((int)conn.State != 0)
            {
                circle.Fill = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                circle.Fill = new SolidColorBrush(Colors.Red);
            }
            DataContext = this;
        }
        public Func<ChartPoint, string> PointLabel { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {


        }


        /// <summary>
        /// Updating the config file to create connection string
        /// </summary>
        /// <param name="con"></param>
        public void updateConfigFile(string con)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement xElement in xmlDoc.DocumentElement)
            {
                if (xElement.Name == "connectionString")
                {
                    xElement.FirstChild.Attributes[2].Value = con;

                }
            }
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        /// <summary>
        /// Data Grid row counter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Adding 1 to make the row count start at 1 instead of 0
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Exports to Excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string ExportName = (sender as Button).Name.ToString();
            bool result = SaveToCSV(dataGrid1, ExportName);//pass the Datagrid and Exportname
            if (result == true)
            {
                _ = MessageBox.Show("Exported successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Save the data CSV format 
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public bool SaveToCSV(DataGrid dataGrid, string Filename)
        {
            bool IsVaild = false;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save CSV Files";
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            saveFileDialog.FileName = Filename;
            string gridname = Filename;
            if (saveFileDialog.ShowDialog().Value)
            {
                string path = Path.GetFullPath(saveFileDialog.FileName);
                createcsvfile(dataGrid, path);
                IsVaild = true;
            }
            return IsVaild;
        }

        /// <summary>
        /// Creating CSV file 
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="FilePath"></param>
        private void createcsvfile(DataGrid dataGrid, string FilePath)
        {
            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);
            dataGrid.UnselectAllCells();
            String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            File.AppendAllText(FilePath, result, UnicodeEncoding.UTF8);
        }

        private void Amount_Sort_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        /// <summary>
        /// ComboBox to filter by LineID in the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Line_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = Line_List.SelectedIndex;
            ComboBoxItem sitem = (ComboBoxItem)Line_List.Items.GetItemAt(i);
            cmd.CommandText = "SELECT * FROM mapaldata WHERE LineID= " + "'" + sitem.Name + "'";
            cmd.Connection = conn;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "LoadDataBinding");
            dataGrid1.DataContext = ds;

        }

        /// <summary>
        /// filtring between dates and hours 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apply_Click(object sender, RoutedEventArgs e)
        {
            cmd.Connection = conn;
            if (DatePickStart.Value < DatePickEnd.Value)
            {
                cmd.CommandText = "SELECT * FROM mapaldata WHERE StartTime BETWEEN " + "'" + DatePickStart.Text + "%" + "'" + "AND  " + "'" + DatePickEnd.Text + "%" + "'; ";
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "LoadDataBinding");
                dataGrid1.DataContext = ds;
                SeriesCollection series = new SeriesCollection();
                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata Where StartTime BETWEEN " + "'" + DatePickStart.Text + "'" + "AND  '" + DatePickEnd.Text + "' Group By LineID";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    series.Add(new PieSeries() { Title = reader.GetString("LineId").ToString(), Values = new ChartValues<Decimal> { Math.Round(reader.GetDecimal("AVG(Amount)"), 2) }, DataLabels = true });
                }

                reader.Close();
                graph.series1 = series;
                Histo_Loaded(sender,e);
                graph.Load();
                graph.Show();

            }
            else
            {
                better.Visibility = Visibility.Visible;
                MessageBox.Show("StartDate is Always Before EndDate Please Try agian", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            better.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// sorting and filtring the datagrid by user input , if theres wrong input error will popup 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Amount_Sort_KeyDown(object sender, KeyEventArgs e)
        {
            float num;
            bool isnum = float.TryParse(Amount_Sort.Text, out num);
            if (e.Key == Key.Enter)// enter
            {

                if (isnum)
                {
                    cmd.CommandText = "SELECT * FROM mapaldata WHERE Amount " + ope + Amount_Sort.Text + " ORDER BY Amount";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "LoadDataBinding");
                    dataGrid1.DataContext = ds;
                }
                else
                {
                    better.Visibility = Visibility.Visible;
                    MessageBox.Show("Wrong Typing.", "TypeError", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                better.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// opens the graph's window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphWindow_Click(object sender, RoutedEventArgs e)
        {
            graph.Show();
            AmountChart_Loaded(sender, e);
            Histo_Loaded(sender, e);


        }

        /// <summary>
        /// filling the pie chart with data 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AmountChart_Loaded(object sender, RoutedEventArgs e)
        {
            SeriesCollection series = new SeriesCollection();
            MySqlCommand cmd = new MySqlCommand("SELECT LineID, sum(Amount) from mapaldata group by LineID", conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                series.Add(new PieSeries() { Title = reader.GetString("LineID").ToString(), Values = new ChartValues<Decimal> { Math.Round(reader.GetDecimal("sum(Amount)"), 2) }, DataLabels = true });
            }

            PieChart.Colors = new List<Color>
            {
              Colors.DeepSkyBlue,
              Colors.LimeGreen,
              Colors.Red,
              Colors.DarkKhaki,
              Colors.Gold

            };
            reader.Close();
            graph.series1 = series;
            graph.Load();

        }

        /// <summary>
        /// filling the line chart with data 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Histo_Loaded(object sender, RoutedEventArgs e)
        {
            cmd.Connection = conn;
            DateTime date1 = new DateTime(2021, 10, 01);
            DateTime date2 = new DateTime(2021, 11, 08);
            List<decimal> dLb = new List<decimal>();
            List<decimal> dLh = new List<decimal>();
            List<decimal> dLy = new List<decimal>();
            List<decimal> dLk = new List<decimal>();
            List<decimal> dLg = new List<decimal>();


            SeriesCollection scol = new SeriesCollection();
            MySqlDataReader reader;
            while (date1 != date2)
            {
                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata WHERE LineID='LHPM' AND StartTime BETWEEN " + "'" + date1.ToString("yyyy-MM-dd") + "'" + "AND" + "'" + date1.AddDays(1).ToString("yyyy-MM-dd") + "'" + ";";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1))
                    {
                        continue;
                    }
                    dLh.Add(reader.GetDecimal("AVG(Amount)"));
                }
                reader.Close();

                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata WHERE LineID='LBlue' AND StartTime BETWEEN " + "'" + date1.ToString("yyyy-MM-dd") + "'" + "AND" + "'" + date1.AddDays(1).ToString("yyyy-MM-dd") + "'" + ";";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1))
                    {
                        continue;
                    }
                    dLb.Add(reader.GetDecimal("AVG(Amount)"));

                }
                reader.Close();

                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata WHERE LineID='LYellow' AND StartTime BETWEEN " + "'" + date1.ToString("yyyy-MM-dd") + "'" + "AND" + "'" + date1.AddDays(1).ToString("yyyy-MM-dd") + "'" + ";";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1))
                    {
                        continue;
                    }
                    dLy.Add(reader.GetDecimal("AVG(Amount)"));


                }
                reader.Close();

                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata WHERE LineID='LKuhne' AND StartTime BETWEEN " + "'" + date1.ToString("yyyy-MM-dd") + "'" + "AND" + "'" + date1.AddDays(1).ToString("yyyy-MM-dd") + "'" + ";";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1))
                    {
                        continue;
                    }
                    dLk.Add(reader.GetDecimal("AVG(Amount)"));

                }
                reader.Close();

                cmd.CommandText = "SELECT LineID, AVG(Amount) FROM mapaldata WHERE LineID='LGreen' AND StartTime BETWEEN " + "'" + date1.ToString("yyyy-MM-dd") + "'" + "AND" + "'" + date1.AddDays(1).ToString("yyyy-MM-dd") + "'" + ";";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1))
                    {
                        continue;
                    }
                    dLg.Add(reader.GetDecimal("AVG(Amount)"));
                }
                date1 = date1.AddDays(1);
                reader.Close();

            }

            scol.Add(new LineSeries() { Title = "LBlue", Values = new ChartValues<decimal>(dLb) });
            scol.Add(new LineSeries() { Title = "LGreen", Values = new ChartValues<decimal>(dLg) });
            scol.Add(new LineSeries() { Title = "LHPM", Values = new ChartValues<decimal>(dLh) });
            scol.Add(new LineSeries() { Title = "LKuhne", Values = new ChartValues<decimal>(dLk) });
            scol.Add(new LineSeries() { Title = "LYellow", Values = new ChartValues<decimal>(dLy) });

            graph.sc = scol;
            graph.Load();
            graph.Show();
        }

        /// <summary>
        /// closing the program 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;

            }
        }

        private void better_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {

        }

        /// <summary>
        /// supporting function to the amount filtering function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueOp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ValueOp.SelectedIndex;
            ComboBoxItem sitem = (ComboBoxItem)ValueOp.Items.GetItemAt(i);
            switch (sitem.Name)
            {

                case "above":
                    ope = ">";
                    break;
                case "equals":
                    ope = "=";
                    break;
                case "under":
                    ope = "<";
                    break;
                case "equalsAbove":
                    ope = ">=";
                    break;
                case "equalsUnder":
                    ope = "<=";
                    break;
                default:
                    break;
            }

        }
    }
}









