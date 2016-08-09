using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoTheBills.Gerenciadores
{
    public class GerenciadorCriptografia
    {
        //private static byte[] chave = { };
        //private static byte[] iv = { 12, 34, 56, 78, 90, 102, 114, 126 };

        //public string Criptografar(string valor, string chaveCriptografia)
        //{
        //    DESCryptoServiceProvider des;
        //    MemoryStream ms;
        //    CryptoStream cs; byte[] input;
        //    try
        //    {
        //        des = new DESCryptoServiceProvider();
        //        ms = new MemoryStream();

        //        input = Encoding.UTF8.GetBytes(valor); chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));
        //        cs = new CryptoStream(ms, des.CreateEncryptor(chave, iv), CryptoStreamMode.Write);
        //        cs.Write(input, 0, input.Length);
        //        cs.FlushFinalBlock();

        //        return Convert.ToBase64String(ms.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public string Descriptografar(string valor, string chaveCriptografia)
        //{
        //    DESCryptoServiceProvider des;
        //    MemoryStream ms;
        //    CryptoStream cs; byte[] input;

        //    try
        //    {
        //        des = new DESCryptoServiceProvider();
        //        ms = new MemoryStream();

        //        input = new byte[valor.Length];
        //        input = Convert.FromBase64String(valor.Replace(" ", "+"));

        //        chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));

        //        cs = new CryptoStream(ms, des.CreateDecryptor(chave, iv), CryptoStreamMode.Write);
        //        cs.Write(input, 0, input.Length);
        //        cs.FlushFinalBlock();

        //        return Encoding.UTF8.GetString(ms.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string EncodeMD5Password(string originalPassword)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            // instancia o MD5CryptoServiceProvider, e transforma a password em byte[] para calcular o hash
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            // converte o hash em string
            return BitConverter.ToString(encodedBytes);
        }
    }
}
