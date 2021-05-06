using System;
using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevIO.Business.Models;
using Microsoft.Data.Sql;

namespace DevIO.Api.Controllers {
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos() {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id) {
            var produto = await ObterProduto(id);

            if (produto is null) return NotFound();

            return CustomResponse(produto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id) {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel is null) return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produtoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel) {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (!UploadArquivo(produtoViewModel.ImagemUpload, produtoViewModel.Imagem)) {
                return CustomResponse(produtoViewModel);
            }

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        private bool UploadArquivo(string arquivo, string imgNome) {
            
            if (string.IsNullOrEmpty(arquivo)) {
                //ModelState.AddModelError(string.Empty, "Forneça uma imagem para este produto!");
                // Ou assim
                NotifyError("Forneça uma imagem para este produto!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);
            if (System.IO.File.Exists(filePath)) {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                //Ou
                NotifyError("Já existe um arquivo com este nome!");
                return false;
            }

            var imageByteDataArray = Convert.FromBase64String(arquivo);
            System.IO.File.WriteAllBytes(filePath, imageByteDataArray);
            return true;
        }



        public async Task<ProdutoViewModel> ObterProduto(Guid id) {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }
    }
}
