using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace desafio_api.Models
{
    public class Venda
    {
        [Key]
        public long id { get; set; }
        public Fornecedor Fornecedor { get; set; }  //relacionbamento com fornecedor
        public Cliente cliente { get; set; }  //relacionamento com cliente
        //[JsonIgnore]
        public List<Produto> Produto { get; set; }  //lista de produtos
        public virtual List<VendaProduto> Produtos {get; set;}  //relacionamento com produtos
        public float totalCompra { get; set; }
        public DateTime dataCompra { get; set; }
        public bool Status {get; set;}  //delecao boleana
    }
}