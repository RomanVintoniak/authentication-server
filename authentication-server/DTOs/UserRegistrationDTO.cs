namespace authentication_server.DTOs
{
    public class UserRegistrationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public UserRegistrationDTO()
        {
            FirstName ??= "";
            LastName ??= "";
            Email ??= "";
            Password ??= "";
            PasswordConfirm ??= "";
        }
    }
}
