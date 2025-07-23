namespace TresDos.Application.Common.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid credentials.")
        {
        }
    }
}
