using iTextSharp.text;
using iTextSharp.text.pdf;
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
using System.Windows.Shapes;

namespace DoTheBills.Telas
{
    /// <summary>
    /// Interaction logic for SelecaoGeracaoRelatorio.xaml
    /// </summary>
    public partial class SelecaoGeracaoRelatorio : Window
    {
        List<string> months = new List<string> { "JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO", "JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO" };
        private Conversores conversores = new Conversores();
        // Set up the fonts to be used on the pages 
        private Font _largeFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.BLACK);
        private Font _header = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.BLACK);
        private Font _standardFont = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL, BaseColor.BLACK);
        private Font _smallFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

        // PostgeSQL-style connection string
        private string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "localhost", "5432", "postgres",
                   "12345", "DUDEBILLS");

        public SelecaoGeracaoRelatorio()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            cbMeses.ItemsSource = months;
        }

        #region Eventos

        private void btnLogin_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<Movimentacao> movimentosMes = new List<Movimentacao>();
                movimentosMes.AddRange(ObterMovimentacoesEntrada());
                movimentosMes.AddRange(ObterMovimentacoesSaida());
                movimentosMes = movimentosMes.OrderBy(o => o.DiaMovimentacao).ToList<Movimentacao>();

                List<Poupanca> poupancas = ObterSaldosPoupancas();

                if (Build((cbMeses.SelectedIndex + 1).ToString(), DateTime.Today.Year.ToString(), movimentosMes, poupancas))
                {
                    Sucesso s = new Sucesso("Relatório gerado com sucesso!");
                    s.Background = Brushes.White;
                    s.Show();
                    this.Close();
                }
                else
                {
                    Erro erro = new Erro("Erro ao gerar relatório");
                    erro.Background = Brushes.White;
                    erro.Show();
                    this.Close();
                }

            }
            catch
            {
                Erro erro = new Erro("Erro ao gerar relatório");
                erro.Background = Brushes.White;
                erro.Show();
                this.Close();
            }
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

        #endregion

        #region Métodos Privados

        private List<Movimentacao> ObterMovimentacoesSaida()
        {
            List<Movimentacao> listaSaida = new List<Movimentacao>();

            try
            {
                //Monta lista de saídas
                DataSet dsSaidas = new DataSet();
                string queryConsultaSaidas = "SELECT * FROM \"PAGAMENTOS\";";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                NpgsqlDataAdapter daTpPag = new NpgsqlDataAdapter(queryConsultaSaidas, conn);

                dsSaidas.Reset();
                daTpPag.Fill(dsSaidas);
                conn.Close();

                for (int i = 0; i < dsSaidas.Tables[0].Rows.Count; i++)
                {
                    int mes = 0;
                    Movimentacao p;
                    string mestring = dsSaidas.Tables[0].Rows[i].Field<string>("MES_PAGAMENTO");
                    int.TryParse(mestring, out mes);

                    if (mes.Equals((cbMeses.SelectedIndex) + 1) || mes.Equals(0))
                    {
                        p = new Movimentacao(dsSaidas.Tables[0].Rows[i].Field<int>("DIA_PAGAMENTO").ToString().PadLeft(2, '0'),
                                          dsSaidas.Tables[0].Rows[i].Field<string>("DSC_PAGAMENTO"),
                                          conversores.ConverterParaDinheiro(dsSaidas.Tables[0].Rows[i].Field<decimal>("VLR_PAGAMENTO").ToString()),
                                          "[-] ",
                                          dsSaidas.Tables[0].Rows[i].Field<int>("ID_RESPONSAVEL").ToString());
                        listaSaida.Add(p);
                    }
                }
            }
            catch (Exception e)
            {
                this.Close();
                return null;
            }

            return listaSaida;
        }

        private List<Movimentacao> ObterMovimentacoesEntrada()
        {
            List<Movimentacao> listaEntrada = new List<Movimentacao>();

            try
            {
                //Monta lista de entradas
                DataSet dsEntradas = new DataSet();
                string queryConsultaEntradas = "SELECT * FROM \"ENTRADAS\";";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                NpgsqlDataAdapter daTpEntrada = new NpgsqlDataAdapter(queryConsultaEntradas, conn);

                dsEntradas.Reset();
                daTpEntrada.Fill(dsEntradas);
                conn.Close();

                for (int i = 0; i < dsEntradas.Tables[0].Rows.Count; i++)
                {
                    int mes = 0;
                    Movimentacao p;
                    string mestring = dsEntradas.Tables[0].Rows[i].Field<string>("MES_ENTRADA");
                    int.TryParse(mestring, out mes);

                    if (mes.Equals((cbMeses.SelectedIndex) + 1) || mes.Equals(0))
                    {
                        p = new Movimentacao(dsEntradas.Tables[0].Rows[i].Field<int>("DIA_ENTRADA").ToString().PadLeft(2, '0'),
                                          dsEntradas.Tables[0].Rows[i].Field<string>("DESC_ENTRADA"),
                                          conversores.ConverterParaDinheiro(dsEntradas.Tables[0].Rows[i].Field<decimal>("VALOR_ENTRADA").ToString()),
                                          "[+] ",
                                          dsEntradas.Tables[0].Rows[i].Field<int>("ID_USUARIO").ToString());
                        listaEntrada.Add(p);
                    }
                }
            }
            catch(Exception e)
            {
                this.Close();
                return null;
            }

            return listaEntrada;
        }

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

        private string SomarMovimentos(List<Movimentacao> movimentosMes_, string sinal_)
        {
            int saida = 0;
            foreach (Movimentacao mov in movimentosMes_)
            {
                if (mov.Sinal.Contains(sinal_))
                {
                    int valorpagamento = 0;
                    int.TryParse(mov.Valor.Replace(",", ""), out valorpagamento);
                    saida = saida + valorpagamento;
                }
            }

            return conversores.ConverterParaDinheiro(saida.ToString());
        }

        private string SomarPoupancas(List<Poupanca> saldos_)
        {
            int saida = 0;
            foreach (Poupanca poup in saldos_)
            {
                    int saldo = 0;
                    int.TryParse(poup.Valor.Replace(",", ""), out saldo);
                    saida = saida + saldo;
            }

            return conversores.ConverterParaDinheiro(saida.ToString());
        }

        #endregion

        #region PDF

        public bool Build(string mes, string ano, List<Movimentacao> movimentos_, List<Poupanca> poupancas_)
        {
            bool retorno = false;
            iTextSharp.text.Document doc = null;
            try
            {
                // Initialize the PDF document 
                doc = new Document();
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(System.IO.Directory.GetCurrentDirectory() + "\\Relatorio_" + mes + "_" + ano + ".pdf",
                        System.IO.FileMode.Create));
                // Set margins and page size for the document 
                doc.SetMargins(10, 10, 10, 10);
                // There are a huge number of possible page sizes, including such sizes as 
                // EXECUTIVE, LEGAL, LETTER_LANDSCAPE, and NOTE 
                doc.SetPageSize(new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.LETTER.Width,
                    iTextSharp.text.PageSize.LETTER.Height));

                // Add metadata to the document.  This information is visible when viewing the 
                // document properities within Adobe Reader. 
                doc.AddTitle("Relatorio_" + mes + "_" + ano);
                doc.AddCreator("DUDE Bill$");

                // Add Xmp metadata to the document. 
                this.CreateXmpMetadata(writer);

                // Open the document for writing content 
                doc.Open();

                // Add pages to the document 
                this.CriarCapaRelatorio(doc, mes, ano);
                this.CriarPaginaMovimentos(doc, movimentos_);
                this.CriarPaginaPoupanca(doc, poupancas_);

                // Add page labels to the document 
                iTextSharp.text.pdf.PdfPageLabels pdfPageLabels = new iTextSharp.text.pdf.PdfPageLabels();
                pdfPageLabels.AddPageLabel(1, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Basic Formatting");
                pdfPageLabels.AddPageLabel(2, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Internal Links");
                writer.PageLabels = pdfPageLabels;

            }
            catch (iTextSharp.text.DocumentException dex)
            {
                // Handle iTextSharp errors 
                retorno = false;
            }
            finally
            {
                // Clean up 
                doc.Close();
                doc = null;
                retorno = true;
            }
            return retorno;
        }

        private void CriarCapaRelatorio(iTextSharp.text.Document doc, string mes, string ano)
        {
            // Write page content.  Note the use of fonts and alignment attributes. 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\nRelatório\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, new Chunk("DUDE Bill$\n\n\n\n"));
            // Add a logo 
            String appPath = System.IO.Directory.GetCurrentDirectory();
            //iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(appPath + "\\PaperAirplane.jpg");
            //logoImage.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            //doc.Add(logoImage);
            //logoImage = null;
            // Write additional page content 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n" + mes + "/" + ano + "\n\n\n\n\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _smallFont, new Chunk("Generated " +
                DateTime.Now.Day.ToString() + " " +
                System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
                DateTime.Now.Year.ToString() + " " +
                DateTime.Now.ToShortTimeString()));
        }

        private void AddParagraph(Document doc, int alignment, iTextSharp.text.Font font, iTextSharp.text.IElement content)
        {
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
            paragraph.SetLeading(0f, 1.2f);
            paragraph.Alignment = alignment;
            paragraph.Font = font;
            paragraph.Add(content);
            doc.Add(paragraph);
        }

        private void AddPageWithInternalLinks(iTextSharp.text.Document doc)
        {
            // Generate links to be embedded in the page 
            Anchor researchAnchor = new Anchor("Research & Hypothesis\n\n", _standardFont);
            researchAnchor.Reference = "#research"; // this link references a named anchor within the document 
            Anchor graphAnchor = new Anchor("Graph\n\n", _standardFont);
            graphAnchor.Reference = "#graph";
            Anchor resultsAnchor = new Anchor("Results & Bibliography", _standardFont);
            resultsAnchor.Reference = "#results";
            // Add a new page to the document 
            doc.NewPage();
            // Add heading text to the page 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new iTextSharp.text.Chunk("TABLECONTENTS\n\n\n\n\n"));
            // Add the links to the page 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, researchAnchor);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, graphAnchor);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, resultsAnchor);
        }

        private void CriarPaginaMovimentos(iTextSharp.text.Document doc, List<Movimentacao> movimentos_)
        {
            // Add a new page to the document 
            doc.NewPage();
            // The header at the top of the page is an anchor linked to by the table of contents. 
            iTextSharp.text.Anchor contentsAnchor = new iTextSharp.text.Anchor("MOVIMENTAÇÕES\n\n");
            contentsAnchor.Name = "research";
            // Add the header anchor to the page 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, contentsAnchor);
            // Create an unordered bullet list.  The 10f argument separates the bullet from the text by 10 points 
            iTextSharp.text.List list = new iTextSharp.text.List(iTextSharp.text.List.UNORDERED, 10f);
            list.SetListSymbol("\u2022");   // Set the bullet symbol (without this a hypen starts each list item) 
            list.IndentationLeft = 20f;     // Indent the list 20 points 

            //Criação da tabela de entradas e saídas do mês
            PdfPTable tabela = new PdfPTable(new float[] { 15f, 100f, 40f, 40f});
            PdfPCell cell = new PdfPCell();

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Phrase = new Phrase("DIA", _header);
            tabela.AddCell(cell);

            cell.Phrase = new Phrase("DESCRIÇÃO MOVIMENTAÇÃO ", _header);
            tabela.AddCell(cell);

            cell.Phrase = new Phrase("[+] VALOR(R$)", _header);
            tabela.AddCell(cell);

            cell.Phrase = new Phrase("[-] VALOR(R$)", _header);
            tabela.AddCell(cell);

            PdfPCell pdf = new PdfPCell();
            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
            pdf.VerticalAlignment = Element.ALIGN_MIDDLE;

            foreach (Movimentacao p in movimentos_)
            {
                pdf.Phrase = new Phrase(p.DiaMovimentacao);
                tabela.AddCell(pdf);

                pdf.Phrase = new Phrase(p.DescricaoMovimentacao);
                tabela.AddCell(pdf);

                if (p.Sinal.Contains("+"))
                {
                    pdf.Phrase = new Phrase("R$ "+p.Valor);
                    tabela.AddCell(pdf);
                    pdf.Phrase = new Phrase("-");
                    tabela.AddCell(pdf);
                }
                else
                {
                    pdf.Phrase = new Phrase("-");
                    tabela.AddCell(pdf);
                    pdf.Phrase = new Phrase("R$ "+p.Valor);
                    tabela.AddCell(pdf);
                }
            }

            int totalEntrada = 0;
            int.TryParse(SomarMovimentos(movimentos_, "+").Replace("R$","").Replace(",","").Replace(".",""),out totalEntrada);
            int totalSaida = 0;
            int.TryParse(SomarMovimentos(movimentos_, "-").Replace("R$", "").Replace(",", "").Replace(".", ""), out totalSaida);

            pdf.Phrase = new Phrase("");
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("TOTAL", _header);
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("R$ "+SomarMovimentos(movimentos_, "+"));
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("R$ "+SomarMovimentos(movimentos_, "-"));
            tabela.AddCell(pdf);
            
            pdf.Phrase = new Phrase("");
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("");
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("SALDO FINAL", _header);
            tabela.AddCell(pdf);
            pdf.Phrase = new Phrase("R$ "+conversores.ConverterParaDinheiro((totalEntrada-totalSaida).ToString()));
            tabela.AddCell(pdf);

            doc.Add(list);
            doc.Add(tabela);

            doc.Add(new iTextSharp.text.Paragraph(" "));

            //Criação da tabela de saldo restante de cada um
            doc.Add(CriarTabelaSaldosRestantes(movimentos_));
        }

        private void CriarPaginaPoupanca(iTextSharp.text.Document doc, List<Poupanca> listaPoupancas_)
        {
            // Add a new page to the document 
            doc.NewPage();
            // The header at the top of the page is an anchor linked to by the table of contents. 
            iTextSharp.text.Anchor contentsAnchor = new iTextSharp.text.Anchor("SALDOS POUPANÇAS E FGTS\n\n");
            contentsAnchor.Name = "research";
            // Add the header anchor to the page 
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, contentsAnchor);
            // Create an unordered bullet list.  The 10f argument separates the bullet from the text by 10 points 
            iTextSharp.text.List list = new iTextSharp.text.List(iTextSharp.text.List.UNORDERED, 10f);
            list.SetListSymbol("\u2022");   // Set the bullet symbol (without this a hypen starts each list item) 
            list.IndentationLeft = 20f;     // Indent the list 20 points 

            //Criação da tabela de saldos das contas 
            PdfPTable tabela2 = new PdfPTable(new float[] { 50f, 80f });
            PdfPCell cell = new PdfPCell();

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Phrase = new Phrase("CONTA", _header);
            
            tabela2.AddCell(cell);
            cell.Phrase = new Phrase("SALDO", _header);
            tabela2.AddCell(cell);

            PdfPCell pdf = new PdfPCell();
            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
            pdf.VerticalAlignment = Element.ALIGN_MIDDLE;

            foreach (Poupanca p in listaPoupancas_)
            {
                pdf.Phrase = new Phrase(p.Nome);
                tabela2.AddCell(pdf);

                pdf.Phrase = new Phrase("R$ " + conversores.ConverterParaDinheiro(p.Valor));
                tabela2.AddCell(pdf);
            }

            pdf.Phrase = new Phrase("TOTAL", _header);
            tabela2.AddCell(pdf);
            pdf.Phrase = new Phrase("R$ " + SomarPoupancas(listaPoupancas_));
            tabela2.AddCell(pdf);

            doc.Add(list);
            doc.Add(tabela2);

        }

        private PdfPTable CriarTabelaSaldosRestantes(List<Movimentacao> movimentos_)
        {
            PdfPTable tabelaSaldo = new PdfPTable(new float[] { 50f, 70f });
            PdfPCell cellSaldo = new PdfPCell();

            cellSaldo.HorizontalAlignment = Element.ALIGN_CENTER;
            cellSaldo.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellSaldo.Phrase = new Phrase("PESSOA", _header);
            tabelaSaldo.AddCell(cellSaldo);

            cellSaldo.Phrase = new Phrase("SALDO (R$)", _header);
            tabelaSaldo.AddCell(cellSaldo);

            PdfPCell pdfSaldo = new PdfPCell();
            pdfSaldo.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfSaldo.VerticalAlignment = Element.ALIGN_MIDDLE;

            decimal totalSaidaDon = 0;
            decimal totalSaidaCa = 0;
            decimal totalEntradaDon = 0;
            decimal totalEntradaCa = 0;

            foreach (Movimentacao mov in movimentos_)
            {
                decimal valor = 0;

                if(mov.Sinal.Contains("-"))
                {
                    if(mov.Responsavel.Equals("1"))
                    {
                        decimal.TryParse(mov.Valor, out valor);
                        totalSaidaDon += valor;
                    }
                    else
                    {
                        decimal.TryParse(mov.Valor, out valor);
                        totalSaidaCa += valor;
                    }
                }
                else
                {
                    if (mov.Responsavel.Equals("1"))
                    {
                        decimal.TryParse(mov.Valor, out valor);
                        totalEntradaDon += valor;
                    }
                    else
                    {
                        decimal.TryParse(mov.Valor, out valor);
                        totalEntradaCa += valor;
                    }
                }
            }

            pdfSaldo.Phrase = new Phrase("DONATO RESSA");
            tabelaSaldo.AddCell(pdfSaldo);
            pdfSaldo.Phrase = new Phrase("R$ " + (totalEntradaDon-totalSaidaDon).ToString());
            tabelaSaldo.AddCell(pdfSaldo);

            pdfSaldo.Phrase = new Phrase("CAMYLLA HORIE");
            tabelaSaldo.AddCell(pdfSaldo);
            pdfSaldo.Phrase = new Phrase("R$ " + (totalEntradaCa - totalSaidaCa).ToString());
            tabelaSaldo.AddCell(pdfSaldo);

            return tabelaSaldo;
        }

        private void AddPageWithImage(iTextSharp.text.Document doc, String imagePath)
        {
            // Read the image file 
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new Uri(imagePath));
            // Set the page size to the dimensions of the image BEFORE adding a new page to the document.  
            float imageWidth = image.Width;
            float imageHeight = image.Height;
            doc.SetMargins(0, 0, 0, 0);
            doc.SetPageSize(new iTextSharp.text.Rectangle(imageWidth, imageHeight));
            // Add a new page 
            doc.NewPage();
            // Add the image to the page  
            doc.Add(image);
            image = null;
        }

        private void CreateXmpMetadata(iTextSharp.text.pdf.PdfWriter writer)
        {
            // Set up the buffer to hold the XMP metadata 
            byte[] buffer = new byte[65536];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, true);
            try
            {
                // XMP supports a number of different schemas, which are made available by iTextSharp. 
                // Here, the Dublin Core schema is chosen. 
                iTextSharp.text.xml.xmp.XmpSchema dc = new iTextSharp.text.xml.xmp.DublinCoreSchema();
                // Add Dublin Core attributes 
                iTextSharp.text.xml.xmp.LangAlt title = new iTextSharp.text.xml.xmp.LangAlt();
                //title.Add("x-defaulty Science Project"); 
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.TITLE, title);
                // Dublin Core allows multiple authors, so we create an XmpArray to hold the values 
                iTextSharp.text.xml.xmp.XmpArray author = new iTextSharp.text.xml.xmp.XmpArray(iTextSharp.text.xml.xmp.XmpArray.ORDERED);
                author.Add("Mhtenberg");
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.CREATOR, author);
                // Multiple subjects are also possible, so another XmpArray is used 
                iTextSharp.text.xml.xmp.XmpArray subject = new iTextSharp.text.xml.xmp.XmpArray(iTextSharp.text.xml.xmp.XmpArray.UNORDERED);
                subject.Add("paperlanes");
                subject.Add("scienceect");
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.SUBJECT, subject);
                // Create an XmpWriter using the MemoryStream defined earlier 
                iTextSharp.text.xml.xmp.XmpWriter xmp = new iTextSharp.text.xml.xmp.XmpWriter(ms);
                xmp.AddRdfDescription(dc);  // Add the completed metadata definition to the XmpWriter 
                xmp.Close();    // This flushes the XMP metadata into the buffer
                //——————————————————————————— 
                // Shrink the buffer to the correct size (discard empty elements of the byte array) 
                int bufsize = buffer.Length;
                int bufcount = 0;
                foreach (byte b in buffer)
                {
                    if (b == 0) break;
                    bufcount++;
                }
                System.IO.MemoryStream ms2 = new System.IO.MemoryStream(buffer, 0, bufcount);
                buffer = ms2.ToArray();
                //———————————————————————————
                // Add all of the XMP metadata to the PDF doc that we’re building 
                writer.XmpMetadata = buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
        }

        #endregion
    }
}
