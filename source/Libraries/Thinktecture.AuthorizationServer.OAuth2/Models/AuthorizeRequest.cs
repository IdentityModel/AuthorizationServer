namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class AuthorizeRequest
    {
        public string client_id { get; set; }
        public string scope { get; set; }
        public string response_type { get; set; }
        public string redirect_uri { get; set; }
        public string state { get; set; }
        public string nonce { get; set; }
    }
}
