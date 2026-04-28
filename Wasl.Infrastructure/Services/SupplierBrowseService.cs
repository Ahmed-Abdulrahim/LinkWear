namespace Wasl.Infrastructure.Services
{
    public class SupplierBrowseService(
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager
        ) : ISupplierBrowseService
    {
        // Get all approved suppliers for StoreOwner browsing.

        public async Task<ResultResponse<SupplierResponse>> GetApprovedSuppliersAsync(string? searchQuery = null)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<SupplierResponse>.Failure("Unauthorized");

            var query = userManager.Users
                .Where(u => u.UserType == UserType.Supplier
                         && u.ApprovalStatus == ApprovalStatus.Approved
                         && u.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var search = searchQuery.Trim().ToLower();
                query = query.Where(u =>
                    (u.BusinessName != null && u.BusinessName.ToLower().Contains(search)) ||
                    (u.City != null && u.City.ToLower().Contains(search)));
            }

            var suppliers = await query
                .OrderBy(u => u.BusinessName)
                .Select(u => new SupplierResponse
                {
                    SupplierId = u.Id.ToString(),
                    BusinessName = u.BusinessName,
                    City = u.City,
                    ActivityType = u.ActivityType,
                    BusinessDescription = u.BusinessDescription,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                })
                .ToListAsync();

            if (!suppliers.Any())
                return ResultResponse<SupplierResponse>.Failure("No suppliers found.");

            return ResultResponse<SupplierResponse>.Success(suppliers);
        }

        // Get supplier details by ID for StoreOwner viewing.
        public async Task<ResultResponse<SupplierResponse>> GetSupplierByIdAsync(Guid supplierId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<SupplierResponse>.Failure("Unauthorized");

            var supplier = await userManager.Users
                .Where(u => u.Id == supplierId
                         && u.UserType == UserType.Supplier
                         && u.ApprovalStatus == ApprovalStatus.Approved
                         && u.IsDeleted != true)
                .Select(u => new SupplierResponse
                {
                    SupplierId = u.Id.ToString(),
                    BusinessName = u.BusinessName,
                    City = u.City,
                    ActivityType = u.ActivityType,
                    BusinessDescription = u.BusinessDescription,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                })
                .FirstOrDefaultAsync();

            if (supplier is null)
                return ResultResponse<SupplierResponse>.Failure("Supplier not found.");

            return ResultResponse<SupplierResponse>.Success(supplier);
        }

        // Get dashboard statistics for the logged-in supplier.
        public async Task<ResultResponse<DashboardResponse>> GetSupplierDashboardAsync()
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<DashboardResponse>.Failure("Unauthorized");

            if (currentUser.Role != "Supplier")
                return ResultResponse<DashboardResponse>.Failure("Unauthorized role");

            var supplierId = Guid.Parse(currentUser.UserId!);

            // Product count
            var productSpec = new ProductSpecification(p => p.SupplierId == supplierId);
            var products = await unitOfWork.Repository<Product>().GetAllSpecTrackedAsync(productSpec);
            var productCount = products.Count();

            // Order counts
            var orderSpec = new OrderSpecification(o => o.SupplierId == supplierId);
            var orders = await unitOfWork.Repository<Order>().GetAllSpecTrackedAsync(orderSpec);
            var orderList = orders.ToList();

            var currentOrderCount = orderList.Count(o =>
                o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled);

            var newOrderCount = orderList.Count(o => o.Status == OrderStatus.Pending);

            // Recent orders (last 10)
            var recentOrders = orderList
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber ?? string.Empty,
                    StoreOwnerName = o.StoreOwner?.BusinessName,
                    ProductName = o.Items?.FirstOrDefault()?.Product?.Name,
                    Quantity = o.Items?.Sum(i => i.Quantity) ?? 0,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                })
                .ToList();

            var dashboard = new DashboardResponse
            {
                ProductCount = productCount,
                CurrentOrderCount = currentOrderCount,
                NewOrderCount = newOrderCount,
                RecentOrders = recentOrders,
            };

            return ResultResponse<DashboardResponse>.Success(dashboard);
        }

        // Get products for a specific approved supplier (for merchant browsing).
        public async Task<ResultResponse<ProductResponse>> GetSupplierProductsAsync(Guid supplierId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<ProductResponse>.Failure("Unauthorized");

            // Verify the supplier exists and is approved
            var supplier = await userManager.Users
                .FirstOrDefaultAsync(u => u.Id == supplierId
                         && u.UserType == UserType.Supplier
                         && u.ApprovalStatus == ApprovalStatus.Approved
                         && u.IsDeleted != true);

            if (supplier is null)
                return ResultResponse<ProductResponse>.Failure("Supplier not found.");

            var productSpec = new ProductSpecification(p => p.SupplierId == supplierId);
            var products = await unitOfWork.Repository<Product>().GetAllSpecTrackedAsync(productSpec);

            if (!products.Any())
                return ResultResponse<ProductResponse>.Failure("No products found for this supplier.");

            var mapped = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                AvailableQuantity = p.AvailableQuantity,
                MinimumOrder = p.MinimumOrder,
                ActivityType = p.ActivityType,
            }).ToList();

            return ResultResponse<ProductResponse>.Success(mapped);
        }
    }
}
