using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcNetCorePaginacionRegistros.Models
{
    [Table("V_DEPARTAMENTOS_INDIVIDUAL")]
    public class VistDepartamento
    {
        [Key]
        [Column("DEPT_NO")]
        public int IdDepartamento { get; set; }
        [Column("DNOMBRE")]
        public string Nombre { get; set; }
        [Column("LOC")]
        public string Localidad { get; set; }
        [Column("POSICION")]
        public int Posicion { get; set; }
    }
}
