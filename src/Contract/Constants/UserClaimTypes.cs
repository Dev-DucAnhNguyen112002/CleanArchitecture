namespace CleanArchitectureTest.Contract.Constants;

public static class UserClaimTypes
{
    // Các trường này có trong table 'users' (cột 'id' và 'username')
    public const string UserId = "user:id";
    public const string Username = "user:username";

    // Thêm các trường này vì chúng có trong table 
    // và hữu ích để đưa vào token
    public const string EmailConfirmed = "user:email_confirmed";
    public const string PhoneNumberConfirmed = "user:phone_number_confirmed";
    public const string FullName = "user:full_name";

    // --- ĐÃ LOẠI BỎ ---
    // Các hằng số sau đã bị xóa vì 
    // table 'users' của bạn không có các cột này:
    // public const string FullName = "user:full_name";
    // public const string AvatarUrl = "user:avatar_url";
    // public const string StorageQuota = "user:storage_quota";
    // public const string StorageUsed = "user:storage_used";
}
