namespace Wasl.Infrastructure.Services
{

    public class AdminService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, INotificationService notificationService, ILogger<AdminService> logger) : IAdminService
    {

        #region User Management

        public async Task<ResultResponse<UserProfileResponse>> GetAllUserAsync()
        {
            var users = await userManager.Users.OrderBy(r => r.UserName).Select(r => new UserProfileResponse
            {
                UserId = r.Id.ToString(),
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                Status = r.UserType.ToString(),
                BusinessName = r.BusinessName,
                IsDeleted = r.IsDeleted,
                ApprovalStatus = r.ApprovalStatus,

            }).ToListAsync();

            return ResultResponse<UserProfileResponse>.Success(users);
        }

        public async Task<ResultResponse<UserProfileResponse>> DeleteUserAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return ResultResponse<UserProfileResponse>.Failure("User not Found");
            user.IsDeleted = true;
            var deleteUser = await userManager.UpdateAsync(user);
            if (!deleteUser.Succeeded)
            {
                return ResultResponse<UserProfileResponse>.Failure(deleteUser.Errors.First().Description);
            }
            return ResultResponse<UserProfileResponse>.Success("User Deleted Successfully");
        }

        #endregion

        #region Supplier Approval

        /* public async Task<ResultResponse<UserProfileResponse>> GetPendingSuppliersAsync()
         {
             var users = await userManager.Users
                 .Where(u => u.UserType == UserType.Supplier && u.ApprovalStatus == ApprovalStatus.Pending)
                 .ToListAsync();

             if (!users.Any())
                 return ResultResponse<UserProfileResponse>.Failure("No pending Suppliers found.");

             var map = mapper.Map<List<UserProfileResponse>>(users);
             return ResultResponse<UserProfileResponse>.Success(map);
         }

         public async Task<ResultResponse<UserProfileResponse>> ApproveSupplierAsync(Guid supplierId)
         {
             var user = await userManager.Users
                 .Where(u => u.Id == supplierId && u.UserType == UserType.Supplier)
                 .FirstOrDefaultAsync();

             if (user is null)
                 return ResultResponse<UserProfileResponse>.Failure("Supplier not found.");

             if (user.ApprovalStatus != ApprovalStatus.Pending)
                 return ResultResponse<UserProfileResponse>.Failure("Supplier is not in Pending status.");

             user.ApprovalStatus = ApprovalStatus.Approved;
             await userManager.UpdateAsync(user);

             logger.LogInformation("Supplier {SupplierId} approved by admin.", supplierId);

             // Notify the Supplier about approval
             await notificationService.SendAndSaveNotificationAsync(
                 supplierId,
                 "Account Approved",
                 "Your Supplier account has been approved. You can now receive orders.",
                 NotificationType.SupplierApproved);

             var map = mapper.Map<UserProfileResponse>(user);
             return ResultResponse<UserProfileResponse>.Success(map);
         }


         public async Task<ResultResponse<UserProfileResponse>> RejectSupplierAsync(Guid supplierId)
         {
             var user = await userManager.Users
                 .Where(u => u.Id == supplierId && u.UserType == UserType.Supplier)
                 .FirstOrDefaultAsync();

             if (user is null)
                 return ResultResponse<UserProfileResponse>.Failure("Supplier not found.");

             if (user.ApprovalStatus != ApprovalStatus.Pending)
                 return ResultResponse<UserProfileResponse>.Failure("Supplier is not in Pending status.");

             user.ApprovalStatus = ApprovalStatus.Rejected;
             await userManager.UpdateAsync(user);

             logger.LogInformation("Supplier {SupplierId} rejected by admin.", supplierId);

             // Notify the Supplier about rejection
             await notificationService.SendAndSaveNotificationAsync(
                 supplierId,
                 "Account Rejected",
                 "Your Supplier account has been rejected. Please contact support for more information.",
                 NotificationType.SupplierRejected);

             var map = mapper.Map<UserProfileResponse>(user);
             return ResultResponse<UserProfileResponse>.Success(map);
         }*/

        #endregion

        #region Order Monitoring


        public async Task<ResultResponse<OrderResponse>> GetAllOrdersAsync()
        {
            var spec = new OrderSpecification();
            var orders = await unitOfWork.Repository<Order>().GetAllSpecTrackedAsync(spec);

            if (!orders.Any())
                return ResultResponse<OrderResponse>.Failure("No orders found.");

            var mapped = mapper.Map<List<OrderResponse>>(orders);
            return ResultResponse<OrderResponse>.Success(mapped);
        }

        #endregion
    }
}
