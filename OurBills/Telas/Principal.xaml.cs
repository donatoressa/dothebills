using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql;
using DoTheBills.Gerenciadores;
using DoTheBills.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        #region Atributos

        private Conversores conversores = new Conversores();
        
        // PostgeSQL-style connection string
        private string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "localhost", "5432", "postgres",
                   "12345", "DUDEBILLS");

        #endregion

        #region Construtor

        public Principal(DataSet usuarioLogado_)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            AtualizarPainelUsuarioLogado(usuarioLogado_);
        }

        #endregion

        #region Eventos

        private void btnUsuarios_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Usuarios u = new Usuarios();
            frame.Content = u;
        }

        private void btnVerCalendario_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Calendario c = new Calendario();
            Calendario c = new Calendario();
            frame.Content = c;
        }

        private void btnCadastrarPagamento_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CadastrarPagamento cp = new CadastrarPagamento();
            frame.Content = cp;
        }

        private void btnGerarRelatorio_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Processando processando = new Processando();
            processando.Show();
            Timer t = new Timer(5000);
            t.Start();

            SelecaoGeracaoRelatorio sgr = new SelecaoGeracaoRelatorio();
            sgr.Show();

            
            t.Stop();
            processando.Close();
        }

        private void btnAlterarPagamento_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AlterarPagamento ap = new AlterarPagamento();
            frame.Content = ap;
        }

        private void btnLogout_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Close();
        }

        private void btnCadastrarEntrada_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CadastrarEntrada ce = new CadastrarEntrada();
            frame.Content = ce;
        }

        private void btnVerPoupancas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManterPoupancas mp = new ManterPoupancas();
            frame.Content = mp;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarPainelUsuarioLogado(DataSet usuarioLogado_)
        {
            tbUsuario.Text = usuarioLogado_.Tables[0].Rows[0].Field<string>("NOME_USUARIO").ToUpper();

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("../Imagens/" + usuarioLogado_.Tables[0].Rows[0].Field<string>("LOGIN").ToString() + ".png", UriKind.Relative);
            logo.EndInit();
            if (logo != null)
                imgUsuarioLogado.Source = logo;
        }

        #endregion

    }
}
