using Npgsql;
using DoTheBills.Gerenciadores;
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
using System.Windows.Shapes;

namespace DoTheBills.Telas
{
    /// <summary>
    /// Interaction logic for ExcluirPagamentos.xaml
    /// </summary>
    public partial class ExcluirPagamentos : Window
    {
        // PostgeSQL-style connection string
        private string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "localhost", "5432", "postgres",
                   "12345", "DUDEBILLS");

        public EventHandler<ExcluirPagamentoEventArgs> pagamentoExcluido;

        public ExcluirPagamentos(List<string> pagamentos_)
        {
            InitializeComponent();
            tbExcluir.Text = "SELECIONE UM ITEM PARA EXCLUIR";
            list.ItemsSource = pagamentos_;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            double screeHeight = SystemParameters.FullPrimaryScreenHeight;
            double screeWidth = SystemParameters.FullPrimaryScreenWidth;
            this.Top = (screeHeight - this.Height) / 2;
            this.Left = (screeWidth - this.Width) / 2;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void btnExcluir_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedIndex >= 0)
            {
                string pag = (string)list.SelectedItem;
                string [] nomePagamento = pag.Replace("[-] ","").Split('|');

                try
                {
                    DataSet datasetTipoPag = new DataSet();
                    DataTable datatableTipoPag = new DataTable();
                    string queryExclusao = "DELETE FROM \"PAGAMENTOS\" WHERE \"DSC_PAGAMENTO\"='" + nomePagamento[0].TrimEnd().TrimStart() + "' AND \"VLR_PAGAMENTO\"='" + nomePagamento[1].Replace("R$", "").Replace(",", "") + "';";
                    NpgsqlConnection conn = new NpgsqlConnection(connstring);
                    conn.Open();
                    NpgsqlDataAdapter daTpPag = new NpgsqlDataAdapter(queryExclusao, conn);

                    datasetTipoPag.Reset();
                    daTpPag.Fill(datasetTipoPag);
                    conn.Close();

                    Sucesso s = new Sucesso("Pagamento excluído com sucesso!");
                    s.Background = Brushes.White;
                    s.Show();
                    //if (this.pagamentoExcluido != null)
                    //    this.pagamentoExcluido(this, new ExcluirPagamentoEventArgs());

                    this.Close();
                }
                catch(Exception ex)
                {
                    Erro erro = new Erro("Erro na exclusão");
                    erro.Background = Brushes.White;
                    erro.Show();
                    this.Close();
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
