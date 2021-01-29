using System.Collections.Generic;

namespace desafio_api.Models
{
    public class Produto
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string codigoProduto { get; set; }
        public float valor { get; set; }
        public bool promocao { get; set; }
        public float valorPromo { get; set; }
        public string categoria { get; set; }
        public string imagem {get; set;}
        public long quantidade { get; set; }
        public Fornecedor Fornecedor { get; set; }  //relacionamento co funcionario
        public virtual List<VendaProduto> Vendas {get; set;} //relacionamento com venda
        public bool Status {get; set;}  //delecao boleana
    }
}