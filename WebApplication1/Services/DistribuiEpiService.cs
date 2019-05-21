using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public class DistribuiEpiService
    {
        private readonly webservicetwos3Entities _context;

        public DistribuiEpiService()
        {
            _context = new webservicetwos3Entities();
        }

        public DistribuicaoSuccessViewModel DistribuiEpi(int crachaId, string epiIdListString)
        {
            DistribuicaoSuccessViewModel viewModel = new DistribuicaoSuccessViewModel();

            var cracha = _context.L_ATRIBUICAOCRACHA.FirstOrDefault(c => c.ID == crachaId);
            var funcionario = _context.L_FUNCIONARIOS.FirstOrDefault(f => f.ID == cracha.FK_FUNCIONARIO);

            var listaEpiIds = epiIdListString.Split('|').ToList();

            List<L_PRODUTOS_ITENS> listaEpis = new List<L_PRODUTOS_ITENS>();

            foreach (string epiId in listaEpiIds)
            {
                int id = int.Parse(epiId);

                var epi = _context.L_PRODUTOS_ITENS.FirstOrDefault(x => x.ID == id);

                if (epi != null)
                {
                    listaEpis.Add(epi);
                }
            }

            foreach (var epi in listaEpis)
            {
                var epiEstoque = _context.L_ESTOQUE.FirstOrDefault(x => x.FK_PRODUTO == epi.ID);

                _context.L_MOVIMENTACAO_ESTOQUE.Add(assemblyMovimentacaoEstoque(epi, funcionario));
                _context.SaveChanges();

                if (epiEstoque == null)
                {
                    _context.L_ESTOQUE.Add(assemblyRegistroEstoque(epi, funcionario));

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        viewModel.Success = false;
                    }
                }

                else
                {
                    epiEstoque.DATA_SAIDA = DateTime.Now.ToString();
                    epiEstoque.FK_FUNCIONARIO_SAIDA = funcionario.ID;
                    epiEstoque.NOME_FUNC_SAIDA = $"{funcionario.NOME} {funcionario.SOBRENOME}";
                    epiEstoque.ENTRADA_SAIDA = "S";
                    epiEstoque.MATRICULA = funcionario.MATRICULA;
                    epiEstoque.STATUS = "D";
                    epiEstoque.STATUS = "Distribuição de EPI";

                    _context.Entry(epiEstoque).State = EntityState.Modified;

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        viewModel.Success = false;
                    }
                }
            }

            viewModel.Success = true;

            return viewModel;
        }

        private L_ESTOQUE assemblyRegistroEstoque(L_PRODUTOS_ITENS epi, L_FUNCIONARIOS funcionario)
        {
            var novoEpiEstoque = new L_ESTOQUE();
            novoEpiEstoque.FK_PRODUTO = epi.ID;
            novoEpiEstoque.DESC_PRODUTO = epi.PRODUTO;
            novoEpiEstoque.QUANTIDADE = 1;
            novoEpiEstoque.DATA_ENTRADA = DateTime.Now;
            novoEpiEstoque.EPC = epi.EPC;
            novoEpiEstoque.DATA_SAIDA = DateTime.Now.ToString();
            novoEpiEstoque.FK_FUNCIONARIO_SAIDA = funcionario.ID;
            //novoEpiEstoque.NOME_FUNC_SAIDA = 
            //novoEpiEstoque.ULT_INSPECAO = 
            novoEpiEstoque.COD_RECEBIMENTO = new Guid();
            //novoEpiEstoque.COD_ESTOQUE = 
            //novoEpiEstoque.ESTOQUE = 
            novoEpiEstoque.ENTRADA_SAIDA = "S";
            novoEpiEstoque.COD_PRODUTO = epi.COD_PRODUTO;
            novoEpiEstoque.MATRICULA = funcionario.MATRICULA;
            novoEpiEstoque.STATUS = "D";
            novoEpiEstoque.DESC_STATUS = "Distribuição de EPI";

            return novoEpiEstoque;
        }

        private L_MOVIMENTACAO_ESTOQUE assemblyMovimentacaoEstoque(L_PRODUTOS_ITENS epi, L_FUNCIONARIOS funcionario)
        {
            var movimentacaoEstoque = new L_MOVIMENTACAO_ESTOQUE();
            movimentacaoEstoque.DATA_MOVIMENTACAO = DateTime.Now;
            movimentacaoEstoque.ENTRADA_SAIDA = "S";
            movimentacaoEstoque.FK_PRODUTO = epi.ID;
            movimentacaoEstoque.DESC_PRODUTO = epi.PRODUTO;
            movimentacaoEstoque.FK_FUNCIONARIO = funcionario.ID;
            //movimentacaoEstoque.TRANSFERENCIA = 
            //movimentacaoEstoque.COD_ESTOQUE = 
            movimentacaoEstoque.QUANTIDADE = 1;
            movimentacaoEstoque.COD_PRODUTO = epi.COD_PRODUTO;
            //movimentacaoEstoque.COD_FUNCIONARIO =
            movimentacaoEstoque.COD_DISTRIBUICAO = new Guid();
            movimentacaoEstoque.EPC = epi.EPC;
            movimentacaoEstoque.STATUS = "D";
            movimentacaoEstoque.DESC_STATUS = "Distribuição de EPI";

            return movimentacaoEstoque;
        }

        private L_FICHACADASTRAL assemblyFichaCadastral()
        {

        }
    }
}