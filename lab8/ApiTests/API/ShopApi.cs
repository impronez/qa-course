using System.Text;
using ApiTests.Models;
using Newtonsoft.Json;

namespace ApiTests.API;

public class ShopApi
{
    private const string GetProductsUri = "http://shop.qatl.ru/api/products";
    private const string DeleteProductUri = "http://shop.qatl.ru/api/deleteproduct";
    private const string AddProductUri = "http://shop.qatl.ru/api/addproduct";
    private const string EditProductUri = "http://shop.qatl.ru/api/editproduct";
    
    private readonly HttpClient _client = new();

    public async Task<HttpResponseMessage> AddProduct(Product product)
    {
        var jsonContent = JsonConvert.SerializeObject(product);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(AddProductUri, content);
        return response;
    }

    public async Task<HttpResponseMessage> EditProduct(Product product)
    {
        var jsonContent = JsonConvert.SerializeObject(product);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(EditProductUri, content);

        return response;
    }

    public async Task<List<Product>?> GetAllProducts()
    {
        var response = await _client.GetAsync(GetProductsUri);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get products. Status Code: {response.StatusCode}");
        }
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
        
        return products;
    }

    public async Task<HttpResponseMessage> DeleteProduct(int id)
    {
        var response = await _client.GetAsync($"{DeleteProductUri}?id={id}");

        return response;
    }
}