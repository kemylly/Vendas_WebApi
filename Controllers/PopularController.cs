using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using desafio_api.Data;
using desafio_api.Models;
using System.Security.Cryptography;

namespace desafio_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class PopularController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        //conexao com o banco
        public PopularController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Popular()
        {
            try{
            //chamando a criptografia
            MD5 senhaHash = MD5.Create();
            Hash senhaCp = new Hash(senhaHash);
            //popular cliente
            Cliente clienteA = new Cliente();
            clienteA.nome = "Kemylly";
            clienteA.email = "admin@gmail.com";
            clienteA.senha = senhaCp.CriptografarSenha("segura123");
            clienteA.documento = "82241708020";
            clienteA.dataCadastro = DateTime.Now;
            clienteA.Status = true;
            database.Clientes.Add(clienteA);
            database.SaveChanges();

            Cliente cliente = new Cliente();
            cliente.nome = "Yona";
            cliente.email = "yona@gmail.com";
            cliente.senha = senhaCp.CriptografarSenha("segura123");
            cliente.documento = "52748054008";
            cliente.dataCadastro = DateTime.Now;
            cliente.Status = true;
            database.Clientes.Add(cliente);
            database.SaveChanges();

            Cliente cliente1 = new Cliente();
            cliente1.nome = "Nezuko";
            cliente1.email = "nezuko@gmail.com";
            cliente1.senha = senhaCp.CriptografarSenha("segura123");
            cliente1.documento = "48012195003";
            cliente1.dataCadastro = DateTime.Now;
            cliente1.Status = true;
            database.Clientes.Add(cliente1);
            database.SaveChanges();

            //cadastrando fornecedores
            Fornecedor fornecedor = new Fornecedor();
            fornecedor.nome = "Editora";
            fornecedor.cnpj = "42716207000107";
            fornecedor.Status = true;
            database.Fornecedores.Add(fornecedor);
            database.SaveChanges();

            //cadastrando produto
            Random r = new Random();
            int codigo = r.Next(1000000); //sete numeros
            Produto produto = new Produto();
            produto.nome = "Jupiter";
            produto.codigoProduto = codigo.ToString();
            produto.valor = 15;
            produto.promocao = true;
            produto.valorPromo = 12;
            produto.categoria = "Livro";
            produto.imagem = "Jupitercapa.jpg";
            produto.quantidade = 11;
            produto.Fornecedor = fornecedor;
            produto.Status = true;
            database.Produtos.Add(produto);
            database.SaveChanges();

            int codigo1 = r.Next(1000000); //sete numeros
            Produto produto1 = new Produto();
            produto1.nome = "Efemero";
            produto1.codigoProduto = codigo1.ToString();
            produto1.valor = 20;
            produto1.promocao = false;
            produto1.categoria = "Livro";
            produto1.imagem = "efemerocapa.jpg";
            produto1.quantidade = 9;
            produto1.Fornecedor = fornecedor;  //editora
            produto1.Status = true;
            database.Produtos.Add(produto1);
            database.SaveChanges();

            //cadastrar uma venda
            Venda venda = new Venda();
            venda.Fornecedor = fornecedor;  //editora
            venda.cliente = cliente;  //yona
            venda.totalCompra = 12;
            venda.dataCompra = DateTime.Now;
            venda.Status = true;
            database.Vendas.Add(venda);
            database.SaveChanges();
            VendaProduto VenPro = new VendaProduto();
            VenPro.Produto = database.Produtos.First(p => p.id == produto.id);  //jupiter
            VenPro.Venda = database.Vendas.First(p => p.id == venda.id);
            database.VendasProdutos.Add(VenPro);
            database.SaveChanges();

            Venda venda1 = new Venda();
            venda1.Fornecedor = fornecedor;
            venda1.cliente = cliente1; //nezuko
            venda1.totalCompra = 12;
            venda1.dataCompra = DateTime.Now;
            venda1.Status = true;
            database.Vendas.Add(venda1);
            database.SaveChanges();
            VendaProduto VenPro1 = new VendaProduto();
            VenPro1.Produto = database.Produtos.First(p => p.id == produto.id);
            VenPro1.Venda = database.Vendas.First(p => p.id == venda1.id);
            database.VendasProdutos.Add(VenPro1);
            database.SaveChanges();
            VendaProduto VenPro2 = new VendaProduto();
            VenPro2.Produto = database.Produtos.First(p => p.id == produto1.id);
            VenPro2.Venda = database.Vendas.First(p => p.id == venda1.id);
            database.VendasProdutos.Add(VenPro2);
            database.SaveChanges();

            Response.StatusCode = 201;
            return Ok(new{info = "Banco populado"});
            }
            catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao popular banco"});
            }
        }
    }
}