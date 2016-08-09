using DoTheBills.Objetos;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Confirmacao.xaml
    /// </summary>
    public partial class Confirmacao : Window
    {
        public event EventHandler<ConfirmacaoEventArgs> DialogFinished;

        public Confirmacao(string mensagem)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTexto.Text = mensagem;
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

        private void spSim_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogFinished != null)
                DialogFinished(this, new ConfirmacaoEventArgs(true));
            this.Close();
        }

        private void spNao_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogFinished != null)
                DialogFinished(this, new ConfirmacaoEventArgs(false));
            this.Close();
        }

        public void OnDialogFinished(bool status)
        {
            if (DialogFinished != null)
                DialogFinished(this, new ConfirmacaoEventArgs(status));
        }
    }
}
