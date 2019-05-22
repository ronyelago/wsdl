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
        private readonly DocumentService _documentService;

        public DistribuiEpiService()
        {
            _context = new webservicetwos3Entities();
            _documentService = new DocumentService();
        }

        public DistribuicaoSuccessViewModel DistribuiEpi(int crachaId, string epiIdListString)
        {
            DistribuicaoSuccessViewModel viewModel = new DistribuicaoSuccessViewModel();

            var cracha = _context.L_ATRIBUICAOCRACHA.FirstOrDefault(c => c.ID == crachaId);
            var funcionario = _context.L_FUNCIONARIOS.FirstOrDefault(f => f.ID == cracha.FK_FUNCIONARIO);

            var listaEpiIds = epiIdListString.Split('|').ToList();

            List<L_PRODUTOS_ITENS> listaEpis = new List<L_PRODUTOS_ITENS>();

            // preenche lista de epi's
            foreach (string epiId in listaEpiIds)
            {
                int id = int.Parse(epiId);

                var epi = _context.L_PRODUTOS_ITENS.FirstOrDefault(x => x.ID == id);

                if (epi != null)
                {
                    listaEpis.Add(epi);
                }
            }

            // guid que será usado para as movimentações
            Guid codigoDistribuicao = Guid.NewGuid();

            // geração do nome do documento (ficha de entrega de epi's)
            string nomeArquivo = $"{funcionario.ID}_{DateTime.Now.Hour}{DateTime.Now.Minute}" +
                $"{DateTime.Now.Millisecond}{DateTime.Now.Year}";

            // geração dos registros de cada distribuição
            foreach (var epi in listaEpis)
            {
                var produto = _context.L_PRODUTOS.FirstOrDefault(x => x.COD_PRODUTO == epi.COD_PRODUTO);
                var epiEstoque = _context.L_ESTOQUE.FirstOrDefault(x => x.FK_PRODUTO == epi.ID);
                
                // geração do registro de movimentação
                var movimentacaoEstoque = assemblyMovimentacaoEstoque(epi, funcionario, produto, codigoDistribuicao);
                _context.L_MOVIMENTACAO_ESTOQUE.Add(movimentacaoEstoque);
                _context.SaveChanges();

                // geração do registro de ficha cadastral
                _context.L_FICHACADASTRAL.Add(assemblyFichaCadastral(cracha));
                _context.SaveChanges();

                if (epiEstoque == null)
                {
                    var registroEstoque = assemblyRegistroEstoque(epi, funcionario);

                    _context.L_ESTOQUE.Add(registroEstoque);                    

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

            var movimentacao = _context.L_MOVIMENTACAO_ESTOQUE.FirstOrDefault(x => x.COD_DISTRIBUICAO == codigoDistribuicao);

            // geração do registro (dados da ficha de entrega epi')
            _context.L_DOCUMENTO_ASSINATURA.Add(assemblyDocumento(movimentacao.COD_DISTRIBUICAO.ToString(),
                nomeArquivo, funcionario.ID, funcionario.FK_CLIENTE.ToString()));
            _context.SaveChanges();

            // geração da ficha de entrega de epi's
            _documentService.convertHtmlDocx(funcionario.ID, movimentacao.COD_DISTRIBUICAO.ToString(), nomeArquivo);

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
            novoEpiEstoque.COD_RECEBIMENTO = Guid.NewGuid();
            //novoEpiEstoque.COD_ESTOQUE = 
            //novoEpiEstoque.ESTOQUE = 
            novoEpiEstoque.ENTRADA_SAIDA = "S";
            novoEpiEstoque.COD_PRODUTO = epi.COD_PRODUTO;
            novoEpiEstoque.MATRICULA = funcionario.MATRICULA;
            novoEpiEstoque.STATUS = "D";
            novoEpiEstoque.DESC_STATUS = "Distribuição de EPI";

            return novoEpiEstoque;
        }

        private L_MOVIMENTACAO_ESTOQUE assemblyMovimentacaoEstoque(L_PRODUTOS_ITENS epi, L_FUNCIONARIOS funcionario, L_PRODUTOS produto, Guid codigoDistribuicao)
        {
            var movimentacaoEstoque = new L_MOVIMENTACAO_ESTOQUE();
            movimentacaoEstoque.DATA_MOVIMENTACAO = DateTime.Now;
            movimentacaoEstoque.ENTRADA_SAIDA = "S";
            movimentacaoEstoque.FK_PRODUTO = produto.ID;
            movimentacaoEstoque.DESC_PRODUTO = epi.PRODUTO;
            movimentacaoEstoque.FK_FUNCIONARIO = funcionario.ID;
            //movimentacaoEstoque.TRANSFERENCIA = 
            //movimentacaoEstoque.COD_ESTOQUE = 
            movimentacaoEstoque.QUANTIDADE = 1;
            movimentacaoEstoque.COD_PRODUTO = epi.COD_PRODUTO;
            //movimentacaoEstoque.COD_FUNCIONARIO =
            movimentacaoEstoque.COD_DISTRIBUICAO = codigoDistribuicao;
            movimentacaoEstoque.EPC = epi.EPC;
            movimentacaoEstoque.STATUS = "D";
            movimentacaoEstoque.DESC_STATUS = "Distribuição de EPI";

            return movimentacaoEstoque;
        }

        private L_FICHACADASTRAL assemblyFichaCadastral(L_ATRIBUICAOCRACHA cracha)
        {
            var fichaCadastral = new L_FICHACADASTRAL();
            fichaCadastral.DATA = DateTime.Now;
            fichaCadastral.FK_FUNCIONARIO = cracha.FK_FUNCIONARIO;
            fichaCadastral.IMPRESSO = "N";

            return fichaCadastral;
        }

        private L_DOCUMENTO_ASSINATURA assemblyDocumento(string codDistribuicao, string nomeArquivo, int? funcionarioId, string clienteId)
        {
            var dbo = new webservicetwos3Entities();
            var documento = new L_DOCUMENTO_ASSINATURA();

            var funcionario = dbo.L_FUNCIONARIOS.FirstOrDefault(x => x.ID == funcionarioId);

            if (funcionario != null)
            {
                documento.CLIENTE = clienteId;
                documento.DATA_ENVIO = DateTime.Now;
                documento.DOWNLOAD = "N";
                documento.MATRICULA = funcionario.MATRICULA;
                documento.NOME_DOCUMENTO = nomeArquivo;
                documento.CHAVE = string.Empty;
                documento.COD_DISTRIBUICAO = Guid.Parse(codDistribuicao);
            }

            return documento;
        }
    }
}