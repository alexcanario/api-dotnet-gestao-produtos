using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers {
    [Route("api/[controller]")]
    public class FornecedoresController : MainController {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(  IFornecedorRepository fornecedorRepository,
                                        IFornecedorService fornecedorService,
                                        IMapper mapper) {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos() {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());            

            return Ok(fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id) {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);
            if(fornecedor is null) return NotFound();

            return Ok(fornecedor);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel) {
            if(!ModelState.IsValid) return BadRequest();
            
            var result = await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            if(!result) return BadRequest();

            return Ok(fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorView) {
            if(!id.Equals(fornecedorView.Id)) return BadRequest();

            if(!ModelState.IsValid) return BadRequest();

            var result = await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorView));

            if(!result) return BadRequest();

            return Ok(fornecedorView);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id) {
            var fornecedor = await ObterFornecedorEndereco(id);

            if(fornecedor is null) return NotFound();

            var result = await _fornecedorService.Remover(id);
            if(!result) return BadRequest();

            return Ok(fornecedor);
        }

        public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id) {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id) {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
     }
}