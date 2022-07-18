using MySql.Data.MySqlClient;
using System.Windows;

namespace MapalProjectFab4
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        MySqlConnection connection = new MySqlConnection();
        MainWindow mw = new MainWindow();
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// creating connection string from user input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            while(true)
            {
                string connectingString = "server = " + Server.Text + ";User Id = " + UserName.Text + ";Password = " + Password.Password.ToString() + ";DataBase =" + DataBase.Text;
                connection.ConnectionString = connectingString;
                try
                {
                    connection.Open();
                    connection.Close();
                    MainWindow.conn.ConnectionString = connectingString;
                    mw.ShowDialog();
                    this.Hide();
                    this.Close();
                   
                }
                catch
                {
                    MessageBox.Show("Please Try Again, Something went worng","LoginError",MessageBoxButton.OK,MessageBoxImage.Error);
                    break;
                }
            }
          
        }

    }
}
