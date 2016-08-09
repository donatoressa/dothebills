using DoTheBills.Telas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Gerenciadores
{
    public class ExcluirPagamentoEventArgs : EventArgs
    {
        public ExcluirPagamentoEventArgs()
        {
            UpdatePagamentos();
        }

        public void UpdatePagamentos()
        {
            Calendario cal = new Calendario();
        }
    }
}
