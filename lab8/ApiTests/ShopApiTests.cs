using ApiTests.API;
using ApiTests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiTests;

[TestClass]
public class ShopApiTests
{
    private static readonly ShopApi ShopApi = new();
    private static readonly List<int> ProductsToDelete = [];

    private static readonly JObject AddProductCases =
        JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\add_product_cases.json"));

    private static readonly JObject EditProductCases =
        JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\edit_product_cases.json"));

    private static async Task<Product?> GetProductById(int id)
    {
        var products = await ShopApi.GetAllProducts();
        return products.FirstOrDefault(p => p.Id == id);
    }

    private static void AssertProductsEqual(Product expected, Product actual)
    {
        Assert.AreEqual(expected.CategoryId, actual.CategoryId,
            $"Product categories are not equal. Expected: {expected.CategoryId}, Actual: {actual.CategoryId}");
        Assert.AreEqual(expected.Title, actual.Title,
            $"Product titles are not equal. Expected: '{expected.Title}', Actual: '{actual.Title}'");
        Assert.AreEqual(expected.Content, actual.Content,
            $"Product content is not equal. Expected: '{expected.Content}', Actual: '{actual.Content}'");
        Assert.AreEqual(expected.Price, actual.Price,
            $"Product price is not equal. Expected: {expected.Price}, Actual: {actual.Price}");
        Assert.AreEqual(expected.OldPrice, actual.OldPrice,
            $"Product old price is not equal. Expected: {expected.OldPrice}, Actual: {actual.OldPrice}");
        Assert.AreEqual(expected.Status, actual.Status,
            $"Product status is not equal. Expected: {expected.Status}, Actual: {actual.Status}");
        Assert.AreEqual(expected.Keywords, actual.Keywords,
            $"Product keywords are not equal. Expected: '{expected.Keywords}', Actual: '{actual.Keywords}'");
        Assert.AreEqual(expected.Description, actual.Description,
            $"Product description is not equal. Expected: '{expected.Description}', Actual: '{actual.Description}'");
        Assert.AreEqual(expected.Hit, actual.Hit,
            $"Product hit status is not equal. Expected: {expected.Hit}, Actual: {actual.Hit}");
    }


    [TestCleanup]
    public async Task TestCleanup()
    {
        foreach (var id in ProductsToDelete)
        {
            await ShopApi.DeleteProduct(id);
            Console.WriteLine($"Deleted product id: {id}");
        }

        ProductsToDelete.Clear();
    }

    [TestMethod]
    [DataRow("valid_with_rus_title")]
    [DataRow("valid")]
    public async Task AddValidProductTest(string key)
    {
        var expectedStatus = 1;

        var product = AddProductCases[key]?.ToObject<Product>();

        var response = await ShopApi.AddProduct(product);
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to add product. Status Code: {response.StatusCode}");

        var jsonResponse =
            JsonConvert.DeserializeObject<AddProductResponse>(response.Content.ReadAsStringAsync().Result);
        Assert.AreEqual(expectedStatus, jsonResponse.Status, "Response status is not equal");

        var addedProduct = await GetProductById(jsonResponse.Id);
        Assert.IsNotNull(addedProduct, "The added product should not be in the list of products");

        ProductsToDelete.Add(jsonResponse.Id);

        AssertProductsEqual(product, addedProduct);
    }

    [TestMethod]
    [DataRow("invalid_by_category_id_less")]
    [DataRow("invalid_by_category_id_more")]
    [DataRow("invalid_by_status_less")]
    [DataRow("invalid_by_status_more")]
    [DataRow("invalid_by_hit_less")]
    [DataRow("invalid_by_hit_more")]
    public async Task AddProductWithInvalidCategoryIdTest(string key)
    {
        var product = AddProductCases[key]?.ToObject<Product>();

        var response = await ShopApi.AddProduct(product);
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to add product. Status Code: {response.StatusCode}");

        var jsonResponse =
            JsonConvert.DeserializeObject<AddProductResponse>(response.Content.ReadAsStringAsync().Result);

        var addedProduct = await GetProductById(jsonResponse.Id);
        if (addedProduct != null) ProductsToDelete.Add(addedProduct.Id);
        
        Assert.IsNull(addedProduct, "The added product should not be in the list of products");
    }

    [TestMethod]
    public async Task AddProductsWithSameTitleTest()
    {
        var expectedStatus = 1;

        List<string> expectedAliases = ["chasiki-tik-tak", "chasiki-tik-tak-0", "chasiki-tik-tak-0-0"];

        var product = AddProductCases["valid"]?.ToObject<Product>();

        foreach (var alias in expectedAliases)
        {
            var response = await ShopApi.AddProduct(product);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to add product. Status Code: {response.StatusCode}");

            var jsonResponse =
                JsonConvert.DeserializeObject<AddProductResponse>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(expectedStatus, jsonResponse.Status,
                $"Response status '{jsonResponse.Status}' is not expected to be '{expectedStatus}'");

            var addedProduct = await GetProductById(jsonResponse.Id);

            ProductsToDelete.Add(addedProduct.Id);

            AssertProductsEqual(product, addedProduct);
            Assert.AreEqual(alias, addedProduct.Alias, "Products are not equal");
        }
    }

    [TestMethod]
    public async Task EditExistingProductTest()
    {
        var product = AddProductCases["valid"]?.ToObject<Product>();
        var addedProductId = await AddProduct(product);

        var editedProduct = EditProductCases["valid_edited"]?.ToObject<Product>() with
        {
            Id = addedProductId
        };

        var jsonEditProductResponse = await ShopApi.EditProduct(editedProduct);
        Assert.IsTrue(jsonEditProductResponse.IsSuccessStatusCode,
            $"Failed to edit product. Status Code: {jsonEditProductResponse.StatusCode}");

        var resultProduct = await GetProductById(editedProduct.Id);
        Assert.IsNotNull(resultProduct, "Product should be in the list of products");

        AssertProductsEqual(resultProduct, editedProduct);
    }

    [TestMethod]
    public async Task EditNotExistingProductTest()
    {
        var product = EditProductCases["valid_with_not_existing_id"]?.ToObject<Product>();

        var response = await ShopApi.EditProduct(product);
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to edit product. Status Code: {response.StatusCode}");

        var resultProduct = await GetProductById(product.Id);
        Assert.IsNull(resultProduct, "Product should not be in the list of products");
    }

    [TestMethod]
    public async Task EditProductWithoutIdTest()
    {
        var expectedStatus = 0;

        var product = EditProductCases["valid_without_id"]?.ToObject<Product>();

        var response = await ShopApi.EditProduct(product);
        Assert.IsTrue(response.IsSuccessStatusCode, "Response should return status code");

        var jsonResponse =
            JsonConvert.DeserializeObject<EditProductResponse>(response.Content.ReadAsStringAsync().Result);
        Assert.AreEqual(expectedStatus, jsonResponse.Status, "Response status is not expected to be equal");
    }

    [TestMethod]
    public async Task DeleteExistingProductTest()
    {
        var expectedStatus = 1;

        var addedProductId = await AddProduct(AddProductCases["valid"]?.ToObject<Product>());

        var response = await ShopApi.DeleteProduct(addedProductId);
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to delete product. Status Code: {response.StatusCode}");

        var jsonResponse =
            JsonConvert.DeserializeObject<DeleteProductResponse>(response.Content.ReadAsStringAsync().Result);
        Assert.AreEqual(expectedStatus, jsonResponse.Status, "Response status is not expected to be equal");
    }

    [TestMethod]
    public async Task DeleteNotExistingProductTest()
    {
        var expectedStatus = 0;
        var id = 9999999;

        var response = await ShopApi.DeleteProduct(id);
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to delete product. Status Code: {response.StatusCode}");

        var jsonResponse =
            JsonConvert.DeserializeObject<DeleteProductResponse>(response.Content.ReadAsStringAsync().Result);
        Assert.AreEqual(expectedStatus, jsonResponse.Status, "Response status is not expected to be equal");
    }

    private static async Task<int> AddProduct(Product product)
    {
        var addProductResponse = await ShopApi.AddProduct(product);
        Assert.IsTrue(addProductResponse.IsSuccessStatusCode,
            $"Failed to add product. Status Code: {addProductResponse.StatusCode}");

        var jsonAddProductResponse =
            JsonConvert.DeserializeObject<AddProductResponse>(addProductResponse.Content.ReadAsStringAsync().Result);
        Assert.AreEqual(1, jsonAddProductResponse.Status, $"Response status is not expected");

        ProductsToDelete.Add(jsonAddProductResponse.Id);

        return jsonAddProductResponse.Id;
    }
}