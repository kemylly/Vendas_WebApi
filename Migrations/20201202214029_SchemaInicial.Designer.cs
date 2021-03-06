﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using desafio_api.Data;

namespace desafio_api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201202214029_SchemaInicial")]
    partial class SchemaInicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("desafio_api.Models.Cliente", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("dataCadastro")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("documento")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("senha")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("desafio_api.Models.Fornecedor", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("cnpj")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Fornecedores");
                });

            modelBuilder.Entity("desafio_api.Models.Produto", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("Fornecedorid")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<long?>("Vendaid")
                        .HasColumnType("bigint");

                    b.Property<string>("categoria")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("codigoProduto")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("imagem")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("promocao")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("quantidade")
                        .HasColumnType("bigint");

                    b.Property<float>("valor")
                        .HasColumnType("float");

                    b.Property<float>("valorPromo")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.HasIndex("Fornecedorid");

                    b.HasIndex("Vendaid");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("desafio_api.Models.Venda", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int?>("Fornecedorid")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("clienteid")
                        .HasColumnType("int");

                    b.Property<DateTime>("dataCompra")
                        .HasColumnType("datetime(6)");

                    b.Property<float>("totalCompra")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.HasIndex("Fornecedorid");

                    b.HasIndex("clienteid");

                    b.ToTable("Vendas");
                });

            modelBuilder.Entity("desafio_api.Models.VendaProduto", b =>
                {
                    b.Property<long>("VendaID")
                        .HasColumnType("bigint");

                    b.Property<int>("ProdutoID")
                        .HasColumnType("int");

                    b.HasKey("VendaID", "ProdutoID");

                    b.HasIndex("ProdutoID");

                    b.ToTable("VendasProdutos");
                });

            modelBuilder.Entity("desafio_api.Models.Produto", b =>
                {
                    b.HasOne("desafio_api.Models.Fornecedor", "Fornecedor")
                        .WithMany("Produtos")
                        .HasForeignKey("Fornecedorid");

                    b.HasOne("desafio_api.Models.Venda", null)
                        .WithMany("Produto")
                        .HasForeignKey("Vendaid");
                });

            modelBuilder.Entity("desafio_api.Models.Venda", b =>
                {
                    b.HasOne("desafio_api.Models.Fornecedor", "Fornecedor")
                        .WithMany()
                        .HasForeignKey("Fornecedorid");

                    b.HasOne("desafio_api.Models.Cliente", "cliente")
                        .WithMany()
                        .HasForeignKey("clienteid");
                });

            modelBuilder.Entity("desafio_api.Models.VendaProduto", b =>
                {
                    b.HasOne("desafio_api.Models.Produto", "Produto")
                        .WithMany("Vendas")
                        .HasForeignKey("ProdutoID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("desafio_api.Models.Venda", "Venda")
                        .WithMany("Produtos")
                        .HasForeignKey("VendaID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
