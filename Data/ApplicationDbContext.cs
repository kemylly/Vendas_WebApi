using System;
using System.Collections.Generic;
using System.Text;
using desafio_api.Models;
using Microsoft.EntityFrameworkCore;

namespace desafio_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Cliente> Clientes {get; set;}
        public DbSet<Fornecedor> Fornecedores {get; set;} 
        public DbSet<Produto> Produtos {get; set;}               
        public DbSet<Venda> Vendas {get; set;}
        public DbSet<VendaProduto> VendasProdutos {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        //    modelBuilder.Entity<Produto>() //tabela que vao ocorrer
        //     .HasOne<Fornecedor>(bc => bc.Fornecedor) //relaçao que está no produto
        //      .WithMany(b => b.Produtos) //nome da lista que está em fornecedor
        //       .HasForeignKey(bc => bc.FornecedorId); //nome da fk que está em produto

         //definindo uma chave composta de VendaProduto
            modelBuilder.Entity<VendaProduto>()
                .HasKey(x => new {x.VendaID, x.ProdutoID});

          //definido o relacionamento muito para muitos
            modelBuilder.Entity<VendaProduto>()
            .HasOne(bc => bc.Venda) //variavel de vendaproduto virtual
             .WithMany(b => b.Produtos) //variavel lista que esta em venda
              .HasForeignKey(bc => bc.VendaID); //variavel id de venda produto
                modelBuilder.Entity<VendaProduto>()
                .HasOne(bc => bc.Produto)
                    .WithMany(c => c.Vendas)
                        .HasForeignKey(bc => bc.ProdutoID);
        }
    }
}