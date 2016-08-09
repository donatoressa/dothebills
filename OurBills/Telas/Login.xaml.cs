using Npgsql;
using DoTheBills.Gerenciadores;
using DoTheBills.Telas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

namespace DoTheBills
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private GerenciadorJanelas gj = new GerenciadorJanelas();
        DataSet datasetUsuario = new DataSet();
        DataTable datatableUsuario = new DataTable();
        // PostgeSQL-style connection string
        //private SQLiteConnection sql_con = new SQLiteConnection("Data Source=DoTheBills;Version=3;New=False;Compress=True;");
        //private SQLiteCommand sql_cmd;
        //private SQLiteDataAdapter DB;

        string connstring = String.Format("Server={0};Port={1};"+
                    "User Id={2};Password={3};Database={4};",
                    "localhost", "5432", "postgres",
                    "12345", "DUDEBILLS");

        public Login()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            
            tbUsuario.Focus();
        }

        private void btnLogin_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(ValidarUsuarioSenha(tbUsuario.Text, tbSenha.Password))
            {
                msgErro.Visibility = Visibility.Collapsed;
                Principal principal = new Principal(datasetUsuario);
                gj.AlternarExibicao(this, principal);
            }
            else
            {
                msgErro.Visibility = Visibility.Visible;
            }
        }

        private bool ValidarUsuarioSenha(string usuario, string senha)
        {
            BrushConverter converter = new BrushConverter();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                if (string.IsNullOrEmpty(usuario))
                {
                    tbUsuario.BorderBrush = converter.ConvertFromString("Red") as Brush;
                    tbUsuario.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
                }
                else
                {
                    tbUsuario.BorderBrush = converter.ConvertFromString("Black") as Brush;
                    tbUsuario.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                }

                if (string.IsNullOrEmpty(senha))
                {
                    tbSenha.BorderBrush = converter.ConvertFromString("Red") as Brush;
                    tbSenha.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
                }
                else
                {
                    tbSenha.BorderBrush = converter.ConvertFromString("Black") as Brush;
                    tbSenha.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                }

                return false;
            }
            else
            {
                tbUsuario.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbUsuario.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                tbSenha.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbSenha.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            GerenciadorCriptografia gc = new GerenciadorCriptografia();
            string senhaCripto = gc.EncodeMD5Password(senha);

            string queryConsultarUsuario = "SELECT * FROM \"USUARIOS\" WHERE \"LOGIN\" = '" + usuario.ToLower() + "' AND \"SENHA\" = '" + senhaCripto + "'";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryConsultarUsuario, conn);
            //sql_con.Open();
            //sql_cmd = sql_con.CreateCommand();
            //DB.SelectCommand = queryConsultarUsuario;
            //DB = new SQLiteDataAdapter(queryConsultarUsuario,sql_con);
            datasetUsuario.Reset();
            da.Fill(datasetUsuario);
            //DB.Fill(datasetUsuario);
            datatableUsuario = datasetUsuario.Tables[0];
            //sql_con.Close();
            conn.Close();

            if (datasetUsuario.Tables[0].Rows.Count > 0)
            {
                if (senhaCripto.Equals(datasetUsuario.Tables[0].Rows[0].Field<string>("SENHA").ToString()) &&
                    usuario.Equals(datasetUsuario.Tables[0].Rows[0].Field<string>("LOGIN").ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
