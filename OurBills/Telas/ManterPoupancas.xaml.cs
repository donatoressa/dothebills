using Npgsql;
using DoTheBills.Gerenciadores;
using DoTheBills.Objetos;
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
    /// Interaction logic for ManterPoupancas.xaml
    /// </summary>
    public partial class ManterPoupancas : UserControl
    {
        List<Poupanca> poup = new List<Poupanca>();
        
        Conversores c = new Conversores();
        private string connstring = String.Format("Server={0};Port={1};" +
           "User Id={2};Password={3};Database={4};",
           "localhost", "5432", "postgres",
           "12345", "DUDEBILLS");

        public ManterPoupancas()
        {
            InitializeComponent();
            poup = ObterSaldosPoupancas();
            PreencherCamposTela();
        }

        #region Métodos Privados

        private List<Poupanca> ObterSaldosPoupancas()
        {
            List<Poupanca> lista = new List<Poupanca>();
            DataSet dsPoupancas = new DataSet();
            string queryConsultaPoupancas = "SELECT * FROM \"POUPANCAS\";";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            NpgsqlDataAdapter daTpEntrada = new NpgsqlDataAdapter(queryConsultaPoupancas, conn);
            dsPoupancas.Reset();
            daTpEntrada.Fill(dsPoupancas);
            conn.Close();

            for (int i = 0; i < dsPoupancas.Tables[0].Rows.Count; i++)
            {
                Poupanca p = new Poupanca(dsPoupancas.Tables[0].Rows[i].Field<string>("BANCO"),
                                    dsPoupancas.Tables[0].Rows[i].Field<decimal>("VALOR_SALDO").ToString(),
                                    dsPoupancas.Tables[0].Rows[i].Field<DateTime>("DATA_ULTIMA_POSICAO").ToString(),
                                    dsPoupancas.Tables[0].Rows[i].Field<int>("NUM_AGENCIA").ToString(),
                                    dsPoupancas.Tables[0].Rows[i].Field<int>("NUM_CONTA").ToString());
                lista.Add(p);
            }

            return lista;
        }

        private void PreencherCamposTela()
        {
            decimal contador = 0;
            decimal valor = 0;

            foreach(Poupanca p in poup)
            {
                DateTime dt = new DateTime();
                DateTime.TryParse(p.UltimaAtualizacao, out dt);

                if(p.Nome.Equals("CAIXA"))
                {
                    tbSaldoCaixa.Text = "R$" + c.ConverterParaDinheiro(p.Valor);
                    tbUltimaAtualizacaoCaixa.Text = string.Concat("Última atualização: ",dt.ToShortDateString());
                }
                else if(p.Nome.Equals("BB"))
                {
                    tbSaldoBB.Text = "R$" + c.ConverterParaDinheiro(p.Valor);
                    tbUltimaAtualizacaoBB.Text = string.Concat("Última atualização: ", dt.ToShortDateString());
                }
                else if(p.Nome.Equals("ITAU"))
                {
                    tbSaldoItau.Text = "R$" + c.ConverterParaDinheiro(p.Valor);
                    tbUltimaAtualizacaoItau.Text = string.Concat("Última atualização: ", dt.ToShortDateString());
                }
                else if(p.Nome.Equals("BRADESCO"))
                {
                    tbSaldoBradesco.Text = "R$" + c.ConverterParaDinheiro(p.Valor);
                    tbUltimaAtualizacaoBradesco.Text = string.Concat("Última atualização: ", dt.ToShortDateString());
                }
                else if (p.Nome.Equals("FGTS"))
                {
                    tbSaldoFGTS.Text = "R$" + c.ConverterParaDinheiro(p.Valor);
                    tbUltimaAtualizacaoFGTS.Text = string.Concat("Última atualização: ", dt.ToShortDateString());
                }

                decimal.TryParse(p.Valor, out valor);
                contador += valor;
            }

            tbSaldoTotal.Text = "R$" + c.ConverterParaDinheiro(contador.ToString());
        }

        private bool UpdateSaldo(string query_)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            // Making connection with Npgsql provider
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            try
            {
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query_, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since we only showing the result we don't need connection anymore
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Eventos

        private void btnAddBB_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbAddSaldoBB.IsEnabled = true;
            btnSalvar.IsEnabled = true;
        }

        private void btnAddCaixa_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbAddSaldoCaixa.IsEnabled = true;
            btnSalvar.IsEnabled = true;
        }

        private void btnAddItau_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbAddSaldoItau.IsEnabled = true;
            btnSalvar.IsEnabled = true;
        }

        private void btnAddBradesco_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbAddSaldoBradesco.IsEnabled = true;
            btnSalvar.IsEnabled = true;
        }

        private void btnAddFGTS_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbAddSaldoFGTS.IsEnabled = true;
            btnSalvar.IsEnabled = true;
        }

        private void btnSalvar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            decimal saldoCaixa = 0;
            decimal saldoAdicionalCaixa = 0;
            decimal.TryParse(tbSaldoCaixa.Text.Replace("R$","").Replace(",","."),out saldoCaixa);
            decimal.TryParse(tbAddSaldoCaixa.Text.Replace("$",""),out saldoAdicionalCaixa);

            decimal saldoBB = 0;
            decimal saldoAdicionalBB = 0;
            decimal.TryParse(tbSaldoBB.Text.Replace("R$", "").Replace(",", "."), out saldoBB);
            decimal.TryParse(tbAddSaldoBB.Text.Replace("$",""),out saldoAdicionalBB);

            decimal saldoItau = 0;
            decimal saldoAdicionalItau = 0;
            decimal.TryParse(tbSaldoItau.Text.Replace("R$", "").Replace(",", "."), out saldoItau);
            decimal.TryParse(tbAddSaldoItau.Text.Replace("$",""),out saldoAdicionalItau);

            decimal saldoBradesco = 0;
            decimal saldoAdicionalBradesco = 0;
            decimal.TryParse(tbSaldoBradesco.Text.Replace("R$", "").Replace(",", "."), out saldoBradesco);
            decimal.TryParse(tbAddSaldoBradesco.Text.Replace("$",""), out saldoAdicionalBradesco);

            decimal saldoFGTS = 0;
            decimal saldoAdicionalFGTS = 0;
            decimal.TryParse(tbSaldoFGTS.Text.Replace("R$", "").Replace(",", "."), out saldoFGTS);
            decimal.TryParse(tbAddSaldoFGTS.Text.Replace("$", ""), out saldoAdicionalFGTS);

            string queryUpdatePoupancaCaixa = "UPDATE \"POUPANCAS\" SET "+
                "\"VALOR_SALDO\" ='"+(saldoCaixa+saldoAdicionalCaixa).ToString() +"',"+
                "\"DATA_ULTIMA_POSICAO\"='"+DateTime.Today.ToShortDateString()+"'"+  
                "WHERE \"BANCO\"='CAIXA';";

            string queryUpdatePoupancaBB = "UPDATE \"POUPANCAS\" SET "+
                "\"VALOR_SALDO\" ='" + (saldoBB + saldoAdicionalBB).ToString() + "'," +
                "\"DATA_ULTIMA_POSICAO\"='"+DateTime.Today.ToShortDateString()+"'"+  
                "WHERE \"BANCO\"='BB';";

            string queryUpdatePoupancaItau = "UPDATE \"POUPANCAS\" SET " +
                "\"VALOR_SALDO\" ='" + (saldoItau + saldoAdicionalItau).ToString() + "'," +
                "\"DATA_ULTIMA_POSICAO\"='" + DateTime.Today.ToShortDateString() + "'" +
                "WHERE \"BANCO\"='ITAU';";

            string queryUpdatePoupancaBradesco = "UPDATE \"POUPANCAS\" SET " +
                "\"VALOR_SALDO\" ='" + (saldoBradesco + saldoAdicionalBradesco).ToString() + "'," +
                "\"DATA_ULTIMA_POSICAO\"='" + DateTime.Today.ToShortDateString() + "'" +
                "WHERE \"BANCO\"='BRADESCO';";

            string queryUpdatePoupancaFGTS = "UPDATE \"POUPANCAS\" SET " +
                "\"VALOR_SALDO\" ='" + (saldoFGTS + saldoAdicionalFGTS).ToString() + "'," +
                "\"DATA_ULTIMA_POSICAO\"='" + DateTime.Today.ToShortDateString() + "'" +
                "WHERE \"BANCO\"='FGTS';";

            if(UpdateSaldo(queryUpdatePoupancaCaixa) &&
            UpdateSaldo(queryUpdatePoupancaBB) &&
            UpdateSaldo(queryUpdatePoupancaItau) &&
            UpdateSaldo(queryUpdatePoupancaBradesco)&&
            UpdateSaldo(queryUpdatePoupancaFGTS))
            {
                Sucesso s = new Sucesso("Saldos atualizados com sucesso!");
                s.Background = Brushes.White;
                s.Show();
            }
            else
            {
                Erro erro = new Erro("Erro na atualização");
                erro.Background = Brushes.White;
                erro.Show();
            }
        }

        #endregion


    }
}
