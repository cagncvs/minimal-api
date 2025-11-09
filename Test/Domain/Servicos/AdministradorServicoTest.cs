using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorServicoTest
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
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoBuscaPorIdInexistente()
    {
        var context = CriarContextoDeTeste();
        var administradorServico = new AdministradorServico(context);

        var resultado = administradorServico.BuscaPorId(999);

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void TestandoLoginAdministradorValido()
    {
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "login@teste.com",
            Senha = "123",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorServico(context);
        administradorServico.Incluir(adm);

        var loginDTO = new LoginDTO { Email = "login@teste.com", Senha = "123" };
        var admLogado = administradorServico.Login(loginDTO);

        Assert.IsNotNull(admLogado);
        Assert.AreEqual("login@teste.com", admLogado?.Email);
    }

    [TestMethod]
    public void TestandoLoginAdministradorInvalido()
    {
        var context = CriarContextoDeTeste();
        var administradorServico = new AdministradorServico(context);

        var loginDTO = new LoginDTO { Email = "naoexiste@teste.com", Senha = "errada" };
        var admLogado = administradorServico.Login(loginDTO);

        Assert.IsNull(admLogado);
    }

    [TestMethod]
    public void TestandoListagemDeAdministradores()
    {
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var administradorServico = new AdministradorServico(context);

        administradorServico.Incluir(new Administrador { Email = "a@a.com", Senha = "1", Perfil = "Adm" });
        administradorServico.Incluir(new Administrador { Email = "b@b.com", Senha = "2", Perfil = "Editor" });

        var lista = administradorServico.Todos(1).ToList();

        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Any(a => a.Email == "a@a.com"));
        Assert.IsTrue(lista.Any(a => a.Email == "b@b.com"));
    }
}