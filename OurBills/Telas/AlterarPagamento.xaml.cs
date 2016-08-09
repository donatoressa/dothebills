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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoTheBills.Telas
{
    /// <summary>
    /// Interaction logic for AlterarPagamento.xaml
    /// </summary>
    public partial class AlterarPagamento : UserControl
    {
        #region Construtor

        public AlterarPagamento()
        {
            InitializeComponent();
        }

        #endregion

        #region Eventos

        private void btnConsultar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(ValidarPreenchimentoFiltros())
            {

            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarPreenchimentoFiltros()
        {
            return (cbTipoPagamento.SelectedIndex >= 0 && cbDiaPagamento.SelectedIndex >= 0);
        }

        #endregion

        private void btnLimpar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnSalvar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnConsultar_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void cbResponsavel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbRecorrente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
