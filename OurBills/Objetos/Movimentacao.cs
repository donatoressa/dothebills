using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Objetos
{
    public class Movimentacao
    {
        #region Atributos

        private string _diaMovimentacao;
        private string _descricaoMovimentacao;
        private string _valor;
        private string _sinal;
        private string _responsavel;

        #endregion

        #region Propriedades

        public string DiaMovimentacao
        {
            get { return _diaMovimentacao; }
            set { _diaMovimentacao = value; }
        }

        public string DescricaoMovimentacao
        {
            get { return _descricaoMovimentacao; }
            set { _descricaoMovimentacao = value; }
        }

        public string Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        public string Sinal
        {
            get { return _sinal; }
            set { _sinal = value; }
        }

        public string Responsavel
        {
            get { return _responsavel; }
            set { _responsavel = value; }
        }

        #endregion
    
        public Movimentacao(string dia_, string descricao_, string valor_, string sinal_)
        {
            this.DiaMovimentacao = dia_;
            this.DescricaoMovimentacao = descricao_;
            this.Valor = valor_;
            this.Sinal = sinal_;
        }

        public Movimentacao(string dia_, string descricao_, string valor_, string sinal_, string responsavel_)
        {
            this.DiaMovimentacao = dia_;
            this.DescricaoMovimentacao = descricao_;
            this.Valor = valor_;
            this.Sinal = sinal_;
            this.Responsavel = responsavel_;
        }
    }
}
