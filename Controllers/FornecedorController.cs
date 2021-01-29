using System;
using System.Linq;
using desafio_api.Data;
using desafio_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using desafio_api.HATEOAS;

namespace desafio_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class FornecedorController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.Hateoas Hateoas;
        //conexao com o banco
        public FornecedorController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new HATEOAS.Hateoas("localhost:5001/api/Fornecedor");
            Hateoas.AddAction("GET_INFO","GET");
            Hateoas.AddAction("DELETE_PRODUCT","DELETE");
            //Hateoas.AddAction("EDIT_PRODUCT","PATCH");
        }

        [HttpGet]
        public IActionResult ListarFornecedor() //listar todos os fornecedores
        {
            try{
                var fornecedor = database.Fornecedores.Include(p => p.Produtos).Where(f => f.Status == true).ToList();
                try{
                    List<FornecedorContainer> fornecedorHateoas = new List<FornecedorContainer>();
                
                    foreach (var forn in fornecedor)
                    {
                        FornecedorContainer fornecedorContainer = new FornecedorContainer();
                        fornecedorContainer.fornecedor = forn;
                        fornecedorContainer.links = Hateoas.GetActions(forn.id.ToString());
                        fornecedorHateoas.Add(fornecedorContainer);
                    } 

                    return Ok(fornecedorHateoas);
                }catch{ 

                    return Ok(fornecedor);
                }

            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar fornecedor"});
            }
        }

        [HttpGet("{id}")]
        public IActionResult ListarFornecedor(int id) //encontrar um fornecedor por id
        {
            try{ //verificar se o fornecedor existe
                Fornecedor fornecedor = database.Fornecedores.Include(p => p.Produtos).First(c => c.id == id);

                if(fornecedor.Status == true){ //verificar se o fornecedor foi deletado
                    try{
                        FornecedorContainer fornecedorHateoas = new FornecedorContainer();
                        fornecedorHateoas.fornecedor = fornecedor;
                        fornecedorHateoas.links = Hateoas.GetActions(fornecedor.id.ToString());

                        return Ok(fornecedorHateoas);
                    }catch{
                        return Ok(fornecedor);
                    }
                    
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Fornecedor deletado"}); 
                }
               
            }catch{ //esse fornecedor não existe
               Response.StatusCode = 400;
               return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("asc")]
        public IActionResult Crescente() //listar em ordem crescente
        {
            try{
                var fornecedor = database.Fornecedores.Include(p => p.Produtos).OrderBy(c => c.nome).Where(f => f.Status == true).ToList();
                try{
                    List<FornecedorContainer> fornecedorHateoas = new List<FornecedorContainer>();
                
                    foreach (var forn in fornecedor)
                    {
                        FornecedorContainer fornecedorContainer = new FornecedorContainer();
                        fornecedorContainer.fornecedor = forn;
                        fornecedorContainer.links = Hateoas.GetActions(forn.id.ToString());
                        fornecedorHateoas.Add(fornecedorContainer);
                    }

                    return Ok(fornecedorHateoas);
                }catch{
                    return Ok(fornecedor);
                }
                
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar fornecedor"});
            }  
        }

        [HttpGet("desc")]
        public IActionResult Decrescente() //listar em ordem decrescente
        {
            try{
                var fornecedor = database.Fornecedores.Include(p => p.Produtos).OrderByDescending(c => c.nome).Where(f => f.Status == true).ToList();
                try{
                    List<FornecedorContainer> fornecedorHateoas = new List<FornecedorContainer>();
                
                    foreach (var forn in fornecedor)
                    {
                        FornecedorContainer fornecedorContainer = new FornecedorContainer();
                        fornecedorContainer.fornecedor = forn;
                        fornecedorContainer.links = Hateoas.GetActions(forn.id.ToString());
                        fornecedorHateoas.Add(fornecedorContainer);
                    }
                    
                    return Ok(fornecedorHateoas);
                }catch{

                    return Ok(fornecedor);
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar fornecedor"});
            }
            
        }

        [HttpGet("nome/{nome}")] //coloque o nome todo digitado
        public IActionResult ListarFornecedorNome(string nome) //encontrar um fornecedor por nome
        {
            try{ //verificar se esse nome existe
                Fornecedor fornecedor = database.Fornecedores.Include(p => p.Produtos).FirstOrDefault(c => c.nome.ToUpper() == nome.ToUpper());

                if(fornecedor.Status == true){ //verificar se o fornecedor foi deletado
                    try{
                        FornecedorContainer fornecedorHateoas = new FornecedorContainer();
                        fornecedorHateoas.fornecedor = fornecedor;
                        fornecedorHateoas.links = Hateoas.GetActions(fornecedor.id.ToString());

                        return Ok(fornecedorHateoas);
                    }catch{
                        return Ok(fornecedor);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Fornecedor deletado"}); 
                }
                
            }catch{ //esse não existe
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("cnpj/{cnpj}")] //pesquisar pelo cnpj do cliente
        public IActionResult ListarFornecedorCnpj(string cnpj)
        {
            try{ //verificar a existencia
                Fornecedor fornecedor = database.Fornecedores.Include(p => p.Produtos).FirstOrDefault(f => f.cnpj == cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", ""));
                if(fornecedor.Status)
                {
                    try{
                        FornecedorContainer fornecedorHateoas = new FornecedorContainer();
                        fornecedorHateoas.fornecedor = fornecedor;
                        fornecedorHateoas.links = Hateoas.GetActions(fornecedor.id.ToString());

                        return Ok(fornecedorHateoas);
                    }catch{
                        return Ok(fornecedor);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Fornecedor deletado"});  
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public IActionResult Cadastrar([FromBody] FornecedorTemp fTemp) //cadastrar o fornecedor
        {
            //validacao
            //validar o nome
            if(fTemp.nome.Length <= 1 || fTemp.nome.Length > 150)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }

            //validar cnpj
            int[] multi1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multi2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            fTemp.cnpj = fTemp.cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
            if(fTemp.cnpj.Length != 14)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Cnpj Invalido"});
            }
            string tempCnpj = fTemp.cnpj.Substring(0,12);
            int soma = 0;
            for(int n = 0; n < 12; n++)
            {
                soma += int.Parse(tempCnpj[n].ToString()) * multi1[n];
            }
            int resto = (soma % 11);
            if(resto < 2){
                resto = 0;
            }else{
                resto = 11 - resto;
            }
            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for(int n = 0; n < 13; n++)
            {
                soma += int.Parse(tempCnpj[n].ToString()) * multi2[n];
            }
            resto = (soma % 11);
            if(resto < 2){
                resto = 0;
            }else{
                resto = 11 - resto;
            }
            digito = digito + resto.ToString();
            if(fTemp.cnpj.EndsWith(digito) == false)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Cnpj Invalido"});
            }
            try{
                var cnpj = database.Fornecedores.FirstOrDefault(f => f.cnpj == fTemp.cnpj);
                if(cnpj != null)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Cnpj já cadastrado"});
                }
            }catch{ }

            //cadastrar
            try{
                Fornecedor fo = new Fornecedor();
                fo.nome = fTemp.nome;
                fo.cnpj = fTemp.cnpj;
                fo.Status = true;

                database.Fornecedores.Add(fo);
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Cadastrado com sucesso");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult("Erro ao cadastrar");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles="Admin")]
        public IActionResult Deletar(int id)  //deletar um fornecedor
        {
            if(id > 0)
            {
                try{
                    Fornecedor forne = database.Fornecedores.First(f => f.id == id);

                    if(forne.Status)
                    {
                        try{
                            forne.Status = false;
                            //database.Fornecedores.Remove(forne);
                            database.SaveChanges();

                            //quando eu deletar um fornecedor, vou deletar os produtos dele
                            //não faz sentido eu ter produtos para comprar de um fornecedor que foi deletado
                            var produto = database.Produtos.Where(p => p.Fornecedor == forne).ToList();
                            //produto.Status = false;
                            if(produto != null)
                            {
                                foreach (var prod in produto)
                                {
                                    prod.Status = false;
                                    database.SaveChanges();
                                }
                            }

                            Response.StatusCode = 201;
                            return new ObjectResult("Fornecedor deletado");
                        }catch{
                             Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Erro ao tentar deletar"});
                        }
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new{msg = "Esse fornecedor já foi deletado"}); 
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
        public IActionResult EditarParcial([FromBody] FornecedorTemp fornecedor) //editar parcial
        {
            if(fornecedor.id > 0) //verificar se é um id valido que está vindo
            {
                try{ //tentar encontrar ele no banco
                    var forn = database.Fornecedores.First(f => f.id == fornecedor.id);

                    if(forn != null) //verificicar se está nulo para poder editar
                    {
                         forn.nome = fornecedor.nome != null ? fornecedor.nome : forn.nome;
                         forn.cnpj = fornecedor.cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "") != null ? fornecedor.cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "") : forn.cnpj;

                         database.SaveChanges();

                        Response.StatusCode = 200;
                        return new ObjectResult(new {msg = "Edição feita com sucesso"});
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Erro ao editar"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Fornecedor não encontrado"});
                }
            }
            else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id de fornecedor é inválido!"});
            }
            //return Ok();
        }

        [HttpPut]
        [Authorize(Roles="Admin")]
        public IActionResult EditarTudo([FromBody] FornecedorTemp fornecedor)
        {
            if(fornecedor.id > 0) //verificar se é valido
            {
                try{ //tentar achar
                    var fo = database.Fornecedores.First(f => f.id == fornecedor.id);

                    if(fo != null)
                    {
                        //validar nome
                        if(fornecedor.nome.Length <= 1 || fornecedor.nome.Length > 150)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Nome Invalido"});
                        }

                        //validar cnpj
                        int[] multi1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                        int[] multi2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                        fornecedor.cnpj = fornecedor.cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
                        if(fornecedor.cnpj.Length != 14)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Cnpj Invalido"});
                        }
                        string tempCnpj = fornecedor.cnpj.Substring(0,12);
                        int soma = 0;
                        for(int n = 0; n < 12; n++)
                        {
                            soma += int.Parse(tempCnpj[n].ToString()) * multi1[n];
                        }
                        int resto = (soma % 11);
                        if(resto < 2){
                            resto = 0;
                        }else{
                            resto = 11 - resto;
                        }
                        string digito = resto.ToString();
                        tempCnpj = tempCnpj + digito;
                        soma = 0;
                        for(int n = 0; n < 13; n++)
                        {
                            soma += int.Parse(tempCnpj[n].ToString()) * multi2[n];
                        }
                        resto = (soma % 11);
                        if(resto < 2){
                            resto = 0;
                        }else{
                            resto = 11 - resto;
                        }
                        digito = digito + resto.ToString();
                        if(fornecedor.cnpj.EndsWith(digito) == false)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Cnpj Invalido"});
                        }
                        try{
                            var cnpj = database.Fornecedores.FirstOrDefault(f => f.cnpj == fornecedor.cnpj);
                            if(cnpj != null)
                            {
                                if(cnpj.id != fornecedor.id)
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "Cnpj já cadastrado"});
                                }else { }
                            }
                        }catch{ }
                        
                        
                        try{ //caminho feliz
                            fo.nome = fornecedor.nome;
                            fo.cnpj = fornecedor.cnpj;

                            database.SaveChanges();

                            Response.StatusCode = 200;
                            return new ObjectResult(new {msg = "Edição feita com sucesso"});
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new {msg = "Erro ao editar fornecedor"});
                        }
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Fornecedor não encontrado"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Verifique se todos os campos estão corretos"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id inválido!"});
            }
            //return Ok();
        }


        public class FornecedorTemp
        {
            public int id { get; set; }
            public string nome { get; set; }
            public string cnpj { get; set; }
            public List<Produto> Produtos { get; set; }
        }

        public class FornecedorContainer
        {
            public Fornecedor fornecedor;
            public Link[] links;
        }
    }
}