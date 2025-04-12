namespace ecommerce_api.Helper
{
    public static class ClaimTypesConstants
    {
        public const string ProductRead = "product_read";
        public const string ProductCreate = "product_create";
        public const string ProductUpdate = "product_update";
        public const string ProductDelete = "product_delete";

        public static readonly Dictionary<string, string> AllPolicies = new()
        {
            { ProductRead, "Xem sản phẩm" },
            { ProductCreate, "Tạo sản phẩm" },
            { ProductUpdate, "Cập nhật sản phẩm" },
            { ProductDelete, "Xoá sản phẩm" }
        };
    }
}
