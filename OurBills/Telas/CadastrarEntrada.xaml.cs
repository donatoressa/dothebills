using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace DoTheBills.Telas
{
    /// <summary>
    /// Interaction logic for CadastrarEntrada.xaml
    /// </summary>
    public partial class CadastrarEntrada : UserControl
    {
        private string connstring = String.Format("Server={0};Port={1};" +
           "User Id={2};Password={3};Database={4};",
           "localhost", "5432", "postgres",
           "12345", "DUDEBILLS");
        private List<string> months = new List<string> { "JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO", "JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO" };
        private BrushConverter converter = new BrushConverter();

        public CadastrarEntrada()
        {
            InitializeComponent();
            cbMeses.ItemsSource = months;
        }

        #region Eventos

        private void btnSalvar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(ValidarPreenchimentoCampos())
            {
                if(SalvarEntrada(tbDescricao.Text, tbValor.Text, cbDiaEntrada.SelectedIndex + 1, cbMeses.SelectedIndex + 1))
                {
                    Sucesso s = new Sucesso("Entrada cadastrada com sucesso!");
                    s.Background = Brushes.White;
                    s.Show();
                }
                else
                {
                    Erro erro = new Erro("Erro no cadastro");
                    erro.Background = Brushes.White;
                    erro.Show();
                }
            }
        }

        private void btnLimpar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbDescricao.Text = string.Empty;
            tbValor.Text = "0";
            cbDiaEntrada.SelectedIndex = -1;
            cbMeses.SelectedIndex = -1;
            tbDescricao.Focus();

            tbDescricao.BorderBrush = converter.ConvertFromString("Black") as Brush;
            tbDescricao.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            tbValor.BorderBrush = converter.ConvertFromString("Black") as Brush;
            tbValor.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            cbDiaEntrada.BorderBrush = converter.ConvertFromString("Black") as Brush;
            cbDiaEntrada.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            cbMeses.BorderBrush = converter.ConvertFromString("Black") as Brush;
            cbMeses.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
        }

        private void cbMeses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMeses.SelectedIndex != -1)
            {
                int mes = cbMeses.SelectedIndex + 1;
                int dias = DateTime.DaysInMonth(DateTime.Today.Year, mes);
                PopularComboDias(dias);
            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarPreenchimentoCampos()
        {
            bool descricao = true;
            bool valor = true;
            bool dia = true;
            bool mes = true;

            if (string.IsNullOrEmpty(tbDescricao.Text))
            {
                descricao = false;
                tbDescricao.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbDescricao.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbDescricao.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbDescricao.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if (string.IsNullOrEmpty(tbValor.Text))
            {
                valor = false;
                tbValor.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbValor.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbValor.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbValor.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if (cbDiaEntrada.SelectedIndex < 0)
            {
                dia = false;
                cbDiaEntrada.BorderBrush = converter.ConvertFromString("Red") as Brush;
                cbDiaEntrada.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                cbDiaEntrada.BorderBrush = converter.ConvertFromString("Black") as Brush;
                cbDiaEntrada.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if (cbMeses.SelectedIndex < 0)
            {
                mes = false;
                cbMeses.BorderBrush = converter.ConvertFromString("Red") as Brush;
                cbMeses.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                cbMeses.BorderBrush = converter.ConvertFromString("Black") as Brush;
                cbMeses.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            return descricao && valor && dia && mes;
        }

        private bool SalvarEntrada(string descricao_, string valor_, int dia_, int mes_)
        {
            string queryInsertEntrada = "INSERT INTO \"ENTRADAS\"(" +
               "\"DESC_ENTRADA\", \"VALOR_ENTRADA\", \"MES_ENTRADA\"," +
               "\"DIA_ENTRADA\")" +
               "VALUES ('" + descricao_ + "'," +
                       "'" + valor_.Replace("$","").Replace(",","").Replace(".","") +"'," +
                       "'" + mes_ + "'," +
                       "'" + dia_ + "');";

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            // Making connection with Npgsql provider
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            try
            {
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryInsertEntrada, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since we only showing the result we don't need connection anymore
                conn.Close();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private void PopularComboDias(int quantidadeDias)
        {
            List<int> dias = new List<int>();

            for (int i = 1; i <= quantidadeDias; i++)
            {
                dias.Add(i);
            }

            cbDiaEntrada.ItemsSource = dias;
        }

        #endregion


    }
}
