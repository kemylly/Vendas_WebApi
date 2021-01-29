using System;
using System.Linq;
using desafio_api.Data;
using desafio_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using desafio_api.HATEOAS;

namespace desafio_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.Hateoas Hateoas;
        
        
        //conexao com o banco
        public ClienteController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new HATEOAS.Hateoas("localhost:5001/api/Fornecedor");
            Hateoas.AddAction("GET_INFO","GET");
            Hateoas.AddAction("DELETE_PRODUCT","DELETE");
        }

        [HttpGet]
        public IActionResult ListarCliente() //listar todos os clientes
        {
            try{
                var cliente = database.Clientes.Where(c => c.Status == true).ToList();
                try{
                    List<ClienteContainer> clienteHateoas = new List<ClienteContainer>();
                
                    foreach (var cli in cliente)
                    {
                        ClienteContainer clienteContainer = new ClienteContainer();
                        clienteContainer.cliente = cli;
                        clienteContainer.links = Hateoas.GetActions(cli.id.ToString());
                        clienteHateoas.Add(clienteContainer);
                    }
                    
                    return Ok(clienteHateoas);
                }catch{
                    return Ok(cliente);
                }
                
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar clientes"}); 
            }
        }

        [HttpGet("{id}")]
        public IActionResult ListarCliente(int id) //encontrar um cliente por id
        {
            try{ //verificar se o cliente existe
                Cliente cliente = database.Clientes.First(c => c.id == id);

                if(cliente.Status == true){ //verificar se o cliente encontrado foi deletado ou não
                    try{
                        ClienteContainer clienteHateoas = new ClienteContainer();
                        clienteHateoas.cliente = cliente;
                        clienteHateoas.links = Hateoas.GetActions(cliente.id.ToString());

                        return Ok(clienteHateoas);
                    }catch{
                        return Ok(cliente);
                    }
                    
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Cliente deletado"}); 
                }
                
            }catch{ //esse cliente não existe
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("asc")]
        public IActionResult Crescente() //listar em ordem crescente
        {
            try{
                var cliente = database.Clientes.OrderBy(c => c.nome).Where(c => c.Status == true).ToList();
                try{
                    List<ClienteContainer> clienteHateoas = new List<ClienteContainer>();
                
                    foreach (var cli in cliente)
                    {
                        ClienteContainer clienteContainer = new ClienteContainer();
                        clienteContainer.cliente = cli;
                        clienteContainer.links = Hateoas.GetActions(cli.id.ToString());
                        clienteHateoas.Add(clienteContainer);
                    }
                    
                    return Ok(clienteHateoas);
                }catch{
                    return Ok(cliente);
                }
                
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar clientes"}); 
            }
        }

        [HttpGet("desc")]
        public IActionResult Decrescente() //listar em ordem decrescente
        {
            try{
                var cliente = database.Clientes.OrderByDescending(c => c.nome).Where(c => c.Status == true).ToList();
                try{
                    List<ClienteContainer> clienteHateoas = new List<ClienteContainer>();
                
                    foreach (var cli in cliente)
                    {
                        ClienteContainer clienteContainer = new ClienteContainer();
                        clienteContainer.cliente = cli;
                        clienteContainer.links = Hateoas.GetActions(cli.id.ToString());
                        clienteHateoas.Add(clienteContainer);
                    }
                    
                    return Ok(clienteHateoas);
                }catch{
                    return Ok(cliente);
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao retornar clientes"}); 
            }
        }

        [HttpGet("nome/{nome}")] //coloque o nome todo digitado
        public IActionResult ListarClienteNome(string nome) //encontrar um cliente por nome
        {
            try{ //verificar se esse nome existe
                Cliente cliente = database.Clientes.First(c => c.nome.ToUpper() == nome.ToUpper());

                if(cliente.Status == true){ //verificar se o cliente encontrado foi deletado ou não
                    try{
                        ClienteContainer clienteHateoas = new ClienteContainer();
                        clienteHateoas.cliente = cliente;
                        clienteHateoas.links = Hateoas.GetActions(cliente.id.ToString());

                        return Ok(clienteHateoas);
                    }catch{
                        return Ok(cliente);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Cliente deletado"}); 
                }

            }catch{ //esse não existe
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("email/{email}")] //coloque email para pesquisar
        public IActionResult ListarClienteEmail(string email)
        {
            try{
                Cliente cliente = database.Clientes.FirstOrDefault(e => e.email.ToUpper() == email.ToUpper());

                if(cliente.Status)  //verificando se esse cleinet foi deletado ou não
                {
                    try{
                        ClienteContainer clienteHateoas = new ClienteContainer();
                        clienteHateoas.cliente = cliente;
                        clienteHateoas.links = Hateoas.GetActions(cliente.id.ToString());

                        return Ok(clienteHateoas);
                    }catch{
                        return Ok(cliente);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Cliente deletado"}); 
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"}); 
            }
        }

        [HttpGet("cpf/{cpf}")]  //cloque o cpf que deseja encontrar
        public IActionResult ListarClienteCpf(string cpf)
        {
            try{
                //verificar o cpf tirando as possiveis coisas a mais que podem vir nele
                Cliente cliente = database.Clientes.FirstOrDefault(c => c.documento == cpf.Trim().Replace(".", "").Replace("-", "").Replace("/", ""));

                if(cliente.Status)
                {
                    try{
                        ClienteContainer clienteHateoas = new ClienteContainer();
                        clienteHateoas.cliente = cliente;
                        clienteHateoas.links = Hateoas.GetActions(cliente.id.ToString());

                        return Ok(clienteHateoas);
                    }catch{
                        return Ok(cliente);
                    }
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Cliente deletado"}); 
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado, revise o conteudo!"});
            }
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] ClienteTemp cTemp)
        {
            //validação
            //validar nome
            if(cTemp.nome == null || cTemp.senha == null || cTemp.documento == null|| cTemp.email == null)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Há campos faltando revise"});
            }
            if(cTemp.nome.Length <= 1 || cTemp.nome.Length > 150)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }

            //validar email
            Regex r = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            if(r.IsMatch (cTemp.email)) //verificar o email
            {
                //verificar se tem um email igual já cadasrado
                var email = database.Clientes.FirstOrDefault(u => u.email == cTemp.email);
                if(email != null)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Email já cadastrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Email Invalido"});
            }
            
            //Validador de senha
            if(cTemp.senha.Length <= 6 || cTemp.senha.Length > 100)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Senha Invalida"});
            }


            //considerar e a entrada de cpf
            int[] multi1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 }; //calculo 1
            int[] multi2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };  //calculo 2
            cTemp.documento = cTemp.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
            if(cTemp.documento.Length != 11) //vrificando a quantidade digitada logo de inicio
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "CPF Invalido"});
            }
            for(int c = 0; c < 10; c++)
            {
                if(c.ToString().PadLeft(11, char.Parse(c.ToString())) == cTemp.documento){
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "CPF Invalido"});
                }
            }
            string tempCpf = cTemp.documento.Substring(0, 9);  //trabando com uma variavel temporaria para os calculos
            int soma = 0;
            for(int c = 0; c < 9; c++)
            {
                soma += int.Parse(tempCpf[c].ToString()) * multi1[c];
            }
            int resto = soma % 11;
            if(resto < 2)
            {
                resto = 0;
            }else{
                resto = 11 - resto;
            }
            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for(int c = 0; c < 10; c++)
            {
                soma += int.Parse(tempCpf[c].ToString()) * multi2[c];
            }
            resto = soma % 11;
            if(resto < 2){
                resto = 0;
            }else{
                resto = 11 - resto;
            }
            digito = digito + resto.ToString();
            if(cTemp.documento.EndsWith(digito) == false)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "CPF Invalido"}); 
            }
            try{
                var cpf = database.Clientes.FirstOrDefault(c => c.documento == cTemp.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", ""));
                if(cpf != null) //verificar se alguem já e cadastrou com esse cpf
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "CPF já cadastrado"});
                }
            }catch { }
            

            //chamando a criptografia
            MD5 senhaHash = MD5.Create();
            Hash senhaCp = new Hash(senhaHash);

            //cadastrando cliente
            try{
               Cliente cl = new Cliente();
               cl.nome = cTemp.nome;
               cl.email = cTemp.email;
               cl.senha = senhaCp.CriptografarSenha(cTemp.senha);
               cl.documento = cTemp.documento;
               cl.dataCadastro = DateTime.Now;
               cl.Status = true;

               database.Clientes.Add(cl);
               database.SaveChanges();

               Response.StatusCode = 201;
               return new ObjectResult("Cadastrado com sucesso");
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult("Erro ao cadastrar");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)  //deletar um cliente
        {
            if(id > 0) //verificar o que está vindo
            {
                try{ //tentar encontrar esse id
                    Cliente cli = database.Clientes.First(c => c.id == id);

                    if(cli.Status) //verifcar se já foi deletado
                    {
                        try{ //caminho feliz tentando deletar
                           
                            cli.Status = false;
                            //database.Clientes.Remove(cli);
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
                        return new ObjectResult(new{msg = "Esse cliente já foi deletado"}); 
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
        public IActionResult EditarParcial([FromBody] ClienteTemp cliente)  //editar parcialmente um cliente usando o metodo patch
        {
            if(cliente.id > 0) //verificando id recebido do body
            {
                try{ //procurando id no banco
                    var cli = database.Clientes.First(c => c.id == cliente.id);

                    //chamando a criptografia
                    MD5 senhaHash = MD5.Create();
                    Hash senhaCp = new Hash(senhaHash);

                    if(cli != null){ // verificando o que veio do banco
                        cli.nome = cliente.nome != null ? cliente.nome : cli.nome;
                        cli.email = cliente.email != null ? cliente.email : cli.email;
                        cli.senha = senhaCp.CriptografarSenha(cliente.senha) != null ? senhaCp.CriptografarSenha(cliente.senha) : cli.senha;
                        cli.documento = cliente.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", "") != null ? cliente.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", "") : cli.documento;
                        cli.dataCadastro = cliente.dataCadastro != null ? cliente.dataCadastro : cli.dataCadastro;
                        
                        database.SaveChanges();

                        Response.StatusCode = 200;
                        return new ObjectResult(new {msg = "Edição feita com sucesso"});
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Erro ao editar cliente"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Cliente não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id de cliente é inválido!"});
            }
            //return Ok();
        }
        
        [HttpPut]
        public IActionResult EditarTudo([FromBody] ClienteTemp cliente)  //editar por inteiro um cliente, usando o metodod put
        {
            if(cliente.id > 0)
            {
                try{
                    var cli = database.Clientes.First(c => c.id == cliente.id);
                    if(cli != null)
                    {
                        //validação
                        Regex r = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
                        if(r.IsMatch (cliente.email)) //verificar o email
                        {
                            //verificar se tem um email igual já cadasrado
                            var email = database.Clientes.FirstOrDefault(u => u.email == cliente.email);
                            if(email != null)
                            {
                                if(email.id != cliente.id)
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "Email já cadastrado"});
                                }
                                else{ }
                            }
                        }else{
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Email Invalido"});
                        }

                        //verificar o nome
                        if(cliente.nome.Length <= 1 || cliente.nome.Length > 150)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Nome Invalido"});
                        }

                        //verificar a senha
                        if(cliente.senha.Length <= 6 || cliente.senha.Length > 100)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Senha Invalida"});
                        }

                        //considerar e a entrada de cpf
                        int[] multi1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 }; //calculo 1
                        int[] multi2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };  //calculo 2
                        cliente.documento = cliente.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
                        if(cliente.documento.Length != 11) //vrificando a quantidade digitada logo de inicio
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "CPF Invalido"});
                        }
                        for(int c = 0; c < 10; c++)
                        {
                            if(c.ToString().PadLeft(11, char.Parse(c.ToString())) == cliente.documento){
                                Response.StatusCode = 400;
                                return new ObjectResult(new{msg = "CPF Invalido"});
                            }
                        }
                        string tempCpf = cliente.documento.Substring(0, 9);  //trabando com uma variavel temporaria para os calculos
                        int soma = 0;
                        for(int c = 0; c < 9; c++)
                        {
                            soma += int.Parse(tempCpf[c].ToString()) * multi1[c];
                        }
                        int resto = soma % 11;
                        if(resto < 2)
                        {
                            resto = 0;
                        }else{
                            resto = 11 - resto;
                        }
                        string digito = resto.ToString();
                        tempCpf = tempCpf + digito;
                        soma = 0;
                        for(int c = 0; c < 10; c++)
                        {
                            soma += int.Parse(tempCpf[c].ToString()) * multi2[c];
                        }
                        resto = soma % 11;
                        if(resto < 2){
                            resto = 0;
                        }else{
                            resto = 11 - resto;
                        }
                        digito = digito + resto.ToString();
                        if(cliente.documento.EndsWith(digito) == false)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "CPF Invalido"}); 
                        }
                        try{
                            var cpf = database.Clientes.FirstOrDefault(c => c.documento == cliente.documento.Trim().Replace(".", "").Replace("-", "").Replace("/", ""));
                            if(cpf != null) //verificar se alguem já e cadastrou com esse cpf
                            {
                                if(cpf.id != cliente.id)
                                {
                                    Response.StatusCode = 400;
                                    return new ObjectResult(new{msg = "CPF já cadastrado"});
                                }
                                else {} 
                            }
                        }catch { }
                        
                        
                        //verificar a data
                        if(cliente.dataCadastro == null)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new{msg = "Data Invalida"}); 
                        }
                        
                        //chamando criptografia
                        MD5 senhaHash = MD5.Create();
                        Hash senhaCp = new Hash(senhaHash);
                        //editar cliente
                        try{
                            cli.nome = cliente.nome;
                            cli.email = cliente.email.Trim();
                            cli.senha = senhaCp.CriptografarSenha(cliente.senha.Trim());
                            cli.documento = cliente.documento;
                            cli.dataCadastro = cliente.dataCadastro;

                            database.SaveChanges();

                            Response.StatusCode = 200;
                            return new ObjectResult(new {msg = "Edição feita com sucesso"});
                        }catch{
                            Response.StatusCode = 400;
                            return new ObjectResult(new {msg = "Erro ao editar cliente"});
                        }

                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Erro ao recuperar cliente"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Cliente não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id de cliente é inválido!"});
            }
            //return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Cliente credencial)
        {
            try{ // tentar fazer a busca do email no banco
                Cliente cliente = database.Clientes.First(c => c.email.Equals(credencial.email));

                if(cliente != null) //verificar o que veio do banco
                {

                MD5 senhaHash = MD5.Create();
                Hash senhaCp = new Hash(senhaHash);

                    if(cliente.senha.Equals(senhaCp.CriptografarSenha(credencial.senha))){ //verificar a senha digitada
                        //chave de segurança
                        string chaveDeSegurana = "kemylly_cavalcante_santos";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSegurana));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id",cliente.id.ToString())); //claim que guarda o id do usuario
                        claims.Add(new Claim("email",cliente.email)); //pegar o email do usuario e colocar em uma claim
                        if(cliente.email == "admin@gmail.com")
                        {
                            claims.Add(new Claim(ClaimTypes.Role,"Admin")); //pegar tipo do usuario
                        }else{
                            claims.Add(new Claim(ClaimTypes.Role,"Comum")); //pegar tipo do usuario
                        }

                        //criando o token e coisas necessarias
                        var JWT = new JwtSecurityToken(
                            issuer: "produtos.com",  //issuer = quem esta fornecendo o jwt ao usuario 
                            expires: DateTime.Now.AddHours(2), //quando expira
                            audience: "usuario_comum", //para quem esta destinado esse token
                            signingCredentials: credenciaisDeAcesso,  //credenciais de acesso de token
                            claims : claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT)); //gerar token
                    }else{
                         //usuario errou a senha
                        Response.StatusCode = 401;
                        return new ObjectResult(new {msg = "Senha invalida"});
                    }
                }else{
                    Response.StatusCode = 401;  //401 = não autorizado
                    return new ObjectResult(new {msg = "Email invalido"});
                }
            }catch{
                Response.StatusCode = 401;  //401 = não autorizado
                return new ObjectResult(new {msg = "Login invalido"});
            }
        }

        public class ClienteTemp  //classe usada como um DTO - intermediaria
        {
            public int id {get; set;}
            public string nome { get; set; }
            public string email { get; set; }
            public string senha { get; set; }
            public string documento { get; set; }
            public DateTime dataCadastro { get; set; }
            public bool status {get; set;}
        }

        public class ClienteContainer  //classe pare o uso do hateoas
        {
            public Cliente cliente;
            public Link[] links;
        }

    }
}