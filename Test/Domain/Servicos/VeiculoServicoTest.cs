using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestandoSalvarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2015
        };

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "HB20",
            Marca = "Hyundai",
            Ano = 2022
        };

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual(1, veiculoDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoBuscaPorIdInexistente()
    {
        var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var resultado = veiculoServico.BuscaPorId(999);

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void TestandoListagemDeVeiculos()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculoServico = new VeiculoServico(context);

        veiculoServico.Incluir(new Veiculo
        {
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2015
        });
        veiculoServico.Incluir(new Veiculo
        {
            Nome = "HB20",
            Marca = "Hyundai",
            Ano = 2022
        });

        // Act
        var lista = veiculoServico.Todos(1).ToList();

        // Assert
        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Any(v => v.Nome == "Fiesta"));
        Assert.IsTrue(lista.Any(v => v.Nome == "HB20"));
    }

    [TestMethod]
    public void TestandoAtualizarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Uno",
            Marca = "Fiat",
            Ano = 2024
        };

        veiculoServico.Incluir(veiculo);

        // Act
        veiculo.Nome = "Uno Sporting";
        veiculo.Ano = 2025;
        veiculoServico.Atualizar(veiculo);

        var veiculoAtualizado = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual("Uno Sporting", veiculoAtualizado?.Nome);
        Assert.AreEqual(2025, veiculoAtualizado?.Ano);
    }

    [TestMethod]
    public void TestandoExcluirVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Uno",
            Marca = "Fiat",
            Ano = 2024
        };

        veiculoServico.Incluir(veiculo);

        // Act
        veiculoServico.Apagar(veiculo);
        var veiculosRestantes = veiculoServico.Todos(1).ToList();

        // Assert
        Assert.AreEqual(0, veiculosRestantes.Count);
    }
}
