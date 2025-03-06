using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
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

//create procedure SP_GRUPO_DEPARTAMENTOS
//(@posicion int)
//as
//	select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL
//	where POSICION >= @posicion AND POSICION < (@posicion + 2)
//go
//exec SP_GRUPO_DEPARTAMENTOS 2

//create view V_GRUPO_EMPLEADOS
//as
//	select CAST(ROW_NUMBER() over (order by APELLIDO) AS INT) as POSICION
//	, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//go

//create procedure SP_GRUPO_EMPLEADOS
//(@posicion int)
//as
//	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM V_GRUPO_EMPLEADOS
//	where POSICION >= @posicion AND POSICION < (@posicion + 3)
//go

//create procedure SP_GRUPO_EMPLEADOS_OFICIO
//(@posicion int, @oficio nvarchar(50))
//as
//select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from 
//	(select
//	ROW_NUMBER() over (order by APELLIDO) as POSICION,
//    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//	where OFICIO = @oficio) query
//	where POSICION >= @posicion AND POSICION < (@posicion + 2)
//go

//exec SP_GRUPO_EMPLEADOS_OFICIO 1, 'ANALISTA'

//create procedure SP_GRUPO_EMPLEADOS_OFICIO_OUT
//(@posicion int, @oficio nvarchar(50)
//, @registros int out)
//as
//--ALMACENAMOS EL NÚMERO DE REGISTROOS DEL FILTRO
//	select registros = count(EMP_NO) from EMP
//	where OFICIO=@oficio
//	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from 
//		(select
//		ROW_NUMBER() over (order by APELLIDO) as POSICION,
//        EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//		where OFICIO = @oficio) query
//		where POSICION >= @posicion AND POSICION < (@posicion + 2)
//go

//exec SP_GRUPO_EMPLEADOS_OFICIO_OUT 1, 'ANALISTA', 2

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

        public async Task<Departamento> FindDepartamento(int idDepartamento)
        {
            var consulta = from datos in this.context.Departamentos
                           where datos.IdDepartamento == idDepartamento
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<Empleado> GetEmpleadoDepartamento(int posicion, int idDepartamento)
        {
            if (posicion < 1)
            {
                posicion = 1; // Asegurar que siempre inicie desde 1
            }

            string sql = "EXEC SP_EMPLEADOS_DEPARTAMENTO @posicion, @idDepartamento";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamIdDepartamento = new SqlParameter("@iddepartamento", idDepartamento);

            List<Empleado> consulta =
                 await this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamIdDepartamento).ToListAsync();

            return consulta.FirstOrDefault(); // Devuelve null si no hay empleados
        }


        public async Task<int> GetNumeroRegistros(int idDepartamento)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdDepartamento == idDepartamento
                           select datos;

            return await consulta.CountAsync();
        }

        public async Task<ModelEmpleadosOficio>
            GetEmpleadosOficioOutAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO_OUT @posicion, @ofico, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados
                .FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros);
            //PRIMERO DEBEMOS EJECUTAR LA CONSULTA PARA PODER
            //RECUPERAR LOS PARAMETROS DE SALIDA
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = int.Parse(pamRegistros.Value.ToString());
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }

        public async Task<int>
            GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados
                .Where(x => x.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>>
            GetEmpleadosOficioAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            var consulta = this.context.Empleados
                .FromSqlRaw(sql, pamPosicion, pamOficio);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync
            (int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta =
                 this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistDepartamentos.CountAsync();
        }

        public async Task<List<Departamento>>
            GetGrupoDepartamenotsAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion =
                new SqlParameter("@posicion", posicion);
            var consulta = 
                this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
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
