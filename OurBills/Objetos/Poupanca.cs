using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Objetos
{
    public class Poupanca
    {
        #region Atributos

        private string _nome;
        private string _valor;
        private string _ultimaAtualizacao;
        private string _agencia;
        private string _conta;

        #endregion

        #region Propriedades

        public string UltimaAtualizacao
        {
            get { return _ultimaAtualizacao; }
            set { _ultimaAtualizacao = value; }
        }
        public string Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string Agencia
        {
            get { return _agencia; }
            set { _agencia = value; }
        }

        public string Conta
        {
            get { return _conta; }
            set { _conta = value; }
        }

        #endregion

        #region Construtor

        public Poupanca(string nome_, string valor_, string ultimaAtualizacao_, string agencia_, string conta_)
        {
            this.Nome = nome_;
            this.Valor = valor_;
            this.UltimaAtualizacao = ultimaAtualizacao_;
            this.Agencia = agencia_;
            this.Conta = conta_;
        }

        #endregion
    }
}
