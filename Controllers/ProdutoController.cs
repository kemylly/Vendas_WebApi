using System;
using System.Linq;
using desafio_api.Data;
using desafio_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using desafio_api.HATEOAS;
using System.Collections.Generic;

namespace desafio_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.Hateoas Hateoas;
        //conexao com o banco
        public ProdutoController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new HATEOAS.Hateoas("localhost:5001/api/Fornecedor");
            Hateoas.AddAction("GET_INFO","GET");
            Hateoas.AddAction("DELETE_PRODUCT","DELETE");
        }

        [HttpGet]
        public IActionResult ListarProduto() //listar todos os produtos
        {
            try{
                var produto = database.Produtos.Include(f => f.Fornecedor).Where(p => p.Status == true).ToList();
                
                try{
                    List<ProdutoContainer> produtoHateoas = new List<ProdutoContainer>();
                
                    foreach (var prod in produto)
                    {
                        ProdutoContainer produtoContainer = new ProdutoContainer();
                        produtoContainer.produto = prod;
                        produtoContainer.links = Hateoas.GetActions(prod.id.ToString());
                        produtoHateoas.Add(produtoContainer);
                    }

                    return Ok(produtoHateoas);
                }catch{
                    return Ok(produto);
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar produtos"});
            }
        }

        [HttpGet("{id}")]
        public IActionResult ListarProduto(int id)  //listar um produto por id
        {
            try{ //tentar localizar o produto
                Produto produto = database.Produtos.Include(f => f.Fornecedor).First(p => p.id == id);
                
                if(produto.Status == true)
                {
                    try{
                        ProdutoContainer produtoHateoas = new ProdutoContainer();
                        produtoHateoas.produto = produto;
                        produtoHateoas.links = Hateoas.GetActions(produto.id.ToString());
                        return Ok(produtoHateoas);
                    }catch{
                        return Ok(produto);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Produto deletado"}); 
                }
                
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            } 
        }

        [HttpGet("asc")]
        public IActionResult Crescente() //listar os produtos em ordem crescente
        {
            try{
                var produto = database.Produtos.Include(f => f.Fornecedor).OrderBy(p => p.nome).Where(p => p.Status == true).ToList();
                try{
                    List<ProdutoContainer> produtoHateoas = new List<ProdutoContainer>();
                
                    foreach (var prod in produto)
                    {
                        ProdutoContainer produtoContainer = new ProdutoContainer();
                        produtoContainer.produto = prod;
                        produtoContainer.links = Hateoas.GetActions(prod.id.ToString());
                        produtoHateoas.Add(produtoContainer);
                    }

                    return Ok(produtoHateoas);
                }catch{
                    return Ok(produto);
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar"});
            }
        }

        [HttpGet("desc")]
        public IActionResult Descrescente() //listra descrescente
        {
            try{
                var produto = database.Produtos.Include(f => f.Fornecedor).OrderBy(p => p.nome).Where(p => p.Status == true).ToList();
                try{
                    List<ProdutoContainer> produtoHateoas = new List<ProdutoContainer>();
                
                    foreach (var prod in produto)
                    {
                        ProdutoContainer produtoContainer = new ProdutoContainer();
                        produtoContainer.produto = prod;
                        produtoContainer.links = Hateoas.GetActions(prod.id.ToString());
                        produtoHateoas.Add(produtoContainer);
                    }

                    return Ok(produtoHateoas);
                }catch{
                    return Ok(produto);
                }
                
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar"});
            }
        }

        [HttpGet("nome/{nome}")]
        public IActionResult ListarProdutoNome(string nome)
        {
            if(nome != null && nome.Length > 1)
            {
                try{
                    Produto produto = database.Produtos.Include(f => f.Fornecedor).FirstOrDefault(p => p.nome.ToUpper() == nome.ToUpper());
                    if(produto.Status == true)
                    {
                        try{
                            
                            ProdutoContainer produtoHateoas = new ProdutoContainer();
                            produtoHateoas.produto = produto;
                            produtoHateoas.links = Hateoas.GetActions(produto.id.ToString());
                            
                            return Ok(produtoHateoas);
                        }catch{

                            return Ok(produto);

                        }
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Produto deletado"}); 
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Não encontrado"});
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome invalido"});
            } 
        }

        [HttpGet("codigo/{codigo}")]  //passa o codigo do produto para achar ele
        public IActionResult ListarProdutoCodigo(string codigo)
        {
            try{
                Produto produto = database.Produtos.Include(f => f.Fornecedor).FirstOrDefault(p => p.codigoProduto == codigo.Trim());
                if(produto.Status)
                {
                    try{
                        ProdutoContainer produtoHateoas = new ProdutoContainer();
                        produtoHateoas.produto = produto;
                        produtoHateoas.links = Hateoas.GetActions(produto.id.ToString());
                        return Ok(produtoHateoas);
                    }catch{
                        return Ok(produto);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Produto deletado"});
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"});
            }
        }

        [HttpGet("categoria/{categoria}")]
        public IActionResult ListarProdutoCategoria(string categoria)
        {
            try{
                var produto = database.Produtos.Where(p => p.categoria.ToLower() == categoria.ToLower() && p.Status == true).Include(f => f.Fornecedor).ToList();
                try{
                    List<ProdutoContainer> produtoHateoas = new List<ProdutoContainer>();
                
                    foreach (var prod in produto)
                    {
                        ProdutoContainer produtoContainer = new ProdutoContainer();
                        produtoContainer.produto = prod;
                        produtoContainer.links = Hateoas.GetActions(prod.id.ToString());
                        produtoHateoas.Add(produtoContainer);
                    }

                    return Ok(produtoHateoas);
                }catch{
                    return Ok(produto);
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrada"});
            }
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public IActionResult Cadastrar([FromBody] ProdutoTemp pTemp)
        {
            //validacao
            if(pTemp.nome.Length <= 1 || pTemp.nome.Length > 150)  //validar nome
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }
            try{
                var nome = database.Produtos.First(n => n.nome.ToUpper() == pTemp.nome.ToUpper());
                if(nome != null)
                {
                     Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome já cadastrado"});
                }
            }catch{
                //está tudo bem
            }
            
            
            if(pTemp.valor <= 0) //verificar se produto é valido
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Valor Invalido"});
            }

            if(pTemp.promocao == true) //verificar se a promocao é verdadeira
            {
                if(pTemp.valorPromo <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Valor da promocao Invalido"});
                }
            }

            if(pTemp.categoria.Length <= 1 || pTemp.categoria.Length > 40)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Categoria Invalido"});
            }

            if(pTemp.imagem == null)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Imagem Invalida"});
            }

            if(pTemp.quantidade.ToString() == null || pTemp.quantidade <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Quantidade Invalido"});
            }

            if(pTemp.FornecedorId < 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Fornecedor Invalido"});
            }
            try{ //ver se esse fornecedor existe mesmo
                var forn =  database.Fornecedores.First(f => f.id == pTemp.FornecedorId);
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Fornecedor Invalido"});
            }

            //cadastrar
            try{
                Produto pr = new Produto();
                Random r = new Random();
                int codigo = r.Next(1000000);
                pr.nome = pTemp.nome;
                pr.codigoProduto = codigo.ToString();
                pr.valor = pTemp.valor;
                pr.promocao = pTemp.promocao;
                pr.categoria = pTemp.categoria;
                pr.imagem = pTemp.imagem;
                pr.quantidade = pTemp.quantidade;
                pr.Fornecedor = database.Fornecedores.First(f => f.id == pTemp.FornecedorId);
                pr.Status = true;
                if(pTemp.promocao)
                {
                    pr.valorPromo = pTemp.valorPromo;
                }

                database.Produtos.Add(pr);
                database.SaveChanges();

                Response.StatusCode = 200;
                return new ObjectResult(new {msg = "Cadastro feito com sucesso"});
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult("Erro ao cadastrar");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles="Admin")]
        public IActionResult Deletar(int id)  //tentar deletar um cliente
        {
            if(id > 0)
            {
                try{
                    Produto pro = database.Produtos.First(p => p.id == id);

                    if(pro.Status)
                    {
                         try{
                            pro.Status = false;
                            //database.Produtos.Remove(pro);
                            database.SaveChanges();

                            Response.StatusCode = 201;
                            return new ObjectResult("Cliente deletado");
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Erro ao tentar deletar"});
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Esse produto já foi deletado"}); 
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Não encontrado"});
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Id invalido"});
            }
           
        }

        [HttpPatch]
        [Authorize(Roles="Admin")]
        public IActionResult EditarParcial([FromBody] ProdutoTemp produto)
        {
            if(produto.id > 0) //verificando id recebido do body
            {
                try{ //procurando id no banco
                    var pro = database.Produtos.First(p => p.id == produto.id);

                    if(pro != null) // verificando o que veio do banco
                    { 
                        if(pro.Status)
                        {
                            try{
                                pro.nome = produto.nome != null ? produto.nome : pro.nome;
                                pro.codigoProduto = produto.codigoProduto != null ? produto.codigoProduto : pro.codigoProduto;
                                pro.valor = produto.valor >= 1 ? produto.valor : pro.valor;
                                pro.promocao = produto.promocao ? produto.promocao : pro.promocao;
                                pro.valorPromo = produto.valorPromo >= 1 ? produto.valorPromo : pro.valorPromo;
                                pro.categoria = produto.categoria != null ? produto.categoria : pro.categoria;
                                pro.imagem= produto.imagem != null ? produto.imagem : pro.imagem;
                                pro.quantidade = produto.quantidade >= 1 ? produto.quantidade : pro.quantidade;
                                pro.Fornecedor = database.Fornecedores.First(f => f.id == produto.FornecedorId) != null ? database.Fornecedores.First(f => f.id == produto.FornecedorId) : pro.Fornecedor;

                                database.SaveChanges();

                                Response.StatusCode = 200;
                                return new ObjectResult(new {msg = "Edição feita com sucesso"});
                            }catch{
                                Response.StatusCode = 400;
                                return new ObjectResult(new {msg = "Erro ao editar produto"});
                            }
                        }else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new {msg = "Esse produto foi deletado"});
                        }
                        
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Erro ao retornar produto"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id de Produto é inválido!"});
            }
        }

        [HttpPut]
        [Authorize(Roles="Admin")]
        public IActionResult EditarTudo([FromBody] ProdutoTemp produto)
        {
            if(produto.id > 0) //verficando se é um id maior que zero
            {
                try{
                    var pro = database.Produtos.First(p => p.id == produto.id);

                    if(pro != null)
                    {
                        //validações
                        if(produto.nome.Length <= 1 || produto.nome.Length > 150)  //validar nome
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Nome Invalido"});
                        }

                        if(produto.codigoProduto == null) //verificar se foi passado o codigo do produto
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Codigo Invalido"});
                        }
                        try{
                            var codigo =  database.Produtos.FirstOrDefault(c => c.codigoProduto == produto.codigoProduto);
                            if(codigo != null)
                            {
                                if(codigo.id != produto.id)
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "Codigo já cadastrado"});
                                }
                            }
                        }catch{ }

                        if(produto.valor <= 0) //verificar se produto é valido
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Valor Invalido"});
                        }

                        if(produto.promocao == true) //verificar se a promocao é verdadeira
                        {
                            if(produto.valorPromo <= 0)
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Valor da promocao Invalido"});
                            }
                        }

                        if(produto.categoria.Length <= 1 || produto.categoria.Length > 40)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Categoria Invalido"});
                        }

                        if(produto.imagem == null)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Imagem Invalida"});
                        }

                        if(produto.quantidade.ToString() == null || produto.quantidade <= 0)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Quantidade Invalido"});
                        }
                        
                        //validando fornecedor
                        if(produto.FornecedorId <= 0)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Fornecedor Invalido"});
                        }
                        try{ //ver se esse fornecedor existe mesmo
                            var forn =  database.Fornecedores.First(f => f.id == produto.FornecedorId);
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Fornecedor Invalido"});
                        }

                        //editar
                        try{  //caminho feliz
                            pro.nome = produto.nome;
                            pro.codigoProduto = produto.codigoProduto;
                            pro.valor = produto.valor;
                            pro.promocao = produto.promocao;
                            pro.categoria = produto.categoria;
                            pro.imagem = produto.imagem;
                            pro.quantidade = produto.quantidade;
                            if(produto.promocao)
                            {
                                pro.valorPromo = produto.valorPromo;
                            }
                            pro.Fornecedor = database.Fornecedores.First(f => f.id == produto.FornecedorId);

                            database.SaveChanges();

                            Response.StatusCode = 200;
                            return new ObjectResult(new {msg = "Edição feita com sucesso"});
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new {msg = "Erro ao editar produto"});
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Produto não encontrado"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Verifique se todos os campos estão corretos"}); 
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id inválido!"});
            }
        }

        public class ProdutoTemp
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
            public int FornecedorId { get; set; }
            public Fornecedor Fornecedor { get; set; }
        }

        public class ProdutoContainer
        {
            public Produto produto;
            public Link[] links;
        }
    }
}