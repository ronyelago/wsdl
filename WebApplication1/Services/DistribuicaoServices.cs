using System.Linq;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public class DistribuicaoServices
    {
        private readonly webservicetwos3Entities _context;

        public DistribuicaoServices()
        {
            this._context = new webservicetwos3Entities();
        }

        public DistribuicaoCrachaViewModel CrachaHandler(string epc)
        {
            DistribuicaoCrachaViewModel distribuicaoCrachaViewModel;

            var cracha = _context.L_ATRIBUICAOCRACHA.FirstOrDefault(x => x.CODIGO_CRACHA == epc);

            if (cracha != null)
            {
                var funcionario = _context.L_FUNCIONARIOS.FirstOrDefault(f => f.ID == cracha.FK_FUNCIONARIO);

                distribuicaoCrachaViewModel = AssemblyEpcIsCracha(funcionario, cracha);

                return distribuicaoCrachaViewModel;
            }

            return null;
        }

        public DistribuicaoEpiViewModel EpiHandler(string epc)
        {
            DistribuicaoEpiViewModel distribuicaoViewModel = new DistribuicaoEpiViewModel();
            distribuicaoViewModel.Epc = epc;

            // Verifica pelo epc se o item existe na base de dados (tabela L_PRODUTOS_ITENS)
            var item = _context.L_PRODUTOS_ITENS.FirstOrDefault(x => x.EPC == epc);

            if (item != null)
            {
                var epi = _context.L_ESTOQUE.FirstOrDefault(x => x.EPC == epc);
            }

            // Caso o epc não seja encontrado na base de dados
            else
            {
                distribuicaoViewModel = AssemblyEpcNotFound();
            }

            return distribuicaoViewModel;
        }

        public DistribuicaoCrachaViewModel AssemblyEpcIsCracha(L_FUNCIONARIOS funcionario, L_ATRIBUICAOCRACHA cracha)
        {
            DistribuicaoCrachaViewModel viewModel = new DistribuicaoCrachaViewModel();
            viewModel.NomeFuncionario = $"{funcionario.NOME} {funcionario.SOBRENOME}";
            viewModel.Epc = cracha.CODIGO_CRACHA;
            viewModel.Matricula = funcionario.MATRICULA;

            return viewModel;
        }

        //public DistribuicaoEpiViewModel AssemblyEpcIsEpi(L_PRODUTOS_ITENS item, L_ESTOQUE epi)
        //{
        //    DistribuicaoEpiViewModel distribuicaoViewModel = new DistribuicaoEpiViewModel();
        //    distribuicaoViewModel.Titulo = "EPI";
        //    distribuicaoViewModel.Epc = epi.EPC;
        //    distribuicaoViewModel.Situacao = item.ATIVO;
        //}

        public DistribuicaoEpiViewModel AssemblyEpcNotFound()
        {
            DistribuicaoEpiViewModel distribuicaoViewModel = new DistribuicaoEpiViewModel();

            distribuicaoViewModel.Titulo = "Item não encontrado";
            distribuicaoViewModel.Situacao = "Item não cadastrado na base de dados";

            return distribuicaoViewModel;
        }
    }
}