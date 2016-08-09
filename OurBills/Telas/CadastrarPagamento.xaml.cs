using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for CadastrarPagamento.xaml
    /// </summary>
    public partial class CadastrarPagamento : UserControl
    {
        #region Atributos

        private string _mesCorrente;
        private string _tipoPagamentoSelecionado;
        private string _responsavelSelecionado;
        private string valorPgto;
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker bw = new BackgroundWorker();
        private BrushConverter converter = new BrushConverter();
        
        // PostgeSQL-style connection string
        private string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "localhost", "5432", "postgres",
                   "12345", "DUDEBILLS");
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        Processando p; 

        #endregion

        #region Propriedades

        public string MesCorrente
        {
            get 
            { 
                return _mesCorrente; 
            }
            set 
            { 
                _mesCorrente = value;
                
            }
        }

        public string TipoPagamentoSelecionado
        {
            get { return _tipoPagamentoSelecionado; }
            set { _tipoPagamentoSelecionado = value; }
        }

        public string ResponsavelSelecionado
        {
            get { return _responsavelSelecionado; }
            set 
            { 
                _responsavelSelecionado = value;
                OnPropertyChanged("ResponsavelSelecionado");
            }
        }



        #endregion

        #region Construtor

        public CadastrarPagamento()
        {
            InitializeComponent();
            ConverterMes(DateTime.Today.Month);
            PopularComboTiposPagamentos();
            PopularComboResponsaveis();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }  

        #endregion

        #region Execuções SQL

        private bool SalvarPagamento()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                DataRowView drv1 = (DataRowView)cbResponsavel.SelectedItem;
                string id_usuario = drv1.DataView[cbResponsavel.SelectedIndex].Row["ID_USUARIO"].ToString();

                DataRowView drv2 = (DataRowView)cbTipoPagamento.SelectedItem;
                string id_tipo_pag = drv2.DataView[cbTipoPagamento.SelectedIndex].Row["ID_TIPO_PAGAMENTO"].ToString();

                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                // quite complex sql statement

                string queryInsertPagamento = "INSERT INTO \"PAGAMENTOS\"(" +
                "\"DSC_PAGAMENTO\", \"DIA_PAGAMENTO\", \"ID_TIPO_PAGAMENTO\"," +
                "\"ID_RESPONSAVEL\", \"VLR_PAGAMENTO\", \"IND_RECORRENTE\", \"IND_CALENDARIO_OUTLOOK\", \"DATA_LIMITE_RECORRENCIA\", \"MES_PAGAMENTO\")" +
                "VALUES ('" + tbDescricao.Text + "'," +
                        "'" + (cbDiaPagamento.SelectedIndex + 1)+ "'," +
                        "'" + id_tipo_pag + "'," +
                        "'" + id_usuario + "'," +
                        "'" + tbValor.Number.ToString().Replace(",", "").Replace(".", "") + "'" + "," + 
                        "'" + DeParaBooleano(cbRecorrente.SelectedIndex) + "'," +
                        "'" + false + "'," +
                        //"'" + DeParaBooleano(cbOutlook.SelectedIndex) + "',"+
                        "'" + (string.IsNullOrEmpty(dpDataLimite.Text) ? "1900-01-01" : dpDataLimite.Text) + "'," +
                        "'" + (DeParaBooleano(cbRecorrente.SelectedIndex).Equals("TRUE") ? string.Empty : DateTime.Today.Month.ToString()) + "');";

                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryInsertPagamento, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since we only showing the result we don't need connection anymore
                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void PopularComboTiposPagamentos()
        {
            DataSet datasetTipoPag = new DataSet();
            DataTable datatableTipoPag = new DataTable();

            string queryConsultaTiposPagamento = "SELECT * FROM \"TIPOS_PAGAMENTOS\";";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            NpgsqlDataAdapter daTpPag = new NpgsqlDataAdapter(queryConsultaTiposPagamento, conn);

            datasetTipoPag.Reset();
            daTpPag.Fill(datasetTipoPag);
            datatableTipoPag = datasetTipoPag.Tables[0];
            cbTipoPagamento.DataContext = datatableTipoPag;
            cbTipoPagamento.DisplayMemberPath = datasetTipoPag.Tables[0].Columns[0].ToString();
            conn.Close();
        }

        private void PopularComboResponsaveis()
        {
            DataSet datasetResp = new DataSet();
            DataTable datatableTipoPag = new DataTable();
            string queryConsultaResponsaveis = "SELECT * FROM \"USUARIOS\";";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            NpgsqlDataAdapter daResp = new NpgsqlDataAdapter(queryConsultaResponsaveis, conn);

            datasetResp.Reset();
            daResp.Fill(datasetResp);
            datatableTipoPag = datasetResp.Tables[0];
            cbResponsavel.DataContext = datatableTipoPag;
            cbResponsavel.DisplayMemberPath = datasetResp.Tables[0].Columns[0].ToString();

            conn.Close();
        }

        #endregion

        #region Eventos

        private void btnSalvar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void cbResponsavel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbResponsavel.SelectedIndex != -1)
            {
                DataRowView drv = (DataRowView)cbResponsavel.SelectedItem;
                ResponsavelSelecionado = drv.DataView[cbResponsavel.SelectedIndex].Row[1].ToString();
            }
        }

        private void OnPropertyChanged(string p)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }

        private void cbRecorrente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRecorrente.SelectedIndex != -1)
            {
                if (cbRecorrente.SelectedIndex.Equals(0))
                {
                    tbDataLimite.Visibility = Visibility.Visible;
                    dpDataLimite.Visibility = Visibility.Visible;
                    tbObs.Visibility = Visibility.Visible;
                }
                else
                {
                    tbDataLimite.Visibility = Visibility.Collapsed;
                    dpDataLimite.Visibility = Visibility.Collapsed;
                    tbObs.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                p = new Processando();
                p.Show();
                System.Threading.Thread.Sleep(3000);
            });
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                p.Close();

                if (ValidarPreenchimentoCampos())
                {
                    if (SalvarPagamento())
                    {
                        Sucesso s = new Sucesso("Pagamento cadastrado com sucesso!");
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
            });
        }

        private void btnLimpar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbDescricao.Text = string.Empty;
            tbValor.Number = 0;
            cbDiaPagamento.SelectedIndex = -1;
            cbRecorrente.SelectedIndex = -1;
            cbResponsavel.SelectedIndex = -1;
            cbTipoPagamento.SelectedIndex = -1;
            tbDataLimite.Text = string.Empty;
            tbObs.Text = string.Empty;

            tbDescricao.BorderBrush = converter.ConvertFromString("Black") as Brush;
            tbDescricao.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            tbValor.BorderBrush = converter.ConvertFromString("Red") as Brush;
            tbValor.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            cbResponsavel.BorderBrush = converter.ConvertFromString("Black") as Brush;
            cbResponsavel.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
        }

        private void tbValor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbValor.Focus();
        }

        #endregion

        #region Métodos Privados Auxiliares

        private string ConverterMes(int mes)
        {
            string mesSaida = string.Empty;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            switch(mes)
            {
                case 1:
                {
                    mesSaida = "JANEIRO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/"+mesSaida+".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 2:
                {
                    mesSaida = "FEVEREIRO";
                    PopularComboDias(29);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 3:
                {
                    mesSaida = "MARÇO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 4:
                {
                    mesSaida = "ABRIL";
                    PopularComboDias(30);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 5:
                {
                    mesSaida = "MAIO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 6:
                {
                    mesSaida = "JUNHO";
                    PopularComboDias(30);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 7:
                {
                    mesSaida = "JULHO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 8:
                {
                    mesSaida = "AGOSTO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 9:
                {
                    mesSaida = "SETEMBRO";
                    PopularComboDias(30);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 10:
                {
                    mesSaida = "OUTUBRO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 11:
                {
                    mesSaida = "NOVEMBRO";
                    PopularComboDias(30);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
                case 12:
                {
                    mesSaida = "DEZEMBRO";
                    PopularComboDias(31);
                    logo.UriSource = new Uri("../Imagens/" + mesSaida + ".png", UriKind.Relative);
                    logo.EndInit();
                    if (logo != null)
                        imgCabecalho.Source = logo;
                    break;
                }
            }

            return mesSaida;
        }

        private bool ValidarPreenchimentoCampos()
        {
            bool resultado = true;

            if (string.IsNullOrEmpty(tbDescricao.Text))
            {
                resultado = false;
                tbDescricao.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbDescricao.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbDescricao.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbDescricao.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if (tbValor.Number == 0)
            {
                resultado = false;
                tbValor.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbValor.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbValor.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbValor.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if(cbResponsavel.SelectedIndex == -1)
            {
                resultado = false;
                cbResponsavel.BorderBrush = converter.ConvertFromString("Red") as Brush;
                cbResponsavel.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                cbResponsavel.BorderBrush = converter.ConvertFromString("Black") as Brush;
                cbResponsavel.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            return resultado;
        }
    
        private void PopularComboDias(int quantidadeDias)
        {
            List<int> dias = new List<int>();

            for(int i = 1; i <= quantidadeDias; i++)
            {
                dias.Add(i);
            }

            cbDiaPagamento.ItemsSource = dias;

        }

        private string DeParaBooleano(int index)
        {
            if(index == 0)
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";
            }
        }



        #endregion

    }
}
