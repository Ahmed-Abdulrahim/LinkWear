namespace Wasl.Application.Shared
{
    public static class StaticMethod
    {
        public static UserType MapRoleToUserType(string role)
        {
            return role switch
            {
                "Admin" => UserType.Admin,
                "StoreOwner" => UserType.StoreOwner,
                "Supplier" => UserType.Supplier,
                _ => throw new ArgumentException($"Invalid role: {role}")
            };
        }
    }
}
