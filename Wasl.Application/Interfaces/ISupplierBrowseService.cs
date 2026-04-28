namespace Wasl.Application.Interfaces
{
    public interface ISupplierBrowseService
    {
        /// <summary>
        /// Get all approved suppliers for StoreOwner browsing.
        /// </summary>
        Task<ResultResponse<SupplierResponse>> GetApprovedSuppliersAsync(string? searchQuery = null);

        /// <summary>
        /// Get supplier details by ID for StoreOwner viewing.
        /// </summary>
        Task<ResultResponse<SupplierResponse>> GetSupplierByIdAsync(Guid supplierId);

        /// <summary>
        /// Get dashboard statistics for the logged-in supplier.
        /// </summary>
        Task<ResultResponse<DashboardResponse>> GetSupplierDashboardAsync();

        /// <summary>
        /// Get products for a specific supplier (for merchant browsing).
        /// </summary>
        Task<ResultResponse<ProductResponse>> GetSupplierProductsAsync(Guid supplierId);
    }
}
