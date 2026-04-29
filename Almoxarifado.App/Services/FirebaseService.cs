using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services
{
    public class FirebaseService : IFirebaseService
    {
        private const string ColecaoProdutos = "Produtos";

        public async Task<List<Produto>> GetProdutosAsync()
        {
            var listaProdutos = new List<Produto>();
            try
            {
                var snapshot = await CrossCloudFirestore.Current.Instance.Collection(ColecaoProdutos).GetAsync();

                foreach (var document in snapshot.Documents)
                {
                    var data = document.Data;

                    var produto = new Produto(
                        id: document.Id,
                        nome: data.ContainsKey("Nome") ? data["Nome"]?.ToString() : string.Empty,
                        marca: data.ContainsKey("Marca") ? data["Marca"]?.ToString() : string.Empty,
                        sku: data.ContainsKey("SKU") ? data["SKU"]?.ToString() : string.Empty,
                        unidade: data.ContainsKey("Unidade") ? data["Unidade"]?.ToString() : string.Empty
                    );

                    if (data.ContainsKey("Ativo") && data["Ativo"] is bool ativo && !ativo)
                    {
                        produto.Desativar();
                    }

                    listaProdutos.Add(produto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return listaProdutos;
        }

        public async Task SalvarProdutoAsync(Produto produto)
        {
            try
            {
                var dadosFirebase = new Dictionary<string, object>
                {
                    { "Nome", produto.Nome },
                    { "Marca", produto.Marca },
                    { "SKU", produto.Sku },
                    { "Unidade", produto.Unidade },
                    { "Ativo", produto.Ativo },
                    { "createdAt", produto.CreatedAt },
                    { "updatedAt", produto.UpdatedAt }
                };

                await CrossCloudFirestore.Current.Instance
                     .Collection(ColecaoProdutos)
                     .Document(produto.Id)
                     .SetAsync(dadosFirebase);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<Usuario> GetUsuarioAsync(string uid)
        {
            return null;
        }

        public async Task<List<Estoque>> GetEstoqueAsync()
        {
            return new List<Estoque>();
        }
    }
}