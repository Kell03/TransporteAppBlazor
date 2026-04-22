namespace TransporteWeb.Contracts
{
    public record AuthResponse
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJVc3VhcmlvIFRlc3QiLCJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJyb2xlIjoiQWRtaW4iLCJleHAiOjE5OTk5OTk5OTksImlhdCI6MTYwOTQ1NjAwMH0.KxYxKxYxKxYxKxYxKxYxKxYxKxYxKxYxKxYxKxY";
    }
}
