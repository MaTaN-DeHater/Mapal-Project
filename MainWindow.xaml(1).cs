using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Configuration;
using MySql.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using MySql.Data.MySqlClient;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using System.Data.OleDb;


namespace MapalProjectFab4
{
  
    
    public partial class MainWindow : System.Windows.Window
    {
#pragma warning disable CS0169 // The field 'MainWindow.id' is never used
        private static float id;
#pragma warning restore CS0169 // The field 'MainWindow.id' is never used
        private static float lval;
        private static string server = "localhost";
        private static string database = "mapaldb";
        private static string uid = "root";
        private static string password = "Matan159";
        public static string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        public static MySqlConnection conn = new MySqlConnection(connectionString);
        private static MySqlCommand cmd = new MySqlCommand();
       



        public MainWindow()
        {
            InitializeComponent();
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
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM Transactions WHERE date BETWEEN DATE('" + StartDate.Text + "') AND DATE('" + EndDate.Text + "') ", conn);

        }

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


        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            
           
        }

        private void Id_Sort_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnAboID_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBelID_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Amount_Sort_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnLogValAbo_Click(object sender, RoutedEventArgs e)
        {
            lval = float.Parse(Amount_Sort.Text);
            cmd.CommandText = "SELECT * FROM mapaldata WHERE Amount> " + lval ;
            cmd.Connection = conn;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "LoadDataBinding");
            dataGrid1.DataContext = ds;

        }

        private void btnLogValBel_Click(object sender, RoutedEventArgs e)
        {
            lval = float.Parse(Amount_Sort.Text);
            cmd.CommandText = "SELECT * FROM mapaldata WHERE Amount< " + lval ;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "LoadDataBinding");
            dataGrid1.DataContext = ds;

        }

        private void Line_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmd.CommandText = "SELECT mapaldata.LineID FROM mapaldata WHERE " + e.ToString(); //Line_List.SelectedValue;
            cmd.Connection = conn;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd.CommandText, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "LoadDataBinding");
            dataGrid1.DataContext = ds;

        }

       
    }

   
}
