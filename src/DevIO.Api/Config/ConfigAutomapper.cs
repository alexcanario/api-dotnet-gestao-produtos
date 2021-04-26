using AutoMapper;
using DevIO.Business.Intefaces;
using DevIO.Data.Repository;

namespace DevIO.Api.Config {
    public class ConfigAutomapper : Profile{
        public ConfigAutomapper() {
            CreateMap<IFornecedorRepository, FornecedorRepository>();
            CreateMap<IEnderecoRepository, EnderecoRepository>();
            CreateMap<IProdutoRepository, ProdutoRepository>();
        }
    }
}