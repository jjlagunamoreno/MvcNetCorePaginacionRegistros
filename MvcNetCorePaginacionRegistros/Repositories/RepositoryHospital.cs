using Microsoft.EntityFrameworkCore;
using MvcNetCorePaginacionRegistros.Data;
using MvcNetCorePaginacionRegistros.Models;

#region VISTAS Y PROCEDIMIENTOS

//CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
//AS
//SELECT 
//    ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS POSICION,
//    DEPT_NO, 
//    DNOMBRE, 
//    LOC 
//FROM DEPT;
//GO
//SELECT * FROM V_DEPARTAMENTOS_INDIVIDUAL WHERE POSICION = 1;

#endregion

namespace MvcNetCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;
        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistDepartamentos.CountAsync();
        }

        public async Task<List<VistDepartamento>>
            GetGrupoVistaDepartamentosAsync(int posicion)
        {
            //select* from V_DEPARTAMENTOS_INDIVIDUAL
            //where POSICION >= 1 and POSICIPN< (1 + 2)
            var consulta = from datas in this.context.VistDepartamentos
                           where datas.Posicion >= posicion
                            && datas.Posicion < (posicion + 2)
                           select datas;
            return await consulta.ToListAsync();
        }

        public async Task<VistDepartamento>
            GetVistaDepartamentoAsync(int posicion)
        {
            VistDepartamento departamento = await
                this.context.VistDepartamentos
                .Where(z => z.Posicion == posicion).FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var departamentos =
                await this.context.Departamentos.ToListAsync();
            return departamentos;
        }

        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync
            (int idDepartamento)
        {
            var empleados = this.context.Empleados
                .Where(x => x.IdDepartamento == idDepartamento);

            if (empleados.Count() == 0)
            {
                return null;
            }
            else
            {
                return await empleados.ToListAsync();
            }
        }
    }
}
