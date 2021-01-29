namespace desafio_api.HATEOAS
{
    public class Link
    {
        public string href {get; set;}
        public string rel {get; set;}
        public string method {get;set;}
        
        public Link(string href, string rel, string method)
        {
            this.href = href;  //acao - link para essa acao
            this.rel = rel;  //o que ee cara faz - ele faz update exemplo
            this.method = method;  // qual metodo devo usar para fazer a acao
        }
    }
}