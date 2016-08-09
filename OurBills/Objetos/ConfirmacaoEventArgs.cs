using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Objetos
{
    public class ConfirmacaoEventArgs
    {
        private bool confirmado;

        public bool Confirmado
        {
            get { return confirmado; }
            set { confirmado = value; }
        }

        public ConfirmacaoEventArgs(bool confirmado_)
        {
            this.confirmado = confirmado_;
        }

    }
}
