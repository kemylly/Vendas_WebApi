using System;

namespace desafio_api.Models
{
    public class Cliente
    {
        public int id {get; set;}
        public string nome { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string documento { get; set; }  //cpf ou rg
        public DateTime dataCadastro { get; set; }
        public bool Status {get; set;}  //delecao boleana
    }
}