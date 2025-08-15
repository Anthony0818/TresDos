namespace TresDos.Application.DTOs.UserDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserDto UserDetail { get; set; }
    }
}
