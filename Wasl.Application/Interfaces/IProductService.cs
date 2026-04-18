namespace Wasl.Application.Interfaces
{
    public interface IProductService
    {
        // Get All Product
        Task<ResultResponse<ProductResponse>> GetAllProduct();

        //GetProduct
        Task<ResultResponse<ProductResponse>> GetProductAsync(Guid ProductId);

        //Add Product
        Task<ResultResponse<ProductResponse>> AddProductAsync(AddProductDto addProductDto);

        //UpdateProduct
        Task<ResultResponse<ProductResponse>> UpdateProductAsync(UpdateProductDto updateProductDto);

        //Delete Product
        Task<ResultResponse<ProductResponse>> DeleteProductAsync(Guid ProductId);

    }
}
