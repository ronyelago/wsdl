using EasyEPI.Infra.Entities;
using EasyEPI.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace easyepi.api.Controllers
{
    [Route("api/[controller]")]
    public class FuncionarioController : Controller
    {
        private readonly IFuncionarioRepository _funcionarioRepository;

        public FuncionarioController(IFuncionarioRepository funcionarioRepository)
        {
            _funcionarioRepository = funcionarioRepository;
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<L_FUNCIONARIOS> Get(int id)
        {
            return _funcionarioRepository.Get(id);
        }

        [HttpGet]
        public IEnumerable<L_FUNCIONARIOS> Get()
        {
            return _funcionarioRepository.GetAll();
        }
    }
}
