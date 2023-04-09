namespace AuthenticationWithIdentity.DataTransferObjects
{
    public record AuthenticationResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
