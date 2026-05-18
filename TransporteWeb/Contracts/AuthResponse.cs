namespace TransporteWeb.Contracts
{
    public record AuthResponse
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? EmpresaId { get; set; }
        public string? RolId { get; set; }
        public string Token { get; set; } 
    }
}
