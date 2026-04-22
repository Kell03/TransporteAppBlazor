namespace Domain.Dto
{
    public class RolPermiso
    {
        public int Id { get; set; }

        public int Rol_Id { get; set; }
        public string Permiso { get; set; }

        public DateTime Created_at { get; set; }

        public virtual Rol? Rol { get; set; }

    }
}
