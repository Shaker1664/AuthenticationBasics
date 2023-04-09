namespace AuthenticationWithIdentity.Exceptions
{
    public class RefreshTokenBadRequest : BadRequestException
    {
        public RefreshTokenBadRequest() : base("Invalid request: The request has invalid input")
        {
        }
    }
}
