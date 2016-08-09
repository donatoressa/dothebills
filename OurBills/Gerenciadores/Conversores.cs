using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Gerenciadores
{
    public class Conversores
    {
        public string ConverterParaDinheiro(string valor)
        {
            string valorSaida = valor;

            if (valor.Length <= 2)
            {
                valorSaida = valor.PadLeft(3, '0');
            }

            valorSaida = valorSaida.Insert(valorSaida.Length - 2, ",");

            return valorSaida;
        }

        public string ConverterParaDinheiroDolar(string valor)
        {
            string valorSaida = valor;

            if (valor.Length <= 2)
            {
                valorSaida = valor.PadLeft(3, '0');
            }

            valorSaida = valorSaida.Insert(valorSaida.Length - 2, ".");

            return valorSaida;
        }

        public string ConverterParaDinheiro(decimal valor)
        {
            string val = valor.ToString();
            string valorSaida = val;

            if (val.Length <= 2)
            {
                valorSaida = val.PadLeft(3, '0');
            }

            valorSaida = valorSaida.Insert(valorSaida.Length - 2, ",");

            return valorSaida;
        }
    }
}
