using System;
using System.Linq;
using desafio_api.Data;
using desafio_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace desafio_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class VendaController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        //conexao com o banco
        public VendaController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult ListarVendas() //listar todas as vendas
        {
            try{
                var venda = database.Vendas
                .Include(f => f.Fornecedor)
                .Include(c => c.cliente)
                .Include(p => p.Produto)
                //.Include(p => p.Produtos)  //venda produtos
                    //.ThenInclude(p => p.Produto)
                .Where(v => v.Status == true)
                .ToList();
                
                return Ok(venda);
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar venda"});
            }
        }

        [HttpGet("{id}")]
        public IActionResult ListarVendas(int id)  //listar as vendas por id
        {
            if(id > 0)
            {
                try{  //verificar a existencia dessa venda
                    Venda venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).First(v => v.id == id);
                    if(venda != null)
                    {
                        if(venda.Status)  //verificar se essa venda foi deletada
                        {
                            return Ok(venda);
                        }
                        else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Venda deletada"});
                        } 
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Venda não encontrada"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Não encontrada"}); 
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Id invalido"});
            }
        }

        [HttpGet("asc")]
        public IActionResult Crescente()
        {
            try{
                var venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).OrderBy(v => v.totalCompra).Where(v => v.Status == true).ToList();
                return Ok(venda);
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar venda"});
            }
            
        }

        [HttpGet("desc")]
        public IActionResult Decrescente()
        {
            try{
                var venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).OrderByDescending(v => v.totalCompra).Where(v => v.Status == true).ToList();
                return Ok(venda);
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar venda"});
            }
            
        }

        [HttpGet("nome/{data}")]
        public IActionResult ListarVendaData(DateTime data)
        {
            try{
                Venda venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).First(v => v.dataCompra == data);
                if(venda.Status)
                {
                    return Ok(venda);
                }else{
                     Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Venda deletada"}); 
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("forne/{fornecedor}")]  //listar as vendas pelo ID do fornecedor
        public IActionResult ListarFornecedor(int fornecedor)
        {
            if(fornecedor > 0)
            {
                try{
                    var venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).Where(v => v.Status == true && v.Fornecedor.id == fornecedor).ToList();
                    try{
                        return Ok(venda);
                    }catch{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Erro ao retornar"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Id invalido"});
            }
        }

        [HttpGet("cliente/{cliente}")]  //listar as vendas pelo ID do cliente
        public IActionResult ListarCliente(int cliente)
        {
            if(cliente > 0)
            {
                try{
                    var venda = database.Vendas.Include(f => f.Fornecedor).Include(c => c.cliente).Where(v => v.Status == true && v.cliente.id == cliente).ToList();
                    return Ok(venda);
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Id invalido"});
            }
        }


        [HttpPost]
        public IActionResult Cadastrar([FromBody] VendaTemp vTemp)
        {
            //validacao
            //primeiro validar se o campo esta vindo vazio ou não
            if(vTemp.FornecedorId <= 0 )
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Um fornecedor precisa ser inidicado"});
            }
            if(vTemp.ClienteId <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Um cliente precisa ser inidicado"});
            }
            if(vTemp.ProdutosId == null)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Produto invalido"});
            }
            //verificar se o campo existe e se é valido
            try{ //verificar fornecedor
                Fornecedor forn1 = database.Fornecedores.First(f => f.id == vTemp.FornecedorId);
                
                if(forn1.Status == true)  //verificar se fpoi excluido ou não
                {
                    try{  //verificar o cliente
                        Cliente cli1 = database.Clientes.First(c => c.id ==  vTemp.ClienteId);
                    
                        if(cli1.Status == true)  //verificar se o cliente foi deletado ou não
                        {
                            //certo podemos prosseguir então
                        }
                        else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Esse cliente foi deletado"});
                        }
                    }catch{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Cliente Invalido"});
                    }
                }
                else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Esse fornecedor foi deletado"});
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Fornecedor Invalido"});
            }

            //verifacando os produos enviados
            foreach (var prod in vTemp.ProdutosId)
            {
                try{
                    Produto produto = database.Produtos.First(p => p.id == prod);

                    if(produto.Status == true) //verificar se o produto foi deletado ou não
                    {
                        if(produto.quantidade >= 1) //verificar a quantidade desse produto
                        {
                            //certo então ainda temos estoque
                        }
                        else{
                            Response.StatusCode = 400;
                            return NotFound($"Produto {prod} indisponivel");
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return NotFound($"Produto {prod} retirado do catalago");
                    } 
                }catch{
                    Response.StatusCode = 400;
                    //return new ObjectResult("Produto não encontrado");
                    return NotFound($"Produto {prod} não encontrado"); //trazer qual produto eu não achei
                }   
            }

            //cadastrar - caminho feliz
            try{
                Venda ve = new Venda();

                ve.Fornecedor = database.Fornecedores.First(f => f.id == vTemp.FornecedorId); //vai dar erro neh, mas o fornecedor indicado
                ve.cliente = database.Clientes.First(c => c.id ==  vTemp.ClienteId); //cliente que fez a compra
                ve.dataCompra = DateTime.Now; //nao precisa digitar
                ve.Status = true;  //nao precisa digitar
            
                database.Vendas.Add(ve);
                database.SaveChanges();

                float calculo = 0;
                foreach(var pro in vTemp.ProdutosId)
                {
                    //relacionamento de venda e produto
                    VendaProduto vp = new VendaProduto();

                    vp.Produto = database.Produtos.First(p => p.id == pro);
                    vp.Venda = database.Vendas.First(v => v.id == ve.id);

                    database.VendasProdutos.Add(vp);
                    database.SaveChanges();

                    //calculo do total da compra
                    Produto produto = database.Produtos.First(p => p.id == pro);

                    produto.quantidade = produto.quantidade - 1; //atualizar a qunatidade do produto
                    database.SaveChanges();

                    if(produto.promocao == false) //verificar a existencia de promocao
                    {
                        //vTemp.totalCompra = vTemp.totalCompra + produto.valor;
                        calculo = calculo + produto.valor;
                    }
                    else
                    {
                        //vTemp.totalCompra = vTemp.totalCompra + produto.valorPromo;
                        calculo = calculo + produto.valorPromo;
                    }
                        
                }

                //achar a venda que estamos fazendo e guardar nela o total da compra - calculo
                Venda ven = database.Vendas.First(v => v.id == ve.id);
                ven.totalCompra = calculo;
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Cadastrado com sucesso");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult("Erro ao cadastrar");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            if(id > 0)
            {
                try{
                    Venda venda =  database.Vendas.First(v => v.id == id);

                    if(venda.Status)
                    {
                        try{
                            venda.Status = false;
                            database.SaveChanges();

                            Response.StatusCode = 201;
                            return new ObjectResult("Venda deletada");
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Erro ao tentar deletar"});
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Essa venda já foi deletada"}); 
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Venda não encontrada"});
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Id invalido"});
            }
        }

        [HttpPatch]
        public IActionResult EditarParcial([FromBody] VendaTemp venda)
        {
            if(venda.id > 0) //comecar verificando o id que esta vindo
            {
                try{ //tentar achar
                    var ven = database.Vendas.First(v => v.id == venda.id);

                    if(ven != null) //verificar o que está vindo do banco
                    {
                        if(ven.Status) //verificar se essa venda foi deletada ou não
                        {
                            try{  //tentar fazer o caminho feliz
                                ven.totalCompra = venda.totalCompra > 0 ? venda.totalCompra : ven.totalCompra;
                                ven.Fornecedor = database.Fornecedores.First(f => f.id == venda.FornecedorId) != null ? database.Fornecedores.First(f => f.id == venda.FornecedorId) : ven.Fornecedor;
                                ven.cliente = database.Clientes.First(c => c.id == venda.ClienteId) != null ? database.Clientes.First(c => c.id == venda.ClienteId) : ven.cliente;
                                ven.dataCompra = venda.dataCompra != null ? venda.dataCompra : ven.dataCompra;
                                //falta editar os produtos

                                database.SaveChanges();

                                Response.StatusCode = 200;
                                return new ObjectResult(new {msg = "Edição feita com sucesso"});
                            }catch{
                                Response.StatusCode = 400;
                                return new ObjectResult(new {msg = "Erro ao editar venda"});
                            }
                        }
                        else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Esse produto já foi deletado"}); 
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Erro ao retornar produto"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id inválido!"});
            }
        }

        [HttpPut]
        public IActionResult EditarTudo([FromBody] VendaTemp venda)
        {
            if(venda.id > 0)
            {
                try{
                    var ven = database.Vendas.First(v => v.id == venda.id);

                    if(ven != null)
                    {
                        if(ven.Status)
                        {
                            //validacao
                            //primeiro validar se o campo esta vindo vazio ou não
                            if(venda.dataCompra == null)
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Está faltando indicar uma data"});
                            }
                            if(venda.FornecedorId <= 0 )
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Um fornecedor precisa ser inidicado"});
                            }
                            if(venda.ClienteId <= 0)
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Um cliente precisa ser inidicado"});
                            }
                            if(venda.ProdutosId == null)
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Produto está vazio"});
                            }
                            //verificar se o campo existe e se é valido
                            try{ //verificar fornecedor
                                Fornecedor forn1 = database.Fornecedores.First(f => f.id == venda.FornecedorId);
                                
                                if(forn1.Status == true)  //verificar se fpoi excluido ou não
                                {
                                    try{  //verificar o cliente
                                        Cliente cli1 = database.Clientes.First(c => c.id ==  venda.ClienteId);
                                    
                                        if(cli1.Status == true)  //verificar se o cliente foi deletado ou não
                                        {
                                            //certo podemos prosseguir então
                                        }
                                        else{
                                            Response.StatusCode = 400;
                                            return new ObjectResult(new{msg = "Esse cliente foi deletado"});
                                        }
                                    }catch{
                                        Response.StatusCode = 400;
                                        return new ObjectResult(new{msg = "Cliente Invalido"});
                                    }
                                }
                                else{
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "Esse fornecedor foi deletado"});
                                }
                            }catch{
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Fornecedor Invalido"});
                            }
                            //procurar os produtos
                            foreach (var prod in venda.ProdutosId)
                            {
                                try{
                                    Produto produto = database.Produtos.First(p => p.id == prod);

                                    if(produto.Status == true) //verificar se o produto foi deletado ou não
                                    {
                                        if(produto.quantidade >= 1) //verificar a quantidade desse produto
                                        {
                                            //certo então ainda temos estoque
                                        }
                                        else{
                                            Response.StatusCode = 400;
                                            //return new ObjectResult("Produto indisponivel");
                                            return NotFound($"Produto {prod} indisponivel");
                                        }
                                    }
                                    else{
                                        Response.StatusCode = 400;
                                        //return new ObjectResult("Produto retirado do catalago");
                                        return NotFound($"Produto {prod} retirado do catalago");
                                    } 
                                }catch{
                                    Response.StatusCode = 400;
                                    return NotFound($"Produto {prod} não encontrado");
                                }   
                            }

                            if(venda.TotalManual > 0) //verificando se ele passou alguma opcao de total compra
                            {
                                if(venda.TotalManual > 3) //verificar se é um valor valido o que foi passado
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "O total manual tem que ser 1, 2 ou 3"});  
                                }
                                if(venda.totalCompra < 1)  //verificar se tem algum valor em tota da compra
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "Está faltando o total da compra"});
                                }
                            }

                            if(venda.TotalManual == 0 && venda.totalCompra > 1) //verificar se foi passado um valor em total de compra sem uma opcao no manual
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "O que voce pretende fazer apenas com o total da compra? Passe uma opcao em TotalManual"});
                            }

                            if(venda.TotalManual < 0)  //verificar se  foi passado um valor abaixo de zero
                            {
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "Opcao de total de compra invalida, digite o valor 1, 2 ou 3 no campo TotalManual"});
                            }

                            //caminho feliz
                            try{
                                ven.dataCompra = venda.dataCompra;
                                ven.Fornecedor = database.Fornecedores.First(f => f.id == venda.FornecedorId);
                                ven.cliente = database.Clientes.First(c => c.id == venda.ClienteId);
                                
                                if(venda.TotalManual >= 0) //se tiver sido passado uma opcao vai ser atribuido um calculo ao total de compra
                                {
                                    if(venda.TotalManual == 1) //igual a um é para somar o valor, ao valor existente 
                                    {
                                        ven.totalCompra = ven.totalCompra + venda.totalCompra;
                                    }else if(venda.TotalManual == 2) //subtrair o valor que está passando em total da compra ao existente
                                    {
                                        ven.totalCompra = ven.totalCompra - venda.totalCompra;
                                    }else if(venda.TotalManual == 3) //subtituir o valor existente
                                    {
                                        ven.totalCompra = venda.totalCompra;
                                    }
                                }

                                database.SaveChanges();
                                //tranformar todas as vendas desse id em uma lista
                                var vepo = database.VendasProdutos.Where(v => v.VendaID == venda.id).ToList();

                                foreach (var venpro in vepo) //percorrer essas vendas
                                {
                                    foreach(var pro in venda.ProdutosId) //pecorrer os produtos enviados
                                    {
                                        if(venpro.ProdutoID == pro)
                                        {
                                            //já esta cadastrado
                                        }
                                        else{
                                            //cria um relacionamento para esse novo produto
                                            VendaProduto vp = new VendaProduto();

                                            vp.Produto = database.Produtos.First(p => p.id == pro);
                                            vp.Venda = database.Vendas.First(v => v.id == venda.id);

                                            database.VendasProdutos.Add(vp);
                                            database.SaveChanges();

                                            //calculo do adiconal da compra
                                            Produto produto = database.Produtos.First(p => p.id == pro);

                                            produto.quantidade = produto.quantidade - 1; //atualizar a qunatidade do produto
                                            database.SaveChanges();

                                            float calculo = 0;
                                            if(produto.promocao == false) //verificar a existencia de promocao
                                            {
                                                calculo = calculo + produto.valor;
                                            }
                                            else
                                            {
                                                calculo = calculo + produto.valorPromo;
                                            }

                                            Venda venda1 = database.Vendas.First(v => v.id == ven.id);
                                            venda1.totalCompra = venda1.totalCompra + calculo;
                                            database.SaveChanges();
                                        }
                                    }
                                }

                                Response.StatusCode = 200;
                                return new ObjectResult(new {msg = "Edição feita com sucesso"});
                            }catch{
                                Response.StatusCode = 400;
                                return new ObjectResult(new {msg = "Erro ao editar venda"});
                            }
                        }
                        else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new {msg = "Venda deletada"});
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Venda não encontrada"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id não encontrado"});
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id inválido!"});
            }
        }

        public class VendaTemp
        {
            public long id { get; set; }
            public int FornecedorId {get; set;} //id de fornecedor
            public Fornecedor Fornecedor { get; set; }  //relacionbamento com fornecedor
            public int ClienteId {get; set;} //id de cliente
            public Cliente cliente { get; set; }  //relacionamento com cliente
            public List<Produto> Produto { get; set; }  //lista de produtos
            public List<int> ProdutosId {get; set;} //lista de id de produto
            public virtual List<VendaProduto> Produtos {get; set;}  //relacionamento com produtos
            public float totalCompra { get; set; }
            public DateTime dataCompra { get; set; }
            public bool Status {get; set;}  //delecao boleana
            public int TotalManual {get;set;} //caso a pessoa queria inserir o total manualmente
        }
    }
}