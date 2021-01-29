using System;
using System.Text;
using System.Security.Cryptography;


namespace desafio_api.Controllers
{
    public class Hash
        {
            private HashAlgorithm _algoritmo;

            public Hash(HashAlgorithm algorithm)
            {
                _algoritmo = algorithm;
            }

            public string CriptografarSenha(string senha)  //criptografar senha
            {
                var encodedValue = Encoding.UTF8.GetBytes(senha);
                var encryptedPassword = _algoritmo.ComputeHash(encodedValue);

                var se = new StringBuilder();
                foreach (var caracter in encryptedPassword)
                {
                    se.Append(caracter.ToString("X2"));
                }
                return se.ToString();
            }

            public bool VerificarSenha(string senhaDigitada, string senhaCadastrada)
            {
                if(string.IsNullOrEmpty(senhaCadastrada))
                    throw new NullReferenceException("Cadastre uma senha");

                var encryptedPassword = _algoritmo.ComputeHash(Encoding.UTF8.GetBytes(senhaDigitada));

                var sb = new StringBuilder();
                foreach (var caractere in encryptedPassword)
                {
                    sb.Append(caractere.ToString("X2"));
                }

                return sb.ToString() == senhaCadastrada;
            }
        }
}

