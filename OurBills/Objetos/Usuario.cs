using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Objetos
{
    public class Usuario
    {
        #region Atributos

        private string idUsuario;
        private string nomeUsuario;
        private string loginUsuario;
        private string fotoUsuario;
        private string sal1Q;
        private string sal2Q;
        private string senha;



        #endregion

        #region Propriedades

        public string IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }

        public string NomeUsuario
        {
            get { return nomeUsuario; }
            set { nomeUsuario = value; }
        }

        public string LoginUsuario
        {
            get { return loginUsuario; }
            set { loginUsuario = value; }
        }

        public string Sal1Q
        {
            get { return sal1Q; }
            set { sal1Q = value; }
        }

        public string Sal2Q
        {
            get { return sal2Q; }
            set { sal2Q = value; }
        }

        public string FotoUsuario
        {
            get { return fotoUsuario; }
            set { fotoUsuario = value; }
        }

        public string Senha
        {
            get { return senha; }
            set { senha = value; }
        }

        #endregion

        //Consulta
        public Usuario(string nomeUsuario_, string loginUsuario_, string fotoUsuario_, string sal1Q_, string sal2Q_)
        {
            this.loginUsuario = loginUsuario_;
            this.nomeUsuario = nomeUsuario_;
            this.fotoUsuario = fotoUsuario_;
            this.sal1Q = sal1Q_;
            this.sal2Q = sal2Q_;
        }

        //Consulta
        public Usuario(string idUsuario_, string nomeUsuario_, string loginUsuario_, string sal1Q_, string sal2Q_, string senha_)
        {
            this.idUsuario = idUsuario_;
            this.loginUsuario = loginUsuario_;
            this.nomeUsuario = nomeUsuario_;
            this.sal1Q = sal1Q_;
            this.sal2Q = sal2Q_;
            this.senha = senha_;
        }
    }
}
