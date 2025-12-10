namespace Application.Features.Auth.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }

        // Frontend State'i için gerekli bilgiler
        public UserDto CurrentContext { get; set; }

        public List<UserProfileDto> AvailableProfiles { get; set; } = new();
    }
}
