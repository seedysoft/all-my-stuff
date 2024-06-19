﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Seedysoft.Carburantes.Infrastructure.Data;

#nullable disable

namespace Seedysoft.Carburantes.Infrastructure.Migrations.CarburantesHist
{
    [DbContext(typeof(CarburantesHistDbContext))]
    [Migration("20231022150410_PricesView")]
    partial class PricesView
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.ComunidadAutonoma", b =>
                {
                    b.Property<int>("IdComunidadAutonoma")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreComunidadAutonoma")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdComunidadAutonoma");

                    b.ToTable("ComunidadAutonoma", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.ComunidadAutonomaHist", b =>
                {
                    b.Property<int>("IdComunidadAutonoma")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreComunidadAutonoma")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdComunidadAutonoma", "AtDate");

                    b.ToTable("ComunidadAutonomaHist", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.EstacionProductoPrecio", b =>
                {
                    b.Property<int>("IdEstacion")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdProducto")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CentimosDeEuro")
                        .HasColumnType("INTEGER");

                    b.HasKey("IdEstacion", "IdProducto", "AtDate");

                    b.ToTable("EstacionProductoPrecio", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.EstacionProductoPrecioHist", b =>
                {
                    b.Property<int>("IdEstacion")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdProducto")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CentimosDeEuro")
                        .HasColumnType("INTEGER");

                    b.HasKey("IdEstacion", "IdProducto", "AtDate");

                    b.ToTable("EstacionProductoPrecioHist", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.EstacionServicio", b =>
                {
                    b.Property<int>("IdEstacion")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CodigoPostal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Horario")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IdMunicipio")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Latitud")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Localidad")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LongitudWgs84")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Margen")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Rotulo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdEstacion");

                    b.ToTable("EstacionServicio", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.EstacionServicioHist", b =>
                {
                    b.Property<int>("IdEstacion")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CodigoPostal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Horario")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IdMunicipio")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Latitud")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Localidad")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LongitudWgs84")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Margen")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Rotulo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdEstacion", "AtDate");

                    b.ToTable("EstacionServicioHist", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.Municipio", b =>
                {
                    b.Property<int>("IdMunicipio")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdProvincia")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreMunicipio")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdMunicipio");

                    b.ToTable("Municipio", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.MunicipioHist", b =>
                {
                    b.Property<int>("IdMunicipio")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdProvincia")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreMunicipio")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdMunicipio", "AtDate");

                    b.ToTable("MunicipioHist", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.ProductoPetrolifero", b =>
                {
                    b.Property<int>("IdProducto")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreProducto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NombreProductoAbreviatura")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdProducto");

                    b.ToTable("ProductoPetrolifero", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.ProductoPetroliferoHist", b =>
                {
                    b.Property<int>("IdProducto")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreProducto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NombreProductoAbreviatura")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdProducto", "AtDate");

                    b.ToTable("ProductoPetroliferoHist", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.Provincia", b =>
                {
                    b.Property<int>("IdProvincia")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdComunidadAutonoma")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreProvincia")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdProvincia");

                    b.ToTable("Provincia", (string)null);
                });

            modelBuilder.Entity("Seedysoft.Carburantes.Core.Entities.ProvinciaHist", b =>
                {
                    b.Property<int>("IdProvincia")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AtDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdComunidadAutonoma")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NombreProvincia")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IdProvincia", "AtDate");

                    b.ToTable("ProvinciaHist", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
