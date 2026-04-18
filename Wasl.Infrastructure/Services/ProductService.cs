namespace Wasl.Infrastructure.Services
{
    public class ProductService(ICurrentUserService currentUser, IMapper mapper, IUnitOfWork unitOfWork, ILogger<ProductService> logger) : IProductService
    {
        //GetAll
        public async Task<ResultResponse<ProductResponse>> GetAllProduct()
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<ProductResponse>.Failure("Unauthorized");

            ProductSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new ProductSpecification(
                    p => p.SupplierId == Guid.Parse(currentUser.UserId!)
                );
            }
            else if (currentUser.Role == "StoreOwner")
            {
                spec = new ProductSpecification();
            }
            else if (currentUser.Role == "Admin")
            {
                spec = new ProductSpecification();
            }
            else
            {
                return ResultResponse<ProductResponse>.Failure("Unauthorized role");
            }

            var products = await unitOfWork.Repository<Product>().GetAllSpecTrackedAsync(spec);

            if (!products.Any())
                return ResultResponse<ProductResponse>.Failure("No products found");

            var mapped = mapper.Map<List<ProductResponse>>(products);

            return ResultResponse<ProductResponse>.Success(mapped);
        }

        //Add Product
        public async Task<ResultResponse<ProductResponse>> AddProductAsync(AddProductDto addProductDto)
        {
            if (!currentUser.IsAuthenticated || currentUser.Role != "Supplier")
                return ResultResponse<ProductResponse>.Failure("Only suppliers can add products");
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                ActivityType = addProductDto.ActivityType,
                Price = addProductDto.Price,
                AvailableQuantity = addProductDto.AvailableQuantity,
                MinimumOrder = addProductDto.MinimumOrder,
                CreatedAt = DateTime.UtcNow,
                SupplierId = Guid.Parse(currentUser.UserId!),
            };

            await unitOfWork.Repository<Product>().AddAsync(product);
            await unitOfWork.CommitAsync();

            var mapped = mapper.Map<ProductResponse>(product);

            logger.LogInformation("Product {ProductId} created by Supplier {SupplierId}",
                product.Id, currentUser.UserId);

            return ResultResponse<ProductResponse>.Success(mapped);

        }

        // DeleteProduct
        public async Task<ResultResponse<ProductResponse>> DeleteProductAsync(Guid ProductId)
        {
            var spec = new ProductSpecification(ProductId);
            var getProduct = await unitOfWork.Repository<Product>().GetByIdSpecAsync(spec);
            if (getProduct is null) return ResultResponse<ProductResponse>.Failure("Product not found ");
            if (getProduct.SupplierId == Guid.Parse(currentUser.UserId!)) return ResultResponse<ProductResponse>.Failure("Unauthorized to delete this Product ");
            unitOfWork.Repository<Product>().Delete(getProduct);
            await unitOfWork.CommitAsync();
            return ResultResponse<ProductResponse>.Success("Product deleted successfully ");
        }

        //GetProduct
        public async Task<ResultResponse<ProductResponse>> GetProductAsync(Guid ProductId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<ProductResponse>.Failure("Unauthorized");

            ProductSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new ProductSpecification(
                    p => p.SupplierId == Guid.Parse(currentUser.UserId!) && p.Id == ProductId
                );
            }
            else if (currentUser.Role == "StoreOwner")
            {
                spec = new ProductSpecification(
                    p => p.Id == ProductId
                );
            }
            else if (currentUser.Role == "Admin")
            {
                spec = new ProductSpecification(p => p.Id == ProductId);
            }
            else
            {
                return ResultResponse<ProductResponse>.Failure("Unauthorized role");
            }
            var result = await unitOfWork.Repository<Product>().GetByIdSpecAsync(spec);
            if (result is null)
            {
                logger.LogError("Failed to retrieve created Product {ProductId}", ProductId);
                return ResultResponse<ProductResponse>.Failure("Product could not be retrieved ");
            }
            var retriveProduct = new ProductResponse
            {
                Id = result.Id,
                Name = result.Name,
                Price = result.Price,
            };
            return ResultResponse<ProductResponse>.Success(retriveProduct);
        }

        //Update Product
        public async Task<ResultResponse<ProductResponse>> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            if (!currentUser.IsAuthenticated || currentUser.Role != "Supplier")
                return ResultResponse<ProductResponse>.Failure("Unauthorized");

            var spec = new ProductSpecification(updateProductDto.ProductId);
            var getProduct = await unitOfWork.Repository<Product>().GetByIdSpecTrackedAsync(spec);
            if (getProduct is null)
            {
                logger.LogWarning("Product {Product} not found for update", updateProductDto.ProductId);
                return ResultResponse<ProductResponse>.Failure("Product not found ");
            }
            if (getProduct.SupplierId != Guid.Parse(currentUser.UserId!))
            {
                logger.LogWarning("User {UserId} unauthorized to update Product {Product}", currentUser.UserId!, getProduct.Id);
                return ResultResponse<ProductResponse>.Failure("Unauthorized to update this post ");
            }


            getProduct.Price = updateProductDto.Price;
            getProduct.AvailableQuantity = updateProductDto.AvailableQuantity;
            getProduct.MinimumOrder = updateProductDto.MinimumOrder;
            await unitOfWork.CommitAsync();
            var mapped = mapper.Map<ProductResponse>(getProduct);
            return ResultResponse<ProductResponse>.Success(mapped);
        }
    }
}
