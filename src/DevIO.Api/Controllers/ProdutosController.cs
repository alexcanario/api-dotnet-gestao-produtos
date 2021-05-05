using System;
using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;
using DevIO.Business.Models;

namespace DevIO.Api.Controllers {
    public class ProdutosController : MainController {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            INotificador notificador,
            IMapper mapper) : base(notificador) {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet("api/produtos")]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos() {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id) {
            var produto = await _produtoRepository.ObterPorId(id);

            if (produto is null) return NotFound();

            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel) {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtoService.Adicionar(produto);

            return CustomResponse(produtoViewModel);
        }

    }

}
}