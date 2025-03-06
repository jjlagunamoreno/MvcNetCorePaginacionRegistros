using Microsoft.AspNetCore.Mvc;
using MvcNetCorePaginacionRegistros.Models;
using MvcNetCorePaginacionRegistros.Repositories;

namespace MvcNetCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult>
    PaginarGrupoVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros =
                await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            ViewData["REGISTROS"] = numRegistros;
            List<VistDepartamento> departamentos =
                await this.repo.GetGrupoVistaDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult>
            PaginarRegistroVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await
                this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            //PRIMERO = 1
            //ULTIMO = 4
            //SIGUIENTE = 5
            //ANTERIOR = 4
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            VistDepartamento departamento =
                await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);
        }
    }
}
