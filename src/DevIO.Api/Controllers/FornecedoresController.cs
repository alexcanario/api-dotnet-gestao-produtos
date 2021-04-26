using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers {
    public class FornecedoresController : MainController {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, IMapper mapper) {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }

        [Route("api/[controller]")]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos() {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());            

            return Ok(fornecedores);
        }
    }
}