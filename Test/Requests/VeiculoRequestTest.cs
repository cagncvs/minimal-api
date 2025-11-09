using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    private static string? tokenAdm;

    [ClassInitialize]
    public static async Task ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);

        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores/login", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<MinimalApi.Dominio.ModelViews.AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        tokenAdm = admLogado?.Token;
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarListagemDeVeiculos()
    {
        // Arrange
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        // Act
        var response = await Setup.client.GetAsync("/veiculos");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var lista = JsonSerializer.Deserialize<List<Veiculo>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(lista);
        Assert.IsTrue(lista!.Count > 0);
    }

    [TestMethod]
    public async Task TestarBuscarVeiculoPorIdExistente()
    {
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        var response = await Setup.client.GetAsync("/veiculos/1");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculo);
        Assert.AreEqual(1, veiculo?.Id);
    }

    [TestMethod]
    public async Task TestarCriarNovoVeiculo()
    {
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        var dto = new VeiculoDTO
        {
            Nome = $"CarroTeste_{Guid.NewGuid()}",
            Marca = "Toyota",
            Ano = 2023
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/veiculos", content);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculoCriado);
        Assert.AreEqual(dto.Nome, veiculoCriado?.Nome);
    }

    [TestMethod]
    public async Task TestarCriarNovoVeiculoInvalido()
    {
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        var dto = new VeiculoDTO
        {
            Nome = "",
            Marca = "",
            Ano = 1900 
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/veiculos", content);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task TestarAtualizarVeiculo()
    {
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        var dto = new VeiculoDTO
        {
            Nome = "Fiesta Novo",
            Marca = "Ford",
            Ano = 2024
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await Setup.client.PutAsync("/veiculos/1", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculoAtualizado = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculoAtualizado);
        Assert.AreEqual("Fiesta Novo", veiculoAtualizado?.Nome);
    }

    [TestMethod]
    public async Task TestarExcluirVeiculo()
    {
        Setup.client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdm);

        // Act
        var response = await Setup.client.DeleteAsync("/veiculos/2");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}
