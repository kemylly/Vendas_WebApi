namespace desafio_api.Models
{
    public class VendaProduto
    {
        //Essa classe faz realção entre produto e venda
        //Uma venda pode ter diversos produtos 
        //E um produto pode participar de diversas vendas
        public long VendaID { get; set; }
        public virtual Venda Venda { get; set; }
        public int ProdutoID { get; set; }
        public virtual Produto Produto { get; set; }
    }
}