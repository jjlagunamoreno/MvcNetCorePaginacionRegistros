using Microsoft.EntityFrameworkCore;

namespace MvcNetCorePaginacionRegistros.Data
{
    public class HospitalContext: DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options)
        {
        }
        public DbSet<Models.VistDepartamento> VistDepartamentos { get; set; }
        public DbSet<Models.Empleado> Empleados { get; set; }
        public DbSet<Models.Departamento> Departamentos { get; set; }
    }
}
