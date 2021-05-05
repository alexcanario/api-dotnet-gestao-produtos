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
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(  IFornecedorRepository fornecedorRepository,
                                        IFornecedorService fornecedorService,
                                        IEnderecoRepository enderecoRepository,
                                        INotificador notificador,
                                        IMapper mapper) : base(notificador) {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
            _enderecoRepository = enderecoRepository;
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

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id) {
            var endereco = await _enderecoRepository.ObterPorId(id);

            return _mapper.Map<EnderecoViewModel>(endereco);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel) {
            if(!ModelState.IsValid) return CustomResponse(ModelState);
            
            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));
            
            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorView) {
            if(!id.Equals(fornecedorView.Id)) {
                NotifyError("O id informado difere do id informado na query");
                return BadRequest(fornecedorView);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorView));

            return CustomResponse(fornecedorView);
        }

        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> AtualizarEndereco(Guid id, EnderecoViewModel endereco) {
            if (!id.Equals(endereco.Id)) return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(endereco));

            return CustomResponse(endereco);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id) {
            var fornecedor = await ObterFornecedorEndereco(id);

            if(fornecedor is null) return NotFound();

            await _fornecedorService.Remover(id);

            return Ok(_mapper.Map<FornecedorViewModel>(fornecedor));
        }

        public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id) {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id) {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        public async Task<IEnumerable<FornecedorViewModel>> ObterTodosFornecedoresComEndereco() {
            var todos = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodosComEndereco());

            return todos;
        }
     }
}