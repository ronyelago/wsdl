using System;
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

        public DistribuicaoViewModel CrachaHandler(string epc)
        {
            DistribuicaoViewModel distribuicaoCrachaViewModel = new DistribuicaoViewModel
            {
                Epc = epc
            };

            var cracha = _context.L_ATRIBUICAOCRACHA.FirstOrDefault(x => x.CODIGO_CRACHA == epc);

            if (cracha != null)
            {
                var funcionario = _context.L_FUNCIONARIOS.FirstOrDefault(f => f.ID == cracha.FK_FUNCIONARIO);

                distribuicaoCrachaViewModel = AssemblyEpcIsCracha(funcionario, cracha);

                return distribuicaoCrachaViewModel;
            }

            return null;
        }

        public DistribuicaoViewModel EpiHandler(string epc)
        {
            DistribuicaoViewModel distribuicaoViewModel = new DistribuicaoViewModel();
            distribuicaoViewModel.Epc = epc;

            // Verifica pelo epc se o item existe na base de dados (tabela L_PRODUTOS_ITENS)
            var item = _context.L_PRODUTOS_ITENS.FirstOrDefault(x => x.EPC == epc);

            if (item != null)
            {
                var epi = _context.L_ESTOQUE.FirstOrDefault(x => x.EPC == epc);

                if (epi != null)
                {
                    return AssemblyEpcIsEpi(item, epi);
                }

                else
                {
                    return AssemblyEpcIsEpi(item, null);
                }
            }

            return distribuicaoViewModel = AssemblyEpcNotFound(epc);
        }

        public DistribuicaoViewModel AssemblyEpcIsCracha(L_FUNCIONARIOS funcionario, L_ATRIBUICAOCRACHA cracha)
        {
            DistribuicaoViewModel viewModel = new DistribuicaoViewModel();
            viewModel.Titulo = $"{funcionario.NOME} {funcionario.SOBRENOME}";
            viewModel.Epc = cracha.CODIGO_CRACHA;
            viewModel.Observacoes = $"Matrícula: {funcionario.MATRICULA}";
            viewModel.Disponivel = true;
            viewModel.Tipo = 2; // Tipo crachá

            return viewModel;
        }

        public DistribuicaoViewModel AssemblyEpcIsEpi(L_PRODUTOS_ITENS item, L_ESTOQUE epi)
        {
            DistribuicaoViewModel distribuicaoViewModel = new DistribuicaoViewModel();
            distribuicaoViewModel.Titulo = item.PRODUTO;
            distribuicaoViewModel.Epc = item.EPC;
            distribuicaoViewModel.Icone = "epiicon";
            distribuicaoViewModel.Disponivel = false;
            distribuicaoViewModel.Tipo = 1; // tipo Epi

            if (item.DT_VALIDADE < DateTime.Now)
            {
                distribuicaoViewModel.Observacoes = "Data de Validade vencida";

                return distribuicaoViewModel;
            }

            else if (item.VALIDADE_TESTE < DateTime.Now)
            {
                distribuicaoViewModel.Observacoes = "Data de Validade de Teste vencida";

                return distribuicaoViewModel;
            }

            if (epi != null)
            {
                // Recebido (somente na tabela L_PRODUTOS_ITENS)
                // ou Recebimento de Itens Testado
                if (epi.STATUS == "R" || epi.STATUS == "A")
                {
                    distribuicaoViewModel.Disponivel = true;
                }

                else if (epi.STATUS == "D") // Distribuição de EPI
                {
                    distribuicaoViewModel.Observacoes = "EPI já atribuído a um funcionário";

                    return distribuicaoViewModel;
                }

                else if (epi.STATUS == "B") // Devolução de EPI ao Estoque
                {
                    distribuicaoViewModel.Observacoes = "EPI encontra-se para ser devolvido ao estoque";
                }

                else if (epi.STATUS == "O") // Distribuído e aguardando assinatura
                {
                    distribuicaoViewModel.Observacoes = "EPI aguardando assinatura";
                }

                else if (epi.STATUS == "M") // Em Manutenção Cliente ou Fornecedor
                {
                    distribuicaoViewModel.Observacoes = "EPI em manutenção";
                }
            }

            else
            {
                distribuicaoViewModel.Observacoes = "EPI disponível para distribuição";
                distribuicaoViewModel.Disponivel = true;
            }
            
            return distribuicaoViewModel;
        }

        public DistribuicaoViewModel AssemblyEpcNotFound(string epc)
        {
            DistribuicaoViewModel distribuicaoViewModel = new DistribuicaoViewModel();

            distribuicaoViewModel.Titulo = "Crachá/EPI não encontrado";
            distribuicaoViewModel.Epc = epc;
            distribuicaoViewModel.Icone = "epcnotfoundicon";
            distribuicaoViewModel.Observacoes = "Item não cadastrado na base de dados";
            distribuicaoViewModel.Disponivel = false;
            distribuicaoViewModel.Tipo = 0; // não encontrado

            return distribuicaoViewModel;
        }
    }
}