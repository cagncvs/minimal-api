using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private static List<Veiculo> veiculos = new List<Veiculo>(){
        new Veiculo{
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2015
        },
        new Veiculo{
            Id = 2,
            Nome = "HB20",
            Marca = "Hyundai",
            Ano = 2022
        },
        new Veiculo{
            Id = 3,
            Nome = "Uno",
            Marca = "Fiat",
            Ano = 2024
        }
    };

    public void Apagar(Veiculo veiculo)
    {
        veiculos.RemoveAll(v => v.Id == veiculo.Id);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var existente = veiculos.FirstOrDefault(v => v.Id == veiculo.Id);
        if (existente == null)
            return;

        existente.Nome = veiculo.Nome;
        existente.Marca = veiculo.Marca;
        existente.Ano = veiculo.Ano;
    }

       Veiculo? IVeiculoServico.BuscaPorId(int id)
    {
        return veiculos.Find(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count > 0 ? veiculos.Max(v => v.Id) + 1 : 1;
        veiculos.Add(veiculo);
    }


    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        return veiculos;
    }
}