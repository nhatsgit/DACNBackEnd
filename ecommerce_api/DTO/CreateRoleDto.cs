namespace ecommerce_api.DTO
{
    public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public List<UserClaimDto> Claims { get; set; } = new List<UserClaimDto>();
}
}
