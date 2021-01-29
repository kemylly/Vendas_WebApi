using System.Collections.Generic;
using Newtonsoft.Json;

namespace desafio_api.Models
{
    public class Fornecedor
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string cnpj { get; set; }
        //[JsonIgnore]
        public List<Produto> Produtos { get; set; }  //lista de produtos desse fornecedor
        public bool Status {get; set;} //delecao boleana
    }
}