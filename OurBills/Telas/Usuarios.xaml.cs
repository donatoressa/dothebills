using Microsoft.Win32;
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
using System.Windows.Threading;

namespace DoTheBills.Telas
{
    /// <summary>
    /// Interaction logic for Usuarios.xaml
    /// </summary>
    public partial class Usuarios : UserControl
    {
        private string connstring = String.Format("Server={0};Port={1};" +
           "User Id={2};Password={3};Database={4};",
           "localhost", "5432", "postgres",
           "12345", "DUDEBILLS");
        private GerenciadorCriptografia gc = new GerenciadorCriptografia();
        private Conversores c = new Conversores();
        List<Usuario> usuarios = new List<Usuario>();
        Usuario usuarioSelecionado;
        private bool alteracaoSelecionada = false;
        private string pathFotoSelecionada = string.Empty;
        private string pathGravarFotoUsuario = string.Empty;
        private static Action EmptyDelegate = delegate() { };

        public Usuarios()
        {
            InitializeComponent();
            CarregarUsuarios();
            dgUsuarios.SelectedItem = 0;
        }

        #region Métodos Privados

        private Usuario CarregarDadosUsuarioSelecionado(string idUsuario_)
        {
            string nome = string.Empty;
            string login = string.Empty;
            string foto = string.Empty;
            string sal1Q = string.Empty;
            string sal2Q = string.Empty;
            Usuario user = null;

            foreach(Usuario usu in usuarios)
            {
                if(usu.IdUsuario.Equals(idUsuario_))
                {
                    nome = usu.NomeUsuario;
                    login = usu.LoginUsuario;
                    foto = string.Concat("../Imagens/", login, ".png");
                    sal1Q = usu.Sal1Q;
                    sal2Q = usu.Sal2Q;

                    user = new Usuario(usu.IdUsuario, nome, login, sal1Q, sal2Q, usu.Senha);
                    break;
                }
            }

            return user;
        }

        private void CarregarUsuarios()
        {
            DataSet dsUsuarios = new DataSet();
            DataTable dtUsuarios = new DataTable();
            string queryConsultaUsuarios = "SELECT * FROM \"USUARIOS\";";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            NpgsqlDataAdapter daTpPag = new NpgsqlDataAdapter(queryConsultaUsuarios, conn);

            dsUsuarios.Reset();
            daTpPag.Fill(dsUsuarios);
            dtUsuarios = dsUsuarios.Tables[0];
            conn.Close();
            
            for(int i = 0; i < dtUsuarios.Rows.Count; i++)
            {
                Usuario u = new Usuario(dtUsuarios.Rows[i].Field<long>("ID_USUARIO").ToString(),
                                        dtUsuarios.Rows[i].Field<string>("NOME_USUARIO"),
                                        dtUsuarios.Rows[i].Field<string>("LOGIN"),
                                        dtUsuarios.Rows[i].Field<decimal>("SAL_1Q").ToString(),
                                        dtUsuarios.Rows[i].Field<decimal>("SAL_2Q").ToString(),
                                        dtUsuarios.Rows[i].Field<string>("SENHA"));

                usuarios.Add(u);
            }

            dgUsuarios.ItemsSource = dtUsuarios.DefaultView;
            dgUsuarios.DisplayMemberPath = dtUsuarios.Columns["NOME_USUARIO"].ToString();
        }

        private bool ValidarPreenchimentoCampos()
        {
            bool saidaImg = true;
            bool saidaNome = true;
            bool saidaLogin = true;
            bool saidaSenha = true;
            bool saidaConfSenha = true;
            BrushConverter converter = new BrushConverter();
            
            if(string.IsNullOrEmpty(tbNome.Text))
            {
                saidaNome = false;
                tbNome.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbNome.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbNome.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbNome.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if(string.IsNullOrEmpty(tbLogin.Text))
            {
                saidaLogin = false;
                tbLogin.BorderBrush = converter.ConvertFromString("Red") as Brush;
                tbLogin.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                tbLogin.BorderBrush = converter.ConvertFromString("Black") as Brush;
                tbLogin.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if(string.IsNullOrEmpty(pbSenha.Password))
            {
                saidaSenha = false;
                pbSenha.BorderBrush = converter.ConvertFromString("Red") as Brush;
                pbSenha.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                pbSenha.BorderBrush = converter.ConvertFromString("Black") as Brush;
                pbSenha.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            if(string.IsNullOrEmpty(pbConfirmacaoSenha.Password))
            {
                saidaConfSenha = false;
                pbConfirmacaoSenha.BorderBrush = converter.ConvertFromString("Red") as Brush;
                pbConfirmacaoSenha.BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
            }
            else
            {
                pbConfirmacaoSenha.BorderBrush = converter.ConvertFromString("Black") as Brush;
                pbConfirmacaoSenha.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
            }

            return saidaImg && saidaNome && saidaLogin && saidaSenha && saidaConfSenha;
        }

        private bool ValidarSenha()
        {
            if(!string.IsNullOrEmpty(pbSenha.Password) && !string.IsNullOrEmpty(pbConfirmacaoSenha.Password))
            {
                return pbSenha.Password.Equals(pbConfirmacaoSenha.Password);
            }
            else
            {
                return false;
            }
        }

        private bool SalvarNovoUsuario(string login_, string nome_, string sal1Q_, string sal2Q_, string senha_)
        {
            string queryInsertUsuario = "INSERT INTO \"USUARIOS\"(" +
              "\"NOME_USUARIO\", \"LOGIN\", \"SENHA\"," +
              "\"SAL_1Q\", \"SAL_2Q\")" +
              "VALUES ('" + nome_ + "'," +
                      "'" + login_ + "'," +
                      "'" + gc.EncodeMD5Password(senha_) + "'," +
                      "'" + sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "'," +
                      "'" + (string.IsNullOrEmpty(sal2Q_) ? "0" : sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "")) + "');";

            string queryInsertEntradaSalario1Q = "INSERT INTO \"ENTRADAS\"(" +
               "\"DESC_ENTRADA\", \"VALOR_ENTRADA\", \"MES_ENTRADA\"," +
               "\"DIA_ENTRADA\")" +
               "VALUES (' salário " + nome_+ "'," +
                       "'" + sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "'," +
                       "'0'," +
                       "'15');";

            string queryInsertEntradaSalario2Q = "INSERT INTO \"ENTRADAS\"(" +
               "\"DESC_ENTRADA\", \"VALOR_ENTRADA\", \"MES_ENTRADA\"," +
               "\"DIA_ENTRADA\")" +
               "VALUES (' salário " + nome_ + "'," +
                       "'" + (string.IsNullOrEmpty(sal2Q_) ? "0" : sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "")) + "'," +
                       "'0'," +
                       "'28');";

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataSet ds1 = new DataSet();
            DataTable dt1 = new DataTable();
            DataSet ds2 = new DataSet();
            DataTable dt2 = new DataTable();
            // Making connection with Npgsql provider
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            try
            {
                //Gravando Usuário
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryInsertUsuario, conn);
                ds.Reset();
                da.Fill(ds);
                conn.Close();

                int sal1 = 0;
                int sal2 = 0;
                int.TryParse(sal1Q_.Replace("$", "").Replace(",", "").Replace(".", ""), out sal1);
                int.TryParse(sal2Q_.Replace("$", "").Replace(",", "").Replace(".", ""), out sal2);

                if (!string.IsNullOrEmpty(sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "")) && sal1 != 0)
                {
                    //Gravando Entrada Salário 1Q
                    NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(queryInsertEntradaSalario1Q, conn);
                    ds1.Reset();
                    da1.Fill(ds1);
                    conn.Close();
                }

                if (!string.IsNullOrEmpty(sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "")) && sal2 != 0)
                {
                    //Gravando Entrada Salário 2Q
                    NpgsqlDataAdapter da2 = new NpgsqlDataAdapter(queryInsertEntradaSalario2Q, conn);
                    ds2.Reset();
                    da2.Fill(ds2);
                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AlterarUsuario(string id_, string login_, string nome_, string sal1Q_, string sal2Q_, string senha_)
        {
            string queryUpdateUsuario = "UPDATE \"USUARIOS\"" +
                                        "SET \"NOME_USUARIO\"='"+nome_+"', \"SENHA\"='"+gc.EncodeMD5Password(senha_)+"', \"LOGIN\"='"+login_+"', "+
                                        "\"SAL_1Q\"='" + sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "', "+
                                        "\"SAL_2Q\"='" + sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "' WHERE \"ID_USUARIO\"='" + id_ + "';";

            string queryUpdateEntradaSalario1Q = "UPDATE \"ENTRADAS\" SET \"VALOR_ENTRADA\"='" + sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "'"+
                                                 "WHERE \"ID_USUARIO\"='" + id_ + "' AND \"DIA_ENTRADA\"='15';";

            string queryUpdateEntradaSalario2Q = "UPDATE \"ENTRADAS\" SET \"VALOR_ENTRADA\"='" + sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "") + "'" + 
                                                 "WHERE \"ID_USUARIO\"='" + id_ + "' AND \"DIA_ENTRADA\"='28';";

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataSet ds1 = new DataSet();
            DataTable dt1 = new DataTable();
            DataSet ds2 = new DataSet();
            DataTable dt2 = new DataTable();
            // Making connection with Npgsql provider
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            try
            {
                //Atualizando Usuário
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryUpdateUsuario, conn);
                ds.Reset();
                da.Fill(ds);
                conn.Close();

                int sal1 = 0;
                int sal2 = 0;
                int.TryParse(sal1Q_.Replace("$", "").Replace(",", "").Replace(".", ""), out sal1);
                int.TryParse(sal2Q_.Replace("$", "").Replace(",", "").Replace(".", ""), out sal2);

                if (!string.IsNullOrEmpty(sal1Q_.Replace("$", "").Replace(",", "").Replace(".", "")) && sal1 != 0)
                {
                    //Atualizando Entrada Salário 1Q
                    NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(queryUpdateEntradaSalario1Q, conn);
                    ds1.Reset();
                    da1.Fill(ds1);
                    conn.Close();
                }

                if (!string.IsNullOrEmpty(sal2Q_.Replace("$", "").Replace(",", "").Replace(".", "")) && sal2 != 0)
                {
                    //Atualizando Entrada Salário 2Q
                    NpgsqlDataAdapter da2 = new NpgsqlDataAdapter(queryUpdateEntradaSalario2Q, conn);
                    ds2.Reset();
                    da2.Fill(ds2);
                    conn.Close();
                }

                //Verifica se houve seleção de foto e salva a mesma no diretório da aplicação
                SalvarFoto();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Grava a foto obtida pelo OpenFileDialog no diretório da aplicação
        /// </summary>
        private void SalvarFoto()
        {
            if (!string.IsNullOrEmpty(pathFotoSelecionada))
            {
                pathGravarFotoUsuario = string.Concat("../Imagens/", tbLogin.Text, ".png");
                System.IO.File.Copy(pathFotoSelecionada, pathGravarFotoUsuario, true);
                pathFotoSelecionada = string.Empty;
                pathGravarFotoUsuario = string.Empty;
            }
        }

        #endregion

        #region Eventos

        private void btnNovo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbLogin.Text = string.Empty;
            tbLogin.IsEnabled = true;
            tbNome.Text = string.Empty;
            tbNome.IsEnabled = true;
            tbSal1Q.Number = 0;
            tbSal1Q.IsEnabled = true;
            tbSal2Q.Number = 0;
            tbSal2Q.IsEnabled = true;
            
            lblSenha.Visibility = Visibility.Visible;
            pbSenha.Visibility = Visibility.Visible;
            lblConfSenha.Visibility = Visibility.Visible;
            pbConfirmacaoSenha.Visibility = Visibility.Visible;
            pbSenha.Password = string.Empty;
            pbConfirmacaoSenha.Password = string.Empty;
            pbConfirmacaoSenha.IsEnabled = true;
            pbSenha.IsEnabled = true;
            ImgUsuario.Source = null;

            btnSalvar.IsEnabled = true;
            btnNovo.IsEnabled = false; 
            btnExcluir.IsEnabled = false;
            btnAlterar.IsEnabled = false;
            imagem.IsEnabled = true;

            alteracaoSelecionada = false;
        }

        private void btnSalvar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool retorno = false;

            if(ValidarPreenchimentoCampos() && ValidarSenha())
            {
                if (alteracaoSelecionada)
                {
                    retorno = AlterarUsuario(usuarioSelecionado.IdUsuario, tbLogin.Text, tbNome.Text, tbSal1Q.Text, tbSal2Q.Text, pbSenha.Password);
                }
                else
                {
                    retorno = SalvarNovoUsuario(tbLogin.Text, tbNome.Text, tbSal1Q.Text, tbSal2Q.Text, pbSenha.Password);
                }

                if(retorno)
                {
                    tbLogin.IsEnabled = false;
                    tbNome.IsEnabled = false;
                    tbSal1Q.IsEnabled = false;
                    tbSal2Q.IsEnabled = false;
                    pbConfirmacaoSenha.IsEnabled = false;
                    pbSenha.IsEnabled = false;
                    btnSalvar.IsEnabled = false;

                    Sucesso s = new Sucesso("Usuário cadastrado ou alterado com sucesso!");
                    s.Background = Brushes.White;
                    s.Show();
                }
                else
                {
                    Erro erro = new Erro("Erro na operação");
                    erro.Background = Brushes.White;
                    erro.Show();
                }
            }
        }

        private void btnExcluir_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            alteracaoSelecionada = false;
            Confirmacao conf = new Confirmacao("Deseja realmente excluir o usuário?");
            conf.DialogFinished += new EventHandler<ConfirmacaoEventArgs>(conf_DialogFinished);
            conf.Show();
        }

        private void btnAlterar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbNome.IsEnabled = true;
            tbLogin.IsEnabled = true;
            tbSal1Q.IsEnabled = true;
            tbSal2Q.IsEnabled = true;
            lblSenha.Visibility = Visibility.Visible;
            pbSenha.Visibility = Visibility.Visible;
            lblConfSenha.Visibility = Visibility.Visible;
            pbConfirmacaoSenha.Visibility = Visibility.Visible;
            pbSenha.Password = string.Empty;
            pbConfirmacaoSenha.Password = string.Empty;

            btnNovo.IsEnabled = true;
            btnAlterar.IsEnabled = false;
            btnExcluir.IsEnabled = true;
            btnSalvar.IsEnabled = true;

            alteracaoSelecionada = true;

            tbNome.Focus();
        }

        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv1 = (DataRowView)dgUsuarios.SelectedItem;

            if (dgUsuarios.SelectedIndex != -1)
            {
                string id_usuario = drv1.DataView[dgUsuarios.SelectedIndex].Row["ID_USUARIO"].ToString();
                usuarioSelecionado = CarregarDadosUsuarioSelecionado(id_usuario);

                tbNome.Text = usuarioSelecionado.NomeUsuario;
                tbLogin.Text = usuarioSelecionado.LoginUsuario;
                tbSal1Q.Text = c.ConverterParaDinheiroDolar(usuarioSelecionado.Sal1Q);
                tbSal2Q.Text = c.ConverterParaDinheiroDolar(usuarioSelecionado.Sal2Q);
                string foto = string.Concat("../Imagens/", usuarioSelecionado.LoginUsuario, ".png");

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(foto, UriKind.Relative);
                logo.EndInit();
                if (logo != null)
                    ImgUsuario.Source = logo;
            }

            pbConfirmacaoSenha.Visibility = Visibility.Collapsed;
            pbSenha.Visibility = Visibility.Collapsed;
            lblConfSenha.Visibility = Visibility.Collapsed;
            lblSenha.Visibility = Visibility.Collapsed;
            btnAlterar.IsEnabled = true;
            btnExcluir.IsEnabled = true;
        }

        private void ImgUsuario_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Arquivos de imagem (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp, *.gif) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp; *.gif";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                pathFotoSelecionada = System.IO.Path.GetDirectoryName(dlg.FileName);

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(pathFotoSelecionada, UriKind.Relative);
                logo.EndInit();
                if (logo != null)
                    ImgUsuario.Source = logo;

                this.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
        }

        /// <summary>
        /// Evento disparado ao selecionar uma opção na tela de confirmação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conf_DialogFinished(object sender, ConfirmacaoEventArgs e)
        {
           if(e.Confirmado)
           {
               string id = string.Empty;

               foreach(Usuario us in usuarios)
               {
                   if(us.LoginUsuario.Equals(tbLogin.Text))
                   {
                       id = us.IdUsuario;
                   }
               }

               string queryExcluirUsuario = "DELETE FROM \"USUARIOS\" WHERE \"ID_USUARIO\" = '"+ id +"' ;";
               string queryExcluirSalarios = "DELETE FROM \"ENTRADAS\" WHERE \"ID_USUARIO\" = '" + id + "';";

               try
               {
                   DataSet ds = new DataSet();
                   DataTable dt = new DataTable();
                   NpgsqlConnection conn = new NpgsqlConnection(connstring);
                   conn.Open();
                   NpgsqlDataAdapter da = new NpgsqlDataAdapter(queryExcluirUsuario, conn);
                   ds.Reset();
                   da.Fill(ds);
                   conn.Close();

                   Sucesso s = new Sucesso("Usuário removido com sucesso!");
                   s.Background = Brushes.White;
                   s.Show();
               }
               catch(Exception ex)
               {
                   Erro erro = new Erro("Erro na exclusão");
                   erro.Background = Brushes.White;
                   erro.Show();
               }
           }
        }

        #endregion


    }
}
