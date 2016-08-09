using Microsoft.Windows.Controls.Primitives;
using Npgsql;
using DoTheBills.Gerenciadores;
using DoTheBills.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Calendario_1.xaml
    /// </summary>
    public partial class Calendario : UserControl
    {
        #region Atributos

        List<string> months = new List<string> { "JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO", "JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO" };
        // PostgeSQL-style connection string
        private string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "localhost", "5432", "postgres",
                   "12345", "DUDEBILLS");
        private Conversores conversores = new Conversores();
        ExcluirPagamentoEventArgs epea;

        #endregion

        public Calendario()
        {
            InitializeComponent();
            
            DateTime d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            MontarDisposicaoDias(d.DayOfWeek);
            CarregaMesAnoAtual();
            AplicarEstiloDias();
            Loaded += WindowLoaded;
        }

        #region Métodos Privados

        private void MontarDisposicaoDias(DayOfWeek diaDaSemana)
        {
            int[] dias = new int[42];
            int dia = (int)diaDaSemana;

            switch (dia)
            {
                case 0: // domingo
                {
                    dias[0] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 1: // segunda-feira
                {
                    dias[0] = ObterDiasMesAnterior();
                    dias[1] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 2: // terça-feira
                {
                    dias[0] = ObterDiasMesAnterior()-1;
                    dias[1] = ObterDiasMesAnterior();
                    dias[2] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 3: //quarta-feira
                {
                    dias[0] = ObterDiasMesAnterior() - 2;
                    dias[1] = ObterDiasMesAnterior() - 1;
                    dias[2] = ObterDiasMesAnterior();
                    dias[3] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 4: //quinta-feira
                {
                    dias[0] = ObterDiasMesAnterior() - 3;
                    dias[1] = ObterDiasMesAnterior() - 2;
                    dias[2] = ObterDiasMesAnterior() - 1;
                    dias[3] = ObterDiasMesAnterior();
                    dias[4] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 5: // sexta-feira
                {
                    dias[0] = ObterDiasMesAnterior() - 4;
                    dias[1] = ObterDiasMesAnterior() - 3;
                    dias[2] = ObterDiasMesAnterior() - 2;
                    dias[3] = ObterDiasMesAnterior() - 1;
                    dias[4] = ObterDiasMesAnterior();
                    dias[5] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }
                case 6: // sábado
                {
                    dias[0] = ObterDiasMesAnterior() - 5;
                    dias[1] = ObterDiasMesAnterior() - 4;
                    dias[2] = ObterDiasMesAnterior() - 3;
                    dias[3] = ObterDiasMesAnterior() - 2;
                    dias[4] = ObterDiasMesAnterior() - 1;
                    dias[5] = ObterDiasMesAnterior();
                    dias[6] = 1;
                    SetarDiasCalendario(dias);
                    break;
                }

            }
        }

        private int ObterDiasMesAnterior()
        {
            int mesAnterior = DateTime.Today.Month - 1;
            int quantidadeDiasMesAnterior = 0;

            if(mesAnterior != 12)
            {
                quantidadeDiasMesAnterior = DateTime.DaysInMonth(DateTime.Today.Year, mesAnterior);
            }
            else
            {
                quantidadeDiasMesAnterior = DateTime.DaysInMonth(DateTime.Today.Year - 1, mesAnterior);
            }

            return quantidadeDiasMesAnterior;
        }

        private void SetarDiasCalendario(int [] dias)
        {
            int quantidadeDiasMesAtual = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            int contadorDias = 2;
            int posicaoDia1 = 0;
            List<string> [] descricoesPagamentos = new List<string> [42];

            //Preenche a lista com os dias do mês atual
            for (int i = 0; i <= dias.Length; i++)
            {
                if (dias[i] == 0)
                {
                    dias[i] = dias[i - 1] + 1;
                    contadorDias++;
                    if (dias[i] == 2 && posicaoDia1==0)
                    {
                        posicaoDia1 = i-1;
                    }
                }

                if (contadorDias > quantidadeDiasMesAtual)
                {
                    break;
                }
            }

            //Complementa a lista com os dias do início do mês seguinte
            int contador = 1;
            for (int j = 0; j < dias.Length; j++)
            {
                if(dias[j] == 0)
                {
                    dias[j] = contador;
                    contador++;
                }
            }

            descricoesPagamentos = CarregaMovimentacaoMes(posicaoDia1);

            d11_dia.Text = dias[0].ToString();
            d11_pgtos.ItemsSource = descricoesPagamentos[0];
            
            d12_dia.Text = dias[1].ToString();
            d12_pgtos.ItemsSource = descricoesPagamentos[1];

            d13_dia.Text = dias[2].ToString();
            d13_pgtos.ItemsSource = descricoesPagamentos[2];

            d14_dia.Text = dias[3].ToString();
            d14_pgtos.ItemsSource = descricoesPagamentos[3];

            d15_dia.Text = dias[4].ToString();
            d15_pgtos.ItemsSource = descricoesPagamentos[4];

            d16_dia.Text = dias[5].ToString();
            d16_pgtos.ItemsSource = descricoesPagamentos[5];

            d17_dia.Text = dias[6].ToString();
            d17_pgtos.ItemsSource = descricoesPagamentos[6];

            d21_dia.Text = dias[7].ToString();
            d21_pgtos.ItemsSource = descricoesPagamentos[7];

            d22_dia.Text = dias[8].ToString();
            d22_pgtos.ItemsSource = descricoesPagamentos[8];

            d23_dia.Text = dias[9].ToString();
            d23_pgtos.ItemsSource = descricoesPagamentos[9];

            d24_dia.Text = dias[10].ToString();
            d24_pgtos.ItemsSource = descricoesPagamentos[10];

            d25_dia.Text = dias[11].ToString();
            d25_pgtos.ItemsSource = descricoesPagamentos[11];

            d26_dia.Text = dias[12].ToString();
            d26_pgtos.ItemsSource = descricoesPagamentos[12];

            d27_dia.Text = dias[13].ToString();
            d27_pgtos.ItemsSource = descricoesPagamentos[13];

            d31_dia.Text = dias[14].ToString();
            d31_pgtos.ItemsSource = descricoesPagamentos[14];

            d32_dia.Text = dias[15].ToString();
            d32_pgtos.ItemsSource = descricoesPagamentos[15];

            d33_dia.Text = dias[16].ToString();
            d33_pgtos.ItemsSource = descricoesPagamentos[16];

            d34_dia.Text = dias[17].ToString();
            d34_pgtos.ItemsSource = descricoesPagamentos[17];

            d35_dia.Text = dias[18].ToString();
            d35_pgtos.ItemsSource = descricoesPagamentos[18];

            d36_dia.Text = dias[19].ToString();
            d36_pgtos.ItemsSource = descricoesPagamentos[19];

            d37_dia.Text = dias[20].ToString();
            d37_pgtos.ItemsSource = descricoesPagamentos[20];

            d41_dia.Text = dias[21].ToString();
            d41_pgtos.ItemsSource = descricoesPagamentos[21];

            d42_dia.Text = dias[22].ToString();
            d42_pgtos.ItemsSource = descricoesPagamentos[22];

            d43_dia.Text = dias[23].ToString();
            d43_pgtos.ItemsSource = descricoesPagamentos[23]; 

            d44_dia.Text = dias[24].ToString();
            d44_pgtos.ItemsSource = descricoesPagamentos[24];

            d45_dia.Text = dias[25].ToString();
            d45_pgtos.ItemsSource = descricoesPagamentos[25];

            d46_dia.Text = dias[26].ToString();
            d46_pgtos.ItemsSource = descricoesPagamentos[26];

            d47_dia.Text = dias[27].ToString();
            d47_pgtos.ItemsSource = descricoesPagamentos[27];

            d51_dia.Text = dias[28].ToString();
            d51_pgtos.ItemsSource = descricoesPagamentos[28];

            d52_dia.Text = dias[29].ToString();
            d52_pgtos.ItemsSource = descricoesPagamentos[29];

            d53_dia.Text = dias[30].ToString();
            d53_pgtos.ItemsSource = descricoesPagamentos[30];

            d54_dia.Text = dias[31].ToString();
            d54_pgtos.ItemsSource = descricoesPagamentos[31];

            d55_dia.Text = dias[32].ToString();
            d55_pgtos.ItemsSource = descricoesPagamentos[32];

            d56_dia.Text = dias[33].ToString();
            d56_pgtos.ItemsSource = descricoesPagamentos[33];

            d57_dia.Text = dias[34].ToString();
            d57_pgtos.ItemsSource = descricoesPagamentos[34];

            d61_dia.Text = dias[35].ToString();
            d61_pgtos.ItemsSource = descricoesPagamentos[35];

            d62_dia.Text = dias[36].ToString();
            d62_pgtos.ItemsSource = descricoesPagamentos[36];

            d63_dia.Text = dias[37].ToString();
            d63_pgtos.ItemsSource = descricoesPagamentos[37];

            d64_dia.Text = dias[38].ToString();
            d64_pgtos.ItemsSource = descricoesPagamentos[38];

            d65_dia.Text = dias[39].ToString();
            d65_pgtos.ItemsSource = descricoesPagamentos[39];

            d66_dia.Text = dias[40].ToString();
            d66_pgtos.ItemsSource = descricoesPagamentos[40];

            d67_dia.Text = dias[41].ToString();
            d67_pgtos.ItemsSource = descricoesPagamentos[41];
        }

        private void CarregaMesAnoAtual()
        {
            mesAtual.Text = months[DateTime.Today.Month-1];
            anoAtual.Text = DateTime.Today.Year.ToString();
        }

        private void ObtemItemParaExclusao(object sender)
        {
            DataGrid dg = new DataGrid();
            dg = (DataGrid)sender;
            if (dg.Items.Count > 0)
            {
                List<string> itens = new List<string>();

                for (int i = 0; i < dg.Items.Count; i++)
                {
                    itens.Add(dg.Items[i].ToString());
                }

                if (itens.Count > 0)
                {
                    ExcluirPagamentos ep = new ExcluirPagamentos(itens);
                    ep.Show();
                }
            }
        }

        private List<string> [] CarregaMovimentacaoMes(int posicaoDia1_)
        {
            List<string> [] listaMovimentacoesMes = new List<string> [42];
            List<Movimentacao> lista = new List<Movimentacao>();
            int mes = 0;
            Movimentacao p;
            string mestring = string.Empty;

            //Consulta Saídas
            DataSet dsPagamentos = new DataSet();
            DataTable dtPagamentos = new DataTable();
            string queryConsultaPagamentos = "SELECT * FROM \"PAGAMENTOS\";";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            NpgsqlDataAdapter daTpPag = new NpgsqlDataAdapter(queryConsultaPagamentos, conn);
            dsPagamentos.Reset();
            daTpPag.Fill(dsPagamentos);
            conn.Close();

            //Consulta Entradas
            DataSet dsEntradas = new DataSet();
            DataTable dtEntradas = new DataTable();
            string queryConsultaEntradas = "SELECT * FROM \"ENTRADAS\";";
            NpgsqlConnection conn2 = new NpgsqlConnection(connstring);
            conn2.Open();
            NpgsqlDataAdapter daTpEntrada = new NpgsqlDataAdapter(queryConsultaEntradas, conn2);
            dsEntradas.Reset();
            daTpEntrada.Fill(dsEntradas);
            conn2.Close();

            //Preenche lista com as saídas
            for (int i = 0;  i < dsPagamentos.Tables[0].Rows.Count; i++)
            {
                mes = 0;
                mestring = dsPagamentos.Tables[0].Rows[i].Field<string>("MES_PAGAMENTO");
                int.TryParse(mestring, out mes);

                if (mes.Equals(DateTime.Today.Month) || mes.Equals(0))
                {
                    p = new Movimentacao(dsPagamentos.Tables[0].Rows[i].Field<int>("DIA_PAGAMENTO").ToString(),
                                      dsPagamentos.Tables[0].Rows[i].Field<string>("DSC_PAGAMENTO"),
                                      conversores.ConverterParaDinheiro(dsPagamentos.Tables[0].Rows[i].Field<decimal>("VLR_PAGAMENTO").ToString()),
                                      "[-] ");
                    lista.Add(p);
                }
                lista = lista.OrderBy(o => o.DiaMovimentacao).ToList<Movimentacao>();
            }

            //Preenche lista com as entradas
            for(int j = 0; j < dsEntradas.Tables[0].Rows.Count; j++)
            {
                mes = 0;
                p = null;
                mestring = dsEntradas.Tables[0].Rows[j].Field<string>("MES_ENTRADA");
                int.TryParse(mestring, out mes);

                if (mes.Equals(DateTime.Today.Month) || mes.Equals(0))
                {
                    p = new Movimentacao(dsEntradas.Tables[0].Rows[j].Field<int>("DIA_ENTRADA").ToString(),
                                      dsEntradas.Tables[0].Rows[j].Field<string>("DESC_ENTRADA"),
                                      conversores.ConverterParaDinheiro(dsEntradas.Tables[0].Rows[j].Field<decimal>("VALOR_ENTRADA").ToString()),
                                      "[+] ");
                    lista.Add(p);
                }
                lista = lista.OrderBy(o => o.DiaMovimentacao).ToList<Movimentacao>();

            }

            string diaAnterior = string.Empty;
            List<string> pagamentosDia = new List<string>();
            List<string> pagamentosSemDiaDefinido = new List<string>();

            //Popula os dias do calendário com as movimentações
            foreach(Movimentacao mov in lista)
            {
                string diaAtual = mov.DiaMovimentacao;

                if (!diaAtual.Equals("0"))
                {
                    if (diaAnterior.Equals(diaAtual) || string.IsNullOrEmpty(diaAnterior))
                    {
                        pagamentosDia.Add(string.Concat(mov.Sinal,mov.DescricaoMovimentacao, " | R$ ", mov.Valor));
                    }
                    else
                    {
                        int dia = 0;
                        int.TryParse(diaAnterior, out dia);
                        listaMovimentacoesMes[posicaoDia1_ + dia - 1] = pagamentosDia;
                        pagamentosDia = null;
                        pagamentosDia = new List<string>();
                        pagamentosDia.Add(string.Concat(mov.Sinal, mov.DescricaoMovimentacao, " | R$ ", mov.Valor));

                        if(mov.Equals(lista.Last()))
                        {
                            dia = 0;
                            int.TryParse(diaAtual, out dia);
                            listaMovimentacoesMes[posicaoDia1_ + dia - 1] = pagamentosDia;
                            pagamentosDia = null;
                        }
                    }
                    diaAnterior = diaAtual;
                }
                else
                {
                    pagamentosSemDiaDefinido.Add(string.Concat(mov.Sinal, mov.DescricaoMovimentacao, " | R$ ", mov.Valor));
                }
            }

            pagamentosSemData.ItemsSource = pagamentosSemDiaDefinido;

            return listaMovimentacoesMes;
        }

        private void AplicarEstiloHoje(int dia, DataGrid dg)
        {
            int hoje = DateTime.Today.Day;
            if(dia == hoje)
            {
                dg.Background = Brushes.Snow;
            }
            
        }

        private void AplicarEstiloDias()
        {
            //Se dia 1 cai no domingo
            if(d11_dia.Text.Equals("1"))
            {
                if(d51_dia.Text.Equals("1"))
                {
                    d51.Background = Brushes.DarkGray;
                    d52.Background = Brushes.DarkGray;
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if(d52_dia.Text.Equals("1"))
                {
                    d52.Background = Brushes.DarkGray;
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if(d53_dia.Text.Equals("1"))
                {
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai na segunda
            else if(d12_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;

                if (d52_dia.Text.Equals("1"))
                {
                    d52.Background = Brushes.DarkGray;
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d53_dia.Text.Equals("1"))
                {
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if(d54_dia.Text.Equals("1"))
                {
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai na terça
            else if (d13_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;
                d12.Background = Brushes.DarkGray;

                if (d53_dia.Text.Equals("1"))
                {
                    d53.Background = Brushes.DarkGray;
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d54_dia.Text.Equals("1"))
                {
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d55_dia.Text.Equals("1"))
                {
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai na quarta
            else if (d14_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;
                d12.Background = Brushes.DarkGray;
                d13.Background = Brushes.DarkGray;

                if (d54_dia.Text.Equals("1"))
                {
                    d54.Background = Brushes.DarkGray;
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d55_dia.Text.Equals("1"))
                {
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d56_dia.Text.Equals("1"))
                {
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai na quinta
            else if (d15_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;
                d12.Background = Brushes.DarkGray;
                d13.Background = Brushes.DarkGray;
                d14.Background = Brushes.DarkGray;

                if (d55_dia.Text.Equals("1"))
                {
                    d55.Background = Brushes.DarkGray;
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d56_dia.Text.Equals("1"))
                {
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d57_dia.Text.Equals("1"))
                {
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai na sexta
            else if (d16_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;
                d12.Background = Brushes.DarkGray;
                d13.Background = Brushes.DarkGray;
                d14.Background = Brushes.DarkGray;
                d15.Background = Brushes.DarkGray;

                if (d56_dia.Text.Equals("1"))
                {
                    d56.Background = Brushes.DarkGray;
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d57_dia.Text.Equals("1"))
                {
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d61_dia.Text.Equals("1"))
                {
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
            //Se dia 1 cai no sábado
            else if (d17_dia.Text.Equals("1"))
            {
                d11.Background = Brushes.DarkGray;
                d12.Background = Brushes.DarkGray;
                d13.Background = Brushes.DarkGray;
                d14.Background = Brushes.DarkGray;
                d15.Background = Brushes.DarkGray;
                d16.Background = Brushes.DarkGray;

                if (d57_dia.Text.Equals("1"))
                {
                    d57.Background = Brushes.DarkGray;
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d61_dia.Text.Equals("1"))
                {
                    d61.Background = Brushes.DarkGray;
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else if (d62_dia.Text.Equals("1"))
                {
                    d62.Background = Brushes.DarkGray;
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
                else
                {
                    d63.Background = Brushes.DarkGray;
                    d64.Background = Brushes.DarkGray;
                    d65.Background = Brushes.DarkGray;
                    d66.Background = Brushes.DarkGray;
                    d67.Background = Brushes.DarkGray;
                }
            }
        }

        private void UpdateRowColor(DataGrid dGrid)
        {
            for (int i = 0; i < dGrid.Items.Count; ++i)
            {
                DataGridRow row = GetRow(dGrid, i);

                string registry = (string)row.Item;

                for (int j = 0; j < dGrid.Columns.Count; ++j)
                {
                    if (registry.Contains("[+]"))
                        row.Foreground = Brushes.Green;
                    else
                        row.Foreground = Brushes.Red;
                }
            }
        }

        private DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index); //Items[index];
            if (row == null)
            {
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                dataGrid.UpdateLayout();
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetVisualChild<T>(v);
                if (child != null)
                    break;
            }
            return child;
        }


        #endregion 

        #region Eventos

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            UpdateRowColor(d11_pgtos);
            UpdateRowColor(d12_pgtos);
            UpdateRowColor(d13_pgtos);
            UpdateRowColor(d14_pgtos);
            UpdateRowColor(d15_pgtos);
            UpdateRowColor(d16_pgtos);
            UpdateRowColor(d17_pgtos);
            UpdateRowColor(d21_pgtos);
            UpdateRowColor(d22_pgtos);
            UpdateRowColor(d23_pgtos);
            UpdateRowColor(d24_pgtos);
            UpdateRowColor(d25_pgtos);
            UpdateRowColor(d26_pgtos);
            UpdateRowColor(d27_pgtos);
            UpdateRowColor(d31_pgtos);
            UpdateRowColor(d32_pgtos);
            UpdateRowColor(d33_pgtos);
            UpdateRowColor(d34_pgtos);
            UpdateRowColor(d35_pgtos);
            UpdateRowColor(d36_pgtos);
            UpdateRowColor(d37_pgtos);
            UpdateRowColor(d41_pgtos);
            UpdateRowColor(d42_pgtos);
            UpdateRowColor(d43_pgtos);
            UpdateRowColor(d44_pgtos);
            UpdateRowColor(d45_pgtos);
            UpdateRowColor(d46_pgtos);
            UpdateRowColor(d47_pgtos);
            UpdateRowColor(d51_pgtos);
            UpdateRowColor(d52_pgtos);
            UpdateRowColor(d53_pgtos);
            UpdateRowColor(d54_pgtos);
            UpdateRowColor(d55_pgtos);
            UpdateRowColor(d56_pgtos);
            UpdateRowColor(d57_pgtos);
            UpdateRowColor(d61_pgtos);
            UpdateRowColor(d62_pgtos);
            UpdateRowColor(d63_pgtos);
            UpdateRowColor(d64_pgtos);
            UpdateRowColor(d65_pgtos);
            UpdateRowColor(d66_pgtos);
            UpdateRowColor(d67_pgtos);
        }

        private void Dia_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ObtemItemParaExclusao(sender);
        }

        #endregion
    }
}
