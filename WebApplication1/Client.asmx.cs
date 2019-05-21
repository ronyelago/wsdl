using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Services;
using WebApplication1.Services;
using WebApplication1.ViewModels;
using Xceed.Words.NET;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for Client
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Client : WebService
    {
        webservicetwos3Entities dbo = new webservicetwos3Entities();
        ControleSessao controleSessao = new ControleSessao();
        HttpClient client;
        private readonly DistribuicaoServices distribuicaoServices = new DistribuicaoServices();
        DistribuiEpiService distribuiEpiService = new DistribuiEpiService();

        [WebMethod]
        public List<L_LOCALESTOQUE> retornaLocalEstoque()
        {
            try
            {
                return dbo.L_LOCALESTOQUE.ToList();
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> movimentacaoEstoque(string listaEPCS, string estoque, string entSaida)
        {
            List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
            RESULTADOMOV mv = new RESULTADOMOV();
            string[] lines = listaEPCS.Split('|');
            L_ESTOQUE le = new L_ESTOQUE();
            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();

            string mensagem = "";
            foreach (var epc in lines)
            {
                mensagem = "";
                if (epc != "")
                {
                    var result = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                    if (result.Count == 0)
                    {
                        mv.Resultado = "Epi Não Existe no Estoque";
                        mv.EPC = epc;
                        mv.DataMovimentacao = DateTime.Now;
                        mv.Produto = "";
                        mv.CorAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                    }
                    else
                    {
                        if (result[0].STATUS != "T" && result[0].STATUS != "M" && result[0].STATUS != "Z")
                        {
                            if (result[0].ENTRADA_SAIDA != entSaida)
                            {
                                switch (entSaida)
                                {
                                    case "E":
                                        var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == estoque).ToList();
                                        int ID = result[0].ID;
                                        L_ESTOQUE est = dbo.L_ESTOQUE.First(x => x.ID == ID);
                                        est.ENTRADA_SAIDA = "E";
                                        est.COD_ESTOQUE = localEstoque[0].CODIGO;
                                        est.ESTOQUE = localEstoque[0].NOME;
                                        est.STATUS = "E";
                                        est.DESC_STATUS = "Entrada Estoque";
                                        dbo.SaveChanges();

                                        lme.COD_ESTOQUE = est.COD_ESTOQUE;
                                        lme.COD_PRODUTO = est.COD_PRODUTO;
                                        lme.DATA_MOVIMENTACAO = DateTime.Now;
                                        lme.DESC_PRODUTO = est.DESC_PRODUTO;
                                        lme.ENTRADA_SAIDA = "E";
                                        lme.EPC = est.EPC;
                                        lme.ESTOQUE = est.ESTOQUE;
                                        lme.FK_PRODUTO = est.FK_PRODUTO;
                                        lme.QUANTIDADE = est.QUANTIDADE;
                                        lme.STATUS = "E";
                                        lme.DESC_STATUS = "Entrada Estoque";
                                        dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                        dbo.SaveChanges();
                                        mensagem = "OK";

                                        break;
                                    case "S":
                                        int IDS = result[0].ID;
                                        L_ESTOQUE estSaida = dbo.L_ESTOQUE.First(x => x.ID == IDS);
                                        estSaida.ENTRADA_SAIDA = "S";
                                        estSaida.STATUS = "S";
                                        estSaida.DESC_STATUS = "Saida Estoque";
                                        dbo.SaveChanges();

                                        lme.COD_ESTOQUE = estSaida.COD_ESTOQUE;
                                        lme.COD_PRODUTO = estSaida.COD_PRODUTO;
                                        lme.DATA_MOVIMENTACAO = DateTime.Now;
                                        lme.DESC_PRODUTO = estSaida.DESC_PRODUTO;
                                        lme.ENTRADA_SAIDA = "S";
                                        lme.EPC = estSaida.EPC;
                                        lme.ESTOQUE = estSaida.ESTOQUE;
                                        lme.FK_PRODUTO = estSaida.FK_PRODUTO;
                                        lme.QUANTIDADE = estSaida.QUANTIDADE;
                                        lme.STATUS = "S";
                                        lme.DESC_STATUS = "Saida Estoque";
                                        dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                        dbo.SaveChanges();
                                        mensagem = "OK";
                                        break;
                                }
                            }
                            else
                            {
                                switch (entSaida)
                                {
                                    case "E":
                                        mensagem = "Este item ja esta em Estoque";
                                        break;
                                    case "S":
                                        mensagem = "Este item ja esta em Saida de Estoque";
                                        break;
                                }
                            }


                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.Resultado = mensagem;
                            mv.EPC = epc;
                            mv.CorAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.CorAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                        }
                    }
                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> recebimentoEstoque(string listaEPCS)
        {
            try
            {
                var gdi = Guid.NewGuid();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                string[] lines = listaEPCS.Split('|');
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        var pti = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                        var epcNoEstoque = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                        if (epcNoEstoque.Count == 0)
                        {
                            if (pti != null)
                            {
                                if (pti.Count > 0)
                                {
                                    foreach (var itens in pti)
                                    {
                                        bool erro = false;
                                        string resultado = "";

                                        if (DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days >= 0)
                                        {
                                            erro = true;
                                            resultado = resultado + "\n" + itens.DT_VALIDADE.Value + " Data de Validade Vencida";
                                        }

                                        if (DateTime.Now.Subtract(itens.VALIDADE_TESTE.Value).Days >= 0)
                                        {
                                            erro = true;
                                            resultado = resultado + "\n" + itens.VALIDADE_TESTE.Value + " Data de Teste Vencida";
                                        }

                                        if (!erro)
                                        {
                                            L_ESTOQUE le = new L_ESTOQUE();
                                            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();

                                            le.COD_PRODUTO = itens.COD_PRODUTO;
                                            le.COD_RECEBIMENTO = gdi;
                                            le.DATA_ENTRADA = DateTime.Now;
                                            le.DESC_PRODUTO = itens.PRODUTO;
                                            le.ENTRADA_SAIDA = "R";
                                            le.EPC = itens.EPC;
                                            le.QUANTIDADE = 1;
                                            le.FK_PRODUTO = dbo.L_PRODUTOS.First(x => x.COD_PRODUTO == itens.COD_PRODUTO).ID;
                                            le.STATUS = "R";
                                            le.DESC_STATUS = "Recebido";
                                            dbo.L_ESTOQUE.Add(le);
                                            dbo.SaveChanges();

                                            lme.COD_PRODUTO = le.COD_PRODUTO;
                                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                                            lme.DESC_PRODUTO = le.DESC_PRODUTO;
                                            lme.ENTRADA_SAIDA = "E";
                                            lme.EPC = le.EPC;
                                            lme.LAT = "-23.5705321";
                                            lme.LONG = "-46.7064147";
                                            //lme.ESTOQUE = est.ESTOQUE;
                                            lme.FK_PRODUTO = le.FK_PRODUTO;
                                            lme.QUANTIDADE = le.QUANTIDADE;
                                            lme.STATUS = "R";
                                            lme.DESC_STATUS = "Recebido";
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                            dbo.SaveChanges();

                                            mv.Produto = le.DESC_PRODUTO;
                                            mv.Resultado = "OK";
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ffffff";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = itens.PRODUTO;
                                            mv.Resultado = resultado;
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não existe em nossa Base de dados";
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                                }
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item ja foi recebido";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                    }

                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }

        }


        [WebMethod]
        public List<RESULTADOMOV> recebimentoEstoqueCnpj(string listaEPCS, string cnpj)
        {
            try
            {
                Guid gdi = Guid.NewGuid();
                L_ESTOQUE le = new L_ESTOQUE();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                string[] lines = listaEPCS.Split('|');
                bool erro = false;
                string resultado = "";
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        var itens = dbo.L_PRODUTOS_ITENS.First(x => x.EPC == epc);
                        var existe = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                        if (existe.Count == 0)
                        {
                            if (itens != null)
                            {

                                if (cnpj != itens.CNPJ_DESTINATARIO)
                                {
                                    erro = true;
                                    resultado = resultado + "\nEste item está vinculado para outra Empresa";
                                }

                                if (DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days >= 0)
                                {
                                    erro = true;
                                    resultado = resultado + "\n" + itens.DT_VALIDADE.Value + " Data de Validade Vencida";
                                }
                                else
                                {
                                    erro = false;
                                    resultado = "";

                                }

                                if (DateTime.Now.Subtract(itens.VALIDADE_TESTE.Value).Days >= 0)
                                {
                                    erro = true;
                                    resultado = resultado + "\n" + itens.VALIDADE_TESTE.Value + " Data de Teste Vencida";
                                }
                                else
                                {
                                    erro = false;
                                    resultado = "";
                                }

                                if (!erro)
                                {

                                    le.COD_PRODUTO = itens.COD_PRODUTO;
                                    le.COD_RECEBIMENTO = gdi;
                                    le.DATA_ENTRADA = DateTime.Now;
                                    le.DESC_PRODUTO = itens.PRODUTO;
                                    le.ENTRADA_SAIDA = "R";
                                    le.EPC = itens.EPC;
                                    le.QUANTIDADE = 1;
                                    le.FK_PRODUTO = dbo.L_PRODUTOS.Where(x => x.COD_PRODUTO == itens.COD_PRODUTO).ToList()[0].ID;
                                    le.STATUS = "R";
                                    le.DESC_STATUS = "Recebido";
                                    dbo.L_ESTOQUE.Add(le);
                                    dbo.SaveChanges();


                                    lme.COD_PRODUTO = le.COD_PRODUTO;
                                    lme.DATA_MOVIMENTACAO = DateTime.Now;
                                    lme.DESC_PRODUTO = le.DESC_PRODUTO;
                                    lme.ENTRADA_SAIDA = "E";
                                    lme.EPC = le.EPC;
                                    lme.LAT = "-23.5705321";
                                    lme.LONG = "-46.7064147";
                                    //lme.ESTOQUE = est.ESTOQUE;
                                    lme.FK_PRODUTO = le.FK_PRODUTO;
                                    lme.QUANTIDADE = le.QUANTIDADE;
                                    lme.STATUS = "R";
                                    lme.DESC_STATUS = "Recebido";
                                    dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                    dbo.SaveChanges();

                                    mv.Produto = le.DESC_PRODUTO;
                                    mv.Resultado = "OK";
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ffffff";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                }
                                else
                                {
                                    mv.Produto = itens.PRODUTO;
                                    mv.Resultado = resultado;
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                }



                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item já foi recebido";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                    }

                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }

        }

        [WebMethod]
        public List<RESULTADOMOV> atribuicaoCracha(string matricula, string cracha)
        {
            RESULTADOMOV resultadoMovimentacao = new RESULTADOMOV();
            List<RESULTADOMOV> listaResultadoMovimentacao = new List<RESULTADOMOV>();

            try
            {
                List<L_ATRIBUICAOCRACHA> atribuicaoCrachaList = dbo.L_ATRIBUICAOCRACHA.Where(x => x.CODIGO_CRACHA == cracha).ToList();
                List<L_PRODUTOS_ITENS> ProdutosItensList = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == cracha).ToList();

                //Verifica se o EPCCode escaneado já foi atrubuído como crachá ou EPI
                if (atribuicaoCrachaList.Any() || ProdutosItensList.Any())
                {
                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                    resultadoMovimentacao.EPC = cracha;
                    resultadoMovimentacao.Resultado = "Ítem já atribuído a um funcionário ou produto.";
                    resultadoMovimentacao.CorAviso = "#FFFFFF";
                    resultadoMovimentacao.HasError = true;

                    listaResultadoMovimentacao.Add(resultadoMovimentacao);

                    return listaResultadoMovimentacao;
                }

                //Busca de funcionários pela matrícula passada como parâmetro
                List<L_FUNCIONARIOS> listaFuncionarios = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();

                // Checa se a consulta retornou algum registro e preencheu a lista de funcionários
                if (listaFuncionarios.Count > 0)
                {
                    L_FUNCIONARIOS funcionario = listaFuncionarios[0];

                    // Na base de dados podem existir funcionários com matrículas idênticas, porém, somente se forem
                    // de empresas distintas
                    if (listaFuncionarios.Count > 1)
                    {
                        foreach (L_FUNCIONARIOS func in listaFuncionarios)
                        {
                            //Checa qual o CNPJ da lista combina com o da Sessão ativa
                            if (func.CNPJ == Sessao.Cnpj)
                            {
                                funcionario = func;

                                break;
                            }
                        }
                    }

                    // Monta o objeto e o salva na base de dados
                    L_ATRIBUICAOCRACHA atribuicaoCracha = new L_ATRIBUICAOCRACHA();

                    atribuicaoCracha.ATIVO = "S";
                    atribuicaoCracha.CODIGO_CRACHA = cracha;
                    atribuicaoCracha.FK_FUNCIONARIO = funcionario.ID;
                    atribuicaoCracha.MATRICULA = funcionario.MATRICULA;
                    atribuicaoCracha.DATA_ATRIBUICAO = DateTime.Now;

                    dbo.L_ATRIBUICAOCRACHA.Add(atribuicaoCracha);
                    dbo.SaveChanges();

                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                    resultadoMovimentacao.EPC = cracha;
                    resultadoMovimentacao.Produto = listaFuncionarios[0].MATRICULA + " - " + listaFuncionarios[0].NOME;
                    resultadoMovimentacao.Resultado = "Crachá atribuído com sucesso.";
                    resultadoMovimentacao.CorAviso = "#FFFFFF";

                    listaResultadoMovimentacao.Add(resultadoMovimentacao);

                    return listaResultadoMovimentacao;
                }

                // Caso a lista de funcionários esteja vazia:
                else
                {
                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                    resultadoMovimentacao.EPC = cracha;
                    resultadoMovimentacao.Resultado = "Matrícula inválida ou inexistente.";
                    resultadoMovimentacao.CorAviso = "#FFFFFF";
                    resultadoMovimentacao.HasError = true;

                    listaResultadoMovimentacao.Add(resultadoMovimentacao);

                    return listaResultadoMovimentacao;
                }
            }

            catch (Exception ex)
            {
                resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                resultadoMovimentacao.EPC = cracha;
                resultadoMovimentacao.Resultado = $"Erro: {ex.Message}";
                resultadoMovimentacao.CorAviso = "#FFFFFF";

                listaResultadoMovimentacao.Add(resultadoMovimentacao);

                return listaResultadoMovimentacao;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> envioParaTeste(string listaEPCS, string estoque)
        {
            try
            {
                string[] lines = listaEPCS.Split('|');
                L_ESTOQUE le = new L_ESTOQUE();
                L_ENVIO_PARA_TESTE lteste = new L_ENVIO_PARA_TESTE();
                var gdi = Guid.NewGuid();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                List<L_ATRIBUICAOCRACHA> cracha = new List<L_ATRIBUICAOCRACHA>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                string codEstoque = "";
                string nomEstoque = "";
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {

                        var pti = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                        if (pti != null)
                        {
                            if (pti.Count > 0)
                            {
                                foreach (var itens in pti)
                                {
                                    try
                                    {
                                        var lEst = dbo.L_ESTOQUE.First(x => x.EPC == epc);
                                        if (lEst.STATUS != "T" && lEst.STATUS != "M" && lEst.STATUS != "Z")
                                        {
                                            var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == estoque).ToList();
                                            if (localEstoque != null)
                                            {
                                                if (localEstoque.Count > 0)
                                                {

                                                    codEstoque = localEstoque[0].CODIGO;
                                                    nomEstoque = localEstoque[0].NOME;
                                                }
                                                else
                                                {
                                                    codEstoque = "0";
                                                    nomEstoque = "Enviado para Teste";
                                                }

                                            }
                                            else
                                            {
                                                codEstoque = "0";
                                                nomEstoque = "Enviado para Teste";
                                            }
                                            if (lEst.ENTRADA_SAIDA == "E")
                                            {
                                                lEst.ENTRADA_SAIDA = "S";
                                                lEst.DATA_SAIDA = DateTime.Now.ToShortTimeString();
                                            }
                                            else
                                            {
                                                lEst.ENTRADA_SAIDA = "E";
                                                lEst.DATA_ENTRADA = DateTime.Now;
                                            }


                                            lEst.FK_FUNCIONARIO_SAIDA = null;
                                            lEst.MATRICULA = null;
                                            lEst.COD_ESTOQUE = codEstoque;
                                            lEst.ESTOQUE = nomEstoque;
                                            lEst.STATUS = "T";
                                            lEst.DESC_STATUS = "Em Testes Cliente ou Fornecedor";
                                            dbo.SaveChanges();

                                            lme.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lme.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                                            lme.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lme.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lme.EPC = lEst.EPC;
                                            lme.ESTOQUE = lEst.ESTOQUE;
                                            lme.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lme.QUANTIDADE = lEst.QUANTIDADE;
                                            lme.STATUS = "T";
                                            lme.DESC_STATUS = "Em Testes Cliente ou Fornecedor";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                            dbo.SaveChanges();

                                            lteste.DATA = DateTime.Now;
                                            lteste.EPC = lme.EPC;
                                            lteste.FK_MOVIMENTACAO = lme.ID;
                                            lteste.TESTE = "S";
                                            dbo.L_ENVIO_PARA_TESTE.Add(lteste);
                                            dbo.SaveChanges();

                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Item Enviado para Teste com Sucesso";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = lEst.DESC_STATUS;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ff7f7f";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                    }
                                    catch
                                    {
                                        if (DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days < 0)
                                        {

                                            var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == estoque).ToList();
                                            if (localEstoque != null)
                                            {
                                                if (localEstoque.Count > 0)
                                                {

                                                    codEstoque = localEstoque[0].CODIGO;
                                                    nomEstoque = localEstoque[0].NOME;
                                                }
                                                else
                                                {
                                                    codEstoque = "0";
                                                    nomEstoque = "Enviado para Teste";
                                                }

                                            }
                                            else
                                            {
                                                codEstoque = "0";
                                                nomEstoque = "Enviado para Teste";
                                            }

                                            L_ESTOQUE lEst = new L_ESTOQUE();
                                            lEst.COD_PRODUTO = itens.COD_PRODUTO;
                                            lEst.DESC_PRODUTO = itens.PRODUTO;
                                            lEst.EPC = itens.EPC;
                                            lEst.COD_RECEBIMENTO = Guid.NewGuid();
                                            lEst.FK_PRODUTO = dbo.L_PRODUTOS.Where(x => x.COD_PRODUTO == itens.COD_PRODUTO).ToList()[0].ID;
                                            lEst.QUANTIDADE = 1;
                                            lEst.FK_FUNCIONARIO_SAIDA = null;
                                            lEst.MATRICULA = null;
                                            lEst.COD_ESTOQUE = codEstoque;
                                            lEst.ESTOQUE = nomEstoque;
                                            lEst.STATUS = "T";
                                            lEst.DESC_STATUS = "Em Testes Cliente ou Fornecedor";
                                            lEst.ENTRADA_SAIDA = "S";
                                            dbo.L_ESTOQUE.Add(lEst);
                                            dbo.SaveChanges();

                                            L_MOVIMENTACAO_ESTOQUE lmes = new L_MOVIMENTACAO_ESTOQUE();
                                            lmes.ENTRADA_SAIDA = "S";
                                            lmes.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lmes.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lmes.DATA_MOVIMENTACAO = DateTime.Now;
                                            lmes.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lmes.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lmes.EPC = lEst.EPC;
                                            lmes.ESTOQUE = lEst.ESTOQUE;
                                            lmes.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lmes.QUANTIDADE = lEst.QUANTIDADE;
                                            lmes.STATUS = "T";
                                            lmes.DESC_STATUS = "Em Testes Cliente ou Fornecedor";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lmes);
                                            dbo.SaveChanges();

                                            L_ENVIO_PARA_TESTE ltst = new L_ENVIO_PARA_TESTE();
                                            ltst.DATA = DateTime.Now;
                                            ltst.EPC = lEst.EPC;
                                            ltst.FK_MOVIMENTACAO = lmes.ID;
                                            ltst.TESTE = "S";
                                            dbo.L_ENVIO_PARA_TESTE.Add(ltst);
                                            dbo.SaveChanges();


                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Item Enviado para Teste com Sucesso";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });


                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Data de Validade Vencida";
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }


                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não existe em nossa Base de dados";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                        }

                    }
                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> recebimentoTeste(string listaEPCS, string dataTeste, string art)
        {
            try
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                L_ENVIO_PARA_TESTE lteste = new L_ENVIO_PARA_TESTE();
                string[] lines = listaEPCS.Split('|');
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        var estoque = dbo.L_ESTOQUE.Where(x => x.EPC == epc && x.STATUS == "T").ToList();
                        if (estoque != null)
                        {
                            if (estoque.Count > 0)
                            {
                                if (estoque[0].STATUS == "T")
                                {
                                    var epcTeste = dbo.L_ENVIO_PARA_TESTE.Where(x => x.EPC == epc).OrderByDescending(x => x.DATA).ToList();
                                    if (epcTeste.Count > 0)
                                    {

                                        var item = epcTeste[0];

                                        if (item.TESTE == "S")
                                        {
                                            var lEst = dbo.L_ESTOQUE.First(x => x.EPC == epc);


                                            lEst.ENTRADA_SAIDA = "E";
                                            lEst.DATA_ENTRADA = DateTime.Now;



                                            //lEst.ENTRADA_SAIDA = "E";
                                            //lEst.DATA_ENTRADA = DateTime.Now;
                                            lEst.FK_FUNCIONARIO_SAIDA = lEst.FK_FUNCIONARIO_SAIDA;
                                            lEst.MATRICULA = lEst.MATRICULA;
                                            lEst.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lEst.ESTOQUE = lEst.ESTOQUE;
                                            lEst.STATUS = "A";
                                            lEst.DESC_STATUS = "Recebimento de Itens Testado";
                                            dbo.SaveChanges();

                                            lme.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lme.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                                            lme.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lme.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lme.EPC = lEst.EPC;
                                            lme.ESTOQUE = lEst.ESTOQUE;
                                            lme.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lme.QUANTIDADE = lEst.QUANTIDADE;
                                            lme.STATUS = "A";
                                            lme.DESC_STATUS = "Recebimento de Itens Testado";
                                            lme.LAT = "-23.5705321";
                                            lme.LONG = "-46.7064147";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                            dbo.SaveChanges();

                                            L_ENVIO_PARA_TESTE ltest = dbo.L_ENVIO_PARA_TESTE.First(x => x.ID == item.ID);
                                            ltest.ART = art;
                                            ltest.DATA_PROXIMO_TESTE = DateTime.Parse(dataTeste);
                                            ltest.TESTE = "N";
                                            ltest.DATA_RETORNO = DateTime.Now;
                                            dbo.SaveChanges();

                                            L_PRODUTOS_ITENS lpi = dbo.L_PRODUTOS_ITENS.First(x => x.EPC == epc);
                                            lpi.VALIDADE_TESTE = ltest.DATA_PROXIMO_TESTE;
                                            dbo.SaveChanges();

                                            L_PRODUTOS lp = dbo.L_PRODUTOS.First(x => x.COD_PRODUTO == lpi.COD_PRODUTO);
                                            lp.VALIDADE_TESTE = ltest.DATA_PROXIMO_TESTE;
                                            dbo.SaveChanges();


                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Item Recebido com Sucesso";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Este item não esta em Teste";
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }

                                    }
                                    else
                                    {
                                        mv.Produto = "";
                                        mv.Resultado = "Este item não esta em Teste";
                                        mv.EPC = epc;
                                        mv.CorAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não esta em Teste";
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não esta em Teste";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não esta em Teste";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                    }
                }

                return mov;

            }
            catch
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = "Erro";
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> inspecaoEPIFuncionario(string listaEPCS, string latitude, string longitude)
        {
            try
            {
                string[] lines = listaEPCS.Split('|');
                string codCracha = "";
                L_ESTOQUE le = new L_ESTOQUE();
                var gdi = Guid.NewGuid();
                bool existeCracha = false;
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                List<L_ATRIBUICAOCRACHA> cracha = new List<L_ATRIBUICAOCRACHA>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                L_INSPECAOFUNCIONARIO li = new L_INSPECAOFUNCIONARIO();
                L_INSPITEM lit = new L_INSPITEM();
                string mensagem = String.Empty;
                bool erro = false;
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        if (!existeCracha)
                        {
                            cracha = dbo.L_ATRIBUICAOCRACHA.Where(x => x.CODIGO_CRACHA == epc).ToList();
                            if (cracha.Count > 0)
                            {
                                codCracha = cracha[0].CODIGO_CRACHA;
                                existeCracha = true;
                            }
                        }
                    }
                }

                if (codCracha != "")
                {
                    foreach (var epc in lines)
                    {
                        if (epc != "")
                        {
                            if (epc != codCracha)
                            {
                                var pti = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                                if (pti != null && pti.Count > 0)
                                {

                                    foreach (var itens in pti)
                                    {
                                        mensagem = "";
                                        erro = false;

                                        var statusEstoque = dbo.L_ESTOQUE.Select(x => new { x.STATUS, x.EPC, x.DESC_STATUS }).Where(x => x.EPC == epc).ToList();

                                        if (statusEstoque != null && statusEstoque.Count > 0)
                                        {

                                            if (statusEstoque[0].STATUS == "B")
                                            {

                                                erro = true;
                                                mensagem = mensagem + "EPI não atribuido para um funcionario, foi devolvido ao Estoque;";

                                                #region
                                                //resultadoMovimentacao.Produto = "";
                                                //resultadoMovimentacao.Resultado = "EPI não atribuido para um funcionario, foi devolvido ao Estoque";
                                                //resultadoMovimentacao.EPC = epc;
                                                //resultadoMovimentacao.CorAviso = "#ff7f7f";
                                                //resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                                //listaResultadoMovimentacao.Add(new RESULTADOMOV { Resultado = resultadoMovimentacao.Resultado, EPC = resultadoMovimentacao.EPC, DataMovimentacao = resultadoMovimentacao.DataMovimentacao, Produto = resultadoMovimentacao.Produto, CorAviso = resultadoMovimentacao.CorAviso });

                                                //li.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                                //li.COD_INSPECAO = gdi.ToString();
                                                //li.COD_PRODUTO = itens.COD_PRODUTO;
                                                //li.DATA_INSPECAO = DateTime.Now;
                                                //li.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //li.DATA_VALIDADETESTE = itens.VALIDADE_TESTE;
                                                //li.DESC_PRODUTO = itens.PRODUTO;
                                                //li.EPC = itens.EPC;
                                                //var mat = cracha[0].MATRICULA;
                                                //li.FUNCIONARIO = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == mat).ToList()[0].NOME;
                                                //li.MOTIVO = "EPI não atribuido para um funcionario, foi devolvido ao Estoque";
                                                //li.LAT = latitude.Replace(',', '.');
                                                //li.LONG = longitude.Replace(',', '.');
                                                //dbo.L_INSPECAOFUNCIONARIO.Add(li);
                                                //dbo.SaveChanges();

                                                //lit.CA = itens.CA;
                                                //lit.COD_FORNECEDOR = itens.COD_FORNECEDOR;
                                                //lit.COD_PRODUTO = itens.COD_PRODUTO;
                                                //lit.DATA_FABRICACAO = itens.DT_FABRICACAO;
                                                //lit.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //lit.DATA_VALIDADE_TESTE = itens.VALIDADE_TESTE;
                                                //lit.DESC_PRODUTO = itens.PRODUTO;
                                                //lit.EPC = itens.EPC;
                                                //lit.FK_INSPECAO = li.ID;
                                                //lit.LOTE = itens.NUMERO_LOTE;
                                                //lit.SITUACAO = "EPI não atribuido para um funcionario, foi devolvido ao Estoque";
                                                //dbo.L_INSPITEM.Add(lit);
                                                //dbo.SaveChanges();
                                                #endregion

                                            }


                                            if (statusEstoque[0].STATUS == "O")
                                            {

                                                erro = true;
                                                mensagem = mensagem + "EPI não atribuido para um funcionario, aguardando assinatura digital;";

                                                #region
                                                //resultadoMovimentacao.Produto = "";
                                                //resultadoMovimentacao.Resultado = "EPI não atribuido para um funcionario, aguardando assinatura digital";
                                                //resultadoMovimentacao.EPC = epc;
                                                //resultadoMovimentacao.CorAviso = "#ff7f7f";
                                                //resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                                //listaResultadoMovimentacao.Add(new RESULTADOMOV { Resultado = resultadoMovimentacao.Resultado, EPC = resultadoMovimentacao.EPC, DataMovimentacao = resultadoMovimentacao.DataMovimentacao, Produto = resultadoMovimentacao.Produto, CorAviso = resultadoMovimentacao.CorAviso });

                                                //li.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                                //li.COD_INSPECAO = gdi.ToString();
                                                //li.COD_PRODUTO = itens.COD_PRODUTO;
                                                //li.DATA_INSPECAO = DateTime.Now;
                                                //li.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //li.DATA_VALIDADETESTE = itens.VALIDADE_TESTE;
                                                //li.DESC_PRODUTO = itens.PRODUTO;
                                                //li.EPC = itens.EPC;
                                                //var mat = cracha[0].MATRICULA;
                                                //li.FUNCIONARIO = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == mat).ToList()[0].NOME;
                                                //li.MOTIVO = "EPI não atribuido para um funcionario, aguardando assinatura digital";
                                                //li.LAT = latitude.Replace(',', '.');
                                                //li.LONG = longitude.Replace(',', '.');
                                                //dbo.L_INSPECAOFUNCIONARIO.Add(li);
                                                //dbo.SaveChanges();

                                                //lit.CA = itens.CA;
                                                //lit.COD_FORNECEDOR = itens.COD_FORNECEDOR;
                                                //lit.COD_PRODUTO = itens.COD_PRODUTO;
                                                //lit.DATA_FABRICACAO = itens.DT_FABRICACAO;
                                                //lit.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //lit.DATA_VALIDADE_TESTE = itens.VALIDADE_TESTE;
                                                //lit.DESC_PRODUTO = itens.PRODUTO;
                                                //lit.EPC = itens.EPC;
                                                //lit.FK_INSPECAO = li.ID;
                                                //lit.LOTE = itens.NUMERO_LOTE;
                                                //lit.SITUACAO = "EPI não atribuido para um funcionario, aguardando assinatura digital";
                                                //dbo.L_INSPITEM.Add(lit);
                                                //dbo.SaveChanges();
                                                #endregion

                                            }

                                            if (statusEstoque[0].STATUS == "A"
                                                || statusEstoque[0].STATUS == "M"
                                                || statusEstoque[0].STATUS == "H"
                                                || statusEstoque[0].STATUS == "T"
                                                || statusEstoque[0].STATUS == "Z")
                                            {
                                                erro = true;
                                                mensagem = mensagem + "EPI não atribuido para um funcionario, " + statusEstoque[0].DESC_STATUS + ";";

                                                #region
                                                //resultadoMovimentacao.Produto = "";
                                                //resultadoMovimentacao.Resultado = "EPI não atribuido para um funcionario, " + statusEstoque[0].DESC_STATUS;
                                                //resultadoMovimentacao.EPC = epc;
                                                //resultadoMovimentacao.CorAviso = "#ff7f7f";
                                                //resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                                //listaResultadoMovimentacao.Add(new RESULTADOMOV { Resultado = resultadoMovimentacao.Resultado, EPC = resultadoMovimentacao.EPC, DataMovimentacao = resultadoMovimentacao.DataMovimentacao, Produto = resultadoMovimentacao.Produto, CorAviso = resultadoMovimentacao.CorAviso });

                                                //li.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                                //li.COD_INSPECAO = gdi.ToString();
                                                //li.COD_PRODUTO = itens.COD_PRODUTO;
                                                //li.DATA_INSPECAO = DateTime.Now;
                                                //li.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //li.DATA_VALIDADETESTE = itens.VALIDADE_TESTE;
                                                //li.DESC_PRODUTO = itens.PRODUTO;
                                                //li.EPC = itens.EPC;
                                                //var mat = cracha[0].MATRICULA;
                                                //li.FUNCIONARIO = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == mat).ToList()[0].NOME;
                                                //li.MOTIVO = "EPI não atribuido para um funcionario, " + statusEstoque[0].DESC_STATUS;
                                                //li.LAT = latitude.Replace(',', '.');
                                                //li.LONG = longitude.Replace(',', '.');
                                                //dbo.L_INSPECAOFUNCIONARIO.Add(li);
                                                //dbo.SaveChanges();

                                                //lit.CA = itens.CA;
                                                //lit.COD_FORNECEDOR = itens.COD_FORNECEDOR;
                                                //lit.COD_PRODUTO = itens.COD_PRODUTO;
                                                //lit.DATA_FABRICACAO = itens.DT_FABRICACAO;
                                                //lit.DATA_VALIDADE = itens.DT_VALIDADE;
                                                //lit.DATA_VALIDADE_TESTE = itens.VALIDADE_TESTE;
                                                //lit.DESC_PRODUTO = itens.PRODUTO;
                                                //lit.EPC = itens.EPC;
                                                //lit.FK_INSPECAO = li.ID;
                                                //lit.LOTE = itens.NUMERO_LOTE;
                                                //lit.SITUACAO = "EPI não atribuido para um funcionario, " + statusEstoque[0].DESC_STATUS;
                                                //dbo.L_INSPITEM.Add(lit);
                                                //dbo.SaveChanges();
                                                #endregion

                                            }
                                        }

                                        try
                                        {
                                            if (dateDiffTeste(itens.VALIDADE_TESTE.Value.ToShortDateString()) <= 0)
                                            {
                                                erro = true;
                                                mensagem = mensagem + "Validade de Teste Vencida;";
                                            }

                                            if (dateDiffTeste(itens.DT_VALIDADE.Value.ToShortDateString()) <= 0)
                                            {
                                                erro = true;
                                                mensagem = mensagem + "Validade Vencida;";
                                            }
                                            var matric = cracha[0].MATRICULA;
                                            var epiFunc = dbo.L_ESTOQUE.Where<L_ESTOQUE>(x => x.EPC == itens.EPC).ToList();

                                            if (epiFunc != null)
                                            {
                                                if (epiFunc.Count > 0)
                                                {
                                                    if (epiFunc[0].MATRICULA == "" || epiFunc[0].MATRICULA == null)
                                                    {
                                                        erro = true;
                                                        mensagem = mensagem + "Este EPI não pertence a nenhum usuario;";
                                                    }
                                                    else
                                                    {
                                                        if (epiFunc[0].MATRICULA != cracha[0].MATRICULA)
                                                        {
                                                            if (statusEstoque[0].STATUS != "O")
                                                            {
                                                                erro = true;
                                                                mensagem = mensagem + "EPI Trocado";
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    erro = true;
                                                    mensagem = mensagem + "Este EPI não pertence a nenhum usuario;";
                                                }
                                            }
                                            else
                                            {
                                                erro = true;
                                                mensagem = mensagem + "Este EPI não pertence a nenhum usuario;";
                                            }

                                            if (!erro)
                                            {
                                                mensagem = "OK";
                                                mv.CorAviso = "#ffffff";
                                            }
                                            else
                                            {
                                                mv.CorAviso = "#ff7f7f";
                                            }

                                            mv.Produto = itens.PRODUTO + " - Funcionario=" + cracha[0].MATRICULA;
                                            mv.Resultado = mensagem;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;

                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                                            li.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                            li.COD_INSPECAO = gdi.ToString();
                                            li.COD_PRODUTO = itens.COD_PRODUTO;
                                            li.DATA_INSPECAO = DateTime.Now;
                                            li.DATA_VALIDADE = itens.DT_VALIDADE;
                                            li.DATA_VALIDADETESTE = itens.VALIDADE_TESTE;
                                            li.DESC_PRODUTO = itens.PRODUTO;
                                            li.EPC = itens.EPC;
                                            var mat = cracha[0].MATRICULA;
                                            li.FUNCIONARIO = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == mat).ToList()[0].NOME;
                                            li.MOTIVO = mensagem;
                                            li.LAT = latitude.Replace(',', '.');
                                            li.LONG = longitude.Replace(',', '.');
                                            dbo.L_INSPECAOFUNCIONARIO.Add(li);
                                            dbo.SaveChanges();

                                            lit.CA = itens.CA;
                                            lit.COD_FORNECEDOR = itens.COD_FORNECEDOR;
                                            lit.COD_PRODUTO = itens.COD_PRODUTO;
                                            lit.DATA_FABRICACAO = itens.DT_FABRICACAO;
                                            lit.DATA_VALIDADE = itens.DT_VALIDADE;
                                            lit.DATA_VALIDADE_TESTE = itens.VALIDADE_TESTE;
                                            lit.DESC_PRODUTO = itens.PRODUTO;
                                            lit.EPC = itens.EPC;
                                            lit.FK_INSPECAO = li.ID;
                                            lit.LOTE = itens.NUMERO_LOTE;
                                            lit.SITUACAO = mensagem;
                                            dbo.L_INSPITEM.Add(lit);
                                            dbo.SaveChanges();


                                        }
                                        catch (Exception er)
                                        {


                                            mv.Produto = le.DESC_PRODUTO + " - Funcionario=" + cracha[0].MATRICULA;
                                            mv.Resultado = er.Message.ToString();
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }



                                    }


                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este EPI não existe em nossa Base de dados";
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                                }
                            }
                        }
                    }
                }
                else
                {

                    mv.DataMovimentacao = DateTime.Now;
                    mv.EPC = "";
                    mv.Resultado = "Não Foi Encontrado Cracha do Funcionario para realizar a Inspeção";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    mv.CorAviso = "#ff7f7f";
                    return mov;
                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }


        }

        [WebMethod]
        public List<RESULTADOMOV> itemNaoConforme(string listaEPCS, string motivo)
        {
            List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
            RESULTADOMOV mv = new RESULTADOMOV();
            string[] lines = listaEPCS.Split('|');
            L_ESTOQUE le = new L_ESTOQUE();
            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
            L_NAOCONFORMIDADE lnc = new L_NAOCONFORMIDADE();
            string mensagem = "";
            foreach (var epc in lines)
            {
                mensagem = "";
                if (epc != "")
                {

                    var result = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                    if (result.Count == 0)
                    {
                        mv.Resultado = "Epi Não Existe no Estoque";
                        mv.EPC = epc;
                        mv.DataMovimentacao = DateTime.Now;
                        mv.Produto = "";
                        mv.CorAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                    }
                    else
                    {
                        var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == "98f42739").ToList();
                        int ID = result[0].ID;
                        L_ESTOQUE est = dbo.L_ESTOQUE.First(x => x.ID == ID);
                        est.ENTRADA_SAIDA = "E";
                        est.COD_ESTOQUE = localEstoque[0].CODIGO;
                        est.ESTOQUE = localEstoque[0].NOME;
                        est.STATUS = "N";
                        lme.DESC_STATUS = "Não Conformidade";
                        dbo.SaveChanges();

                        lme.COD_ESTOQUE = est.COD_ESTOQUE;
                        lme.COD_PRODUTO = est.COD_PRODUTO;
                        lme.DATA_MOVIMENTACAO = DateTime.Now;
                        lme.DESC_PRODUTO = est.DESC_PRODUTO;
                        lme.ENTRADA_SAIDA = "E";
                        lme.EPC = est.EPC;
                        lme.ESTOQUE = est.ESTOQUE;
                        lme.FK_PRODUTO = est.FK_PRODUTO;
                        lme.QUANTIDADE = est.QUANTIDADE;
                        lme.STATUS = "N";
                        lme.DESC_STATUS = "Não Conformidade";
                        dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                        dbo.SaveChanges();
                        mensagem = "OK";

                        lnc.DATA_CONFORMIDADE = DateTime.Now;
                        lnc.EPC = est.EPC;
                        lnc.MOTIVO = motivo;
                        dbo.L_NAOCONFORMIDADE.Add(lnc);
                        dbo.SaveChanges();

                        mv.Produto = result[0].DESC_PRODUTO;
                        mv.Resultado = mensagem;
                        mv.EPC = epc;
                        mv.CorAviso = "#ffffff";
                        mv.DataMovimentacao = DateTime.Now;
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                    }






                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> manutencaoEPI(string listaEPCS, string estoque)
        {
            List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
            RESULTADOMOV mv = new RESULTADOMOV();
            string[] lines = listaEPCS.Split('|');
            L_ESTOQUE le = new L_ESTOQUE();
            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
            L_MANUTENCAOEPI lm = new L_MANUTENCAOEPI();
            string mensagem = "";
            foreach (var epc in lines)
            {
                mensagem = "";
                if (epc != "")
                {
                    var result = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                    if (result.Count == 0)
                    {
                        mv.Resultado = "Epi Não Existe no Estoque";
                        mv.EPC = epc;
                        mv.DataMovimentacao = DateTime.Now;
                        mv.Produto = "";
                        mv.CorAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                    }
                    else
                    {

                        if (result[0].STATUS != "T" && result[0].STATUS != "M" && result[0].STATUS != "Z")
                        {
                            int IDS = result[0].ID;
                            L_ESTOQUE estSaida = dbo.L_ESTOQUE.First(x => x.ID == IDS);
                            estSaida.ENTRADA_SAIDA = "S";
                            estSaida.STATUS = "M";
                            estSaida.DESC_STATUS = "Em Manutenção Cliente ou Fornecedor";
                            estSaida.FK_FUNCIONARIO_SAIDA = null;
                            estSaida.MATRICULA = null;
                            dbo.SaveChanges();

                            lme.COD_ESTOQUE = estSaida.COD_ESTOQUE;
                            lme.COD_PRODUTO = estSaida.COD_PRODUTO;
                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                            lme.DESC_PRODUTO = estSaida.DESC_PRODUTO;
                            lme.ENTRADA_SAIDA = "S";
                            lme.EPC = estSaida.EPC;
                            lme.ESTOQUE = estSaida.ESTOQUE;
                            lme.FK_PRODUTO = estSaida.FK_PRODUTO;
                            lme.QUANTIDADE = estSaida.QUANTIDADE;
                            lme.STATUS = "M";
                            lme.DESC_STATUS = "Em Manutenção Cliente ou Fornecedor";
                            estSaida.FK_FUNCIONARIO_SAIDA = null;
                            estSaida.MATRICULA = null;
                            lme.LAT = "-23.5705321";
                            lme.LONG = "-46.7064147";
                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                            dbo.SaveChanges();
                            mensagem = "OK";

                            lm.DATA_MANUTENCAO = DateTime.Now;
                            lm.EPC = lme.EPC;
                            lm.STATUS = "S";
                            dbo.L_MANUTENCAOEPI.Add(lm);
                            dbo.SaveChanges();


                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.Resultado = mensagem;
                            mv.EPC = epc;
                            mv.CorAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = "";
                            mv.CorAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                    }
                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> funcionarioCracha(string cracha)
        {
            try
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                var crach = dbo.L_ATRIBUICAOCRACHA.Where(x => x.MATRICULA == cracha).ToList();
                if (crach.Count > 0)
                {
                    var matric = crach[0].MATRICULA;
                    var funcMatricula = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matric).ToList();

                    mv.Produto = funcMatricula[0].NOME;
                    mv.Resultado = "OK";
                    mv.EPC = funcMatricula[0].MATRICULA;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.CorAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    return mov;

                }
                else
                {
                    mv.Produto = "";
                    mv.Resultado = "Este cracha não esta atribuido";
                    mv.EPC = "";
                    mv.DataMovimentacao = DateTime.Now;
                    mv.CorAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    return mov;
                }
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod]
        public bool FuncionarioTemSenha(string listaEPCS)
        {
            webservicetwos3Entities dbo = new webservicetwos3Entities();
            string[] lines = listaEPCS.Split('|');
            foreach (var epc in lines)
            {
                var atr = dbo.L_LOGIN_FUNCIONARIO.Where(x => x.CRACHA == epc).ToList();
                if (atr != null)
                {
                    if (atr.Count > 0)
                    {


                        if (atr[0].SENHA == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }

        [WebMethod]
        public List<RESULTADOMOV> cadastrarSenha(string senha, string matricula)
        {
            try
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                L_LOGIN_FUNCIONARIO llf = new L_LOGIN_FUNCIONARIO();
                var funcMatricula = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                if (funcMatricula.Count > 0)
                {
                    var crach = dbo.L_ATRIBUICAOCRACHA.Where(x => x.MATRICULA == matricula).ToList();

                    try
                    {
                        var lfunc = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                        var email = lfunc[0].EMAIL;
                        llf = dbo.L_LOGIN_FUNCIONARIO.First(x => x.MATRICULA == email);
                        llf.CRACHA = crach[0].CODIGO_CRACHA;
                        llf.DESC_FUNCIONARIO = funcMatricula[0].NOME;
                        llf.FK_FUNCIONARIO = funcMatricula[0].ID;
                        //llf.MATRICULA = matricula;
                        llf.SENHA = senha;
                        dbo.SaveChanges();
                    }
                    catch
                    {
                        var lfunc = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                        var email = lfunc[0].EMAIL;
                        llf.CRACHA = crach[0].CODIGO_CRACHA;
                        llf.DESC_FUNCIONARIO = funcMatricula[0].NOME;
                        llf.FK_FUNCIONARIO = funcMatricula[0].ID;
                        llf.MATRICULA = email;
                        llf.SENHA = senha;
                        dbo.L_LOGIN_FUNCIONARIO.Add(llf);
                        dbo.SaveChanges();
                    }

                    var matric = matricula;
                    mv.Produto = funcMatricula[0].NOME;
                    mv.Resultado = "OK";
                    mv.EPC = funcMatricula[0].MATRICULA;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.CorAviso = "#ffffff";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    return mov;

                }
                else
                {
                    mv.Produto = "";
                    mv.Resultado = "Este cracha não esta atribuido";
                    mv.EPC = "";
                    mv.DataMovimentacao = DateTime.Now;
                    mv.CorAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    return mov;
                }
            }
            catch (Exception er)
            {

                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod (EnableSession = true)]
        public List<DADOSLOGIN> loginFuncionario(string loginUsuario, string senha)
        {
            try
            {
                DADOSLOGIN mv = new DADOSLOGIN();
                List<DADOSLOGIN> mov = new List<DADOSLOGIN>();
                var listaUsuarios = dbo.L_LOGIN.Where(x => x.USUARIO == loginUsuario && x.SENHA == senha).ToList();
                if (listaUsuarios.Count > 0)
                {
                    var usuario = listaUsuarios[0];

                    int fkFuncionario = Convert.ToInt32(usuario.FK_USUARIO.ToString());
                    var dadosUsuario = dbo.L_USUARIO_COLABORADOR.First(x => x.id == fkFuncionario);
                    int fkCliente = Convert.ToInt32(dadosUsuario.FK_CLIENTE.ToString());
                    var dadosEmpresa = dbo.L_CLIENTE.First(x => x.ID == fkCliente);

                    mv.Produto = "1";
                    mv.Resultado = "OK";
                    mv.EPC = loginUsuario;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ffffff";
                    mv.Empresa = dadosEmpresa.NOME;
                    mv.Nome = dadosUsuario.nome;
                    mv.FkCliente = Convert.ToInt32(dadosUsuario.FK_CLIENTE.ToString());
                    mv.Cnpj = StdTools.Unformatted(dadosUsuario.cnpj);
                    mov.Add(new DADOSLOGIN
                    {
                        Resultado = mv.Resultado,
                        EPC = mv.EPC,
                        DataMovimentacao = mv.DataMovimentacao,
                        Produto = mv.Produto,
                        corAviso = mv.corAviso,
                        Empresa = mv.Empresa,
                        Nome = mv.Nome,
                        FkCliente = mv.FkCliente,
                        Cnpj = mv.Cnpj
                    });

                    //Controle de sessao
                    //inicialização dos parametros durante login
                    SessaoUsuario sessaoUsuario = controleSessao.criarSessao(Session.SessionID);
                    sessaoUsuario.autenticado = "OK";
                    sessaoUsuario.nomeFuncionario = usuario.NOME;
                    sessaoUsuario.pAcesso = usuario.P_ACESSO;
                    sessaoUsuario.senhaFuncionario = usuario.SENHA;
                    sessaoUsuario.codigoFuncionario = usuario.FK_USUARIO.ToString();

                    Sessao.FkCnpj = Convert.ToInt32(sessaoUsuario.fkCnpj);
                    Sessao.Cnpj = sessaoUsuario.Cnpj;

                    controleSessao.incluirDadosdeSessao(sessaoUsuario);

                    return mov;
                }
                else
                {
                    mv.Produto = "2";
                    mv.Resultado = "Credenciais inválidas";
                    mv.EPC = loginUsuario;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new DADOSLOGIN { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }
            }
            catch (Exception ex)
            {
                DADOSLOGIN mv = new DADOSLOGIN();
                List<DADOSLOGIN> mov = new List<DADOSLOGIN>();
                mv.Produto = "3";
                mv.Resultado = "Verifique sua Conexão!";
                mv.EPC = loginUsuario;
                mv.DataMovimentacao = DateTime.Now;
                mv.corAviso = "#ff7f7f";
                mov.Add(new DADOSLOGIN { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> devolucaoEPI(string listaEPCS, string estoque)
        {
            List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
            RESULTADOMOV mv = new RESULTADOMOV();
            string[] lines = listaEPCS.Split('|');
            L_ESTOQUE le = new L_ESTOQUE();
            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
            L_MANUTENCAOEPI lm = new L_MANUTENCAOEPI();
            string mensagem = "";
            foreach (var epc in lines)
            {
                mensagem = "";
                if (epc != "")
                {
                    var result = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();

                    if (result.Count > 0)
                    {

                        if (result[0].STATUS == "M" || result[0].STATUS == "D")
                        {

                            int IDS = result[0].ID;
                            L_ESTOQUE estSaida = dbo.L_ESTOQUE.First(x => x.ID == IDS);
                            estSaida.ENTRADA_SAIDA = "E";
                            estSaida.STATUS = "B";
                            estSaida.DESC_STATUS = "Devolução de EPI ao Estoque";
                            estSaida.FK_FUNCIONARIO_SAIDA = null;
                            estSaida.MATRICULA = null;
                            dbo.SaveChanges();

                            lme.COD_ESTOQUE = estSaida.COD_ESTOQUE;
                            lme.COD_PRODUTO = estSaida.COD_PRODUTO;
                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                            lme.DESC_PRODUTO = estSaida.DESC_PRODUTO;
                            lme.ENTRADA_SAIDA = "E";
                            lme.EPC = estSaida.EPC;
                            lme.ESTOQUE = estSaida.ESTOQUE;
                            lme.FK_PRODUTO = estSaida.FK_PRODUTO;
                            lme.QUANTIDADE = estSaida.QUANTIDADE;
                            lme.STATUS = "B";
                            lme.DESC_STATUS = "Devolução de EPI ao Estoque";
                            lme.LAT = "-23.5705321";
                            lme.LONG = "-46.7064147";
                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                            dbo.SaveChanges();
                            mensagem = "OK";

                            lm.DATA_MANUTENCAO = DateTime.Now;
                            lm.EPC = lme.EPC;
                            lm.STATUS = "B";
                            dbo.L_MANUTENCAOEPI.Add(lm);
                            dbo.SaveChanges();


                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.Resultado = mensagem;
                            mv.EPC = epc;
                            mv.CorAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.CorAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }

                    }
                    else
                    {
                        mv.Resultado = "Epi Não Existe no Estoque";
                        mv.EPC = epc;
                        mv.DataMovimentacao = DateTime.Now;
                        mv.Produto = "";
                        mv.CorAviso = "#ff7f7f";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                    }
                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> descartarItem(string listaEPCS, string motivo)
        {
            List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
            RESULTADOMOV mv = new RESULTADOMOV();
            string[] lines = listaEPCS.Split('|');
            L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
            L_DESCARTE ldesc = new L_DESCARTE();
            foreach (var epc in lines)
            {

                if (epc != "")
                {
                    try
                    {
                        L_ESTOQUE result = dbo.L_ESTOQUE.First(x => x.EPC == epc);
                        if (result.STATUS != "T" && result.STATUS != "Z" && result.STATUS != "M")
                        {
                            result.FK_FUNCIONARIO_SAIDA = null;
                            result.MATRICULA = null;
                            result.NOME_FUNC_SAIDA = null;
                            result.STATUS = "Z";
                            result.DESC_STATUS = "Descartado";
                            result.ENTRADA_SAIDA = "S";
                            dbo.SaveChanges();


                            lme.COD_PRODUTO = result.COD_PRODUTO;
                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                            lme.DESC_PRODUTO = result.DESC_PRODUTO;
                            lme.ENTRADA_SAIDA = "S";
                            lme.EPC = result.EPC;
                            lme.FK_PRODUTO = result.FK_PRODUTO;
                            lme.QUANTIDADE = result.QUANTIDADE;
                            lme.STATUS = "Z";
                            lme.DESC_STATUS = "Descartado";
                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                            dbo.SaveChanges();

                            ldesc.EPC = epc;
                            ldesc.COD_ESTOQUE = "";
                            ldesc.LOCAL_ESTOQUE = "";
                            ldesc.FK_PRODUTOS_ITENS = result.FK_PRODUTO;
                            ldesc.MOTIVO = motivo;
                            ldesc.DATA_DESCARTE = DateTime.Now;
                            dbo.L_DESCARTE.Add(ldesc);
                            dbo.SaveChanges();

                            mv.Resultado = "OK";
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result.DESC_PRODUTO;
                            mv.CorAviso = "#ffffff";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                        else
                        {
                            mv.Resultado = result.DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result.DESC_PRODUTO;
                            mv.CorAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }

                    }
                    catch
                    {
                        L_ESTOQUE result = new L_ESTOQUE();
                        var lpst = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                        if (lpst != null)
                        {
                            if (lpst.Count > 0)
                            {
                                result.EPC = lpst[0].EPC;
                                result.COD_PRODUTO = lpst[0].COD_PRODUTO;
                                result.DESC_PRODUTO = lpst[0].PRODUTO;
                                result.QUANTIDADE = 1;
                                result.FK_FUNCIONARIO_SAIDA = null;
                                result.MATRICULA = null;
                                result.NOME_FUNC_SAIDA = null;
                                result.STATUS = "Z";
                                result.DESC_STATUS = "Descartado";
                                result.ENTRADA_SAIDA = "S";
                                result.DATA_ENTRADA = DateTime.Now;
                                dbo.L_ESTOQUE.Add(result);
                                dbo.SaveChanges();


                                lme.COD_PRODUTO = result.COD_PRODUTO;
                                lme.DATA_MOVIMENTACAO = DateTime.Now;
                                lme.DESC_PRODUTO = result.DESC_PRODUTO;
                                lme.ENTRADA_SAIDA = "S";
                                lme.EPC = result.EPC;
                                lme.FK_PRODUTO = result.FK_PRODUTO;
                                lme.QUANTIDADE = result.QUANTIDADE;
                                lme.STATUS = "Z";
                                lme.DESC_STATUS = "Descartado";
                                dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                dbo.SaveChanges();

                                ldesc.EPC = epc;
                                ldesc.COD_ESTOQUE = "";
                                ldesc.LOCAL_ESTOQUE = "";
                                ldesc.FK_PRODUTOS_ITENS = result.FK_PRODUTO;
                                ldesc.MOTIVO = motivo;
                                ldesc.DATA_DESCARTE = DateTime.Now;
                                dbo.L_DESCARTE.Add(ldesc);
                                dbo.SaveChanges();

                                mv.Resultado = "OK";
                                mv.EPC = epc;
                                mv.DataMovimentacao = DateTime.Now;
                                mv.Produto = result.DESC_PRODUTO;
                                mv.CorAviso = "#ffffff";
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                            }
                            else
                            {
                                mv.Resultado = "Este item não existe na base de dados";
                                mv.EPC = epc;
                                mv.DataMovimentacao = DateTime.Now;
                                mv.Produto = "";
                                mv.CorAviso = "#ff7f7f";
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                            }
                        }
                        else
                        {
                            mv.Resultado = "Este item não existe na base de dados";
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = "";
                            mv.CorAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }

                    }


                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> consultaEPIouCracha(string listaEPCS, string cnpj)
        {
            try
            {
                var gdi = Guid.NewGuid();
                L_ESTOQUE le = new L_ESTOQUE();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                string[] lines = listaEPCS.Split('|');
                string resultado = "";
                List<L_FUNCIONARIOS> dadosFuncionario = new List<L_FUNCIONARIOS>();
                bool erro = false;
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {

                        var lista_epis = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc && x.CNPJ_DESTINATARIO == cnpj).ToList();

                        if (lista_epis != null)
                        {
                            if (lista_epis.Count > 0)
                            {
                                foreach (var itens in lista_epis)
                                {
                                    resultado = "";
                                    var pti = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                                    if (pti != null)
                                    {
                                        if (pti.Count > 0)
                                        {
                                            if (pti[0].STATUS != "D")
                                            {
                                                resultado = resultado + "\n" + pti[0].DESC_STATUS;
                                            }
                                            var matricula = pti[0].MATRICULA;
                                            if (matricula != null)
                                            {
                                                dadosFuncionario = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                                                resultado = resultado + "\nFuncionario Atribuido=" + dadosFuncionario[0].MATRICULA + " - " + dadosFuncionario[0].NOME + " " + dadosFuncionario[0].SOBRENOME;

                                            }
                                            else
                                            {

                                                resultado = resultado + "\nNenhum Usuario Atribuido a este EPI";

                                            }
                                        }
                                        else
                                        {

                                            resultado = resultado + "\nNenhum Usuario Atribuido a este EPI";
                                        }
                                    }
                                    else
                                    {

                                        resultado = resultado + "\nNenhum Usuario Atribuido a este EPI";
                                    }


                                    if (DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days >= 0)
                                    {
                                        erro = true;
                                        resultado = resultado + "\n" + itens.DT_VALIDADE.Value + " Data de Validade Vencida";
                                    }

                                    if (DateTime.Now.Subtract(itens.VALIDADE_TESTE.Value).Days >= 0)
                                    {
                                        erro = true;
                                        resultado = resultado + "\n" + itens.VALIDADE_TESTE.Value + " Data de Teste Vencida";
                                    }

                                    RESULTADOMOV resultadoMovimentacao = new RESULTADOMOV();

                                    resultadoMovimentacao.Produto = itens.PRODUTO;
                                    resultadoMovimentacao.Resultado = resultado;
                                    resultadoMovimentacao.EPC = epc;
                                    if (erro)
                                    {
                                        resultadoMovimentacao.CorAviso = "#ff7f7f";
                                    }
                                    else
                                    {
                                        resultadoMovimentacao.CorAviso = "#ffffff";
                                    }
                                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                    mov.Add(resultadoMovimentacao);
                                }
                            }
                            else
                            {
                                // Através de Joins verifica na tabela Clientes o CNPJ da empresa logada
                                var funcionario_da_empresa_solicitada =
                                    (from atribuicaoCracha in dbo.L_ATRIBUICAOCRACHA
                                     join funcionarios in dbo.L_FUNCIONARIOS on atribuicaoCracha.FK_FUNCIONARIO equals funcionarios.ID
                                     join cliente in dbo.L_CLIENTE on funcionarios.FK_CLIENTE equals cliente.ID
                                     where atribuicaoCracha.CODIGO_CRACHA == epc && cliente.CNPJ == cnpj
                                     select funcionarios
                                    ).ToList();

                                if (funcionario_da_empresa_solicitada.Count == 0)
                                {
                                    // Não encontrou funcionários na empresa com CNPJ informado
                                    RESULTADOMOV resultadoMovimentacao = new RESULTADOMOV();

                                    resultadoMovimentacao.Produto = "";
                                    resultadoMovimentacao.Resultado = "Este item não existe em nossa Base de dados";
                                    resultadoMovimentacao.EPC = "";
                                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                    resultadoMovimentacao.CorAviso = "#ff7f7f";
                                    mov.Add(resultadoMovimentacao);
                                }
                                else
                                {
                                    // Encontrou funcionários na empresa com CNPJ informado

                                    StringBuilder exibicao_funcionario = new StringBuilder();
                                    exibicao_funcionario.Append(funcionario_da_empresa_solicitada[0].MATRICULA);
                                    exibicao_funcionario.Append(" - ");
                                    exibicao_funcionario.Append(funcionario_da_empresa_solicitada[0].NOME);

                                    RESULTADOMOV resultadoMovimentacao = new RESULTADOMOV();

                                    resultadoMovimentacao.Produto = "";
                                    resultadoMovimentacao.Resultado = exibicao_funcionario.ToString();
                                        
                                    resultadoMovimentacao.EPC = epc;
                                    resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                                    resultadoMovimentacao.CorAviso = "#ffffff";
                                    mov.Add(resultadoMovimentacao);
                                }


                            }
                        }
                        else
                        {
                            RESULTADOMOV resultadoMovimentacao = new RESULTADOMOV();

                            resultadoMovimentacao.Produto = "";
                            resultadoMovimentacao.Resultado = "Este item não existe em nossa Base de dados";
                            resultadoMovimentacao.EPC = epc;
                            resultadoMovimentacao.CorAviso = "#ff7f7f";
                            resultadoMovimentacao.DataMovimentacao = DateTime.Now;
                            mov.Add(resultadoMovimentacao);

                        }
                    }

                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }

        }

        [WebMethod]
        public List<RESULTADOMOV> retornarDadosEpi(string listaEPCS)
        {
            try
            {
                var gdi = Guid.NewGuid();
                L_ESTOQUE le = new L_ESTOQUE();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                string[] lines = listaEPCS.Split('|');
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        var pti = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                        if (pti != null)
                        {
                            if (pti.Count > 0)
                            {
                                foreach (var itens in pti)
                                {

                                    var matricula = itens.MATRICULA;
                                    if (itens.MATRICULA != null)
                                    {
                                        var dadorFuncionario = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                                        mv.Produto = itens.DESC_PRODUTO;
                                        mv.Resultado = "Funcionario Atribuido=" + dadorFuncionario[0].MATRICULA + " - " + dadorFuncionario[0].NOME + " " + dadorFuncionario[0].SOBRENOME;
                                        mv.EPC = epc;
                                        mv.CorAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                    }
                                    else
                                    {

                                        mv.Produto = itens.DESC_PRODUTO;
                                        mv.Resultado = "Nenhum Usuario Atribuido a este EPI";
                                        mv.EPC = epc;
                                        mv.CorAviso = "#ffffff";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                            }
                        }
                    }

                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }

        }

        [WebMethod]
        public int dateDiffTeste(string data)
        {
            System.TimeSpan diffResult = DateTime.Parse(DateTime.Parse(data).ToShortDateString()).Subtract(DateTime.Parse(DateTime.Now.ToShortDateString()));
            return diffResult.Days;
        }

        [WebMethod]
        public string importacaoDeDados(L_PRODUTOS_ITENS lp)
        {
            return "Nenhum dado Recebido";
        }

        [WebMethod]
        public List<DistribuicaoViewModel> ValidaListaCrachas(string listaEpc)
        {
            List<string> epcList = listaEpc.Split('|').ToList();
            List<DistribuicaoViewModel> crachaViewModels = new List<DistribuicaoViewModel>();

            foreach(string epc in epcList)
            {
                var cracha = distribuicaoServices.CrachaHandler(epc);

                if (cracha != null)
                {
                    crachaViewModels.Add(cracha);
                }
            }

            return crachaViewModels;
        }

        [WebMethod]
        public List<DistribuicaoViewModel> ValidaListaEpi(string listaEpc)
        {
            List<string> epcList = listaEpc.Split('|').ToList();
            List<DistribuicaoViewModel> epcViewModels = new List<DistribuicaoViewModel>();
            DistribuicaoViewModel distribuicaoViewModel = new DistribuicaoViewModel();

            foreach (string epc in epcList)
            {
                var epi = distribuicaoServices.EpiHandler(epc);
                epcViewModels.Add(epi);
            }

            return epcViewModels;
        }

        [WebMethod]
        public DistribuicaoSuccessViewModel DistribuiEpi(int crachaId, string epiIdListString)
        {
            return distribuiEpiService.DistribuiEpi(crachaId, epiIdListString);
        }

        [WebMethod]
        public List<DadosEpi> retornarDadosEpiValidar(string listaEpc, string cnpj, int fkCliente)
        {
            List<DadosEpi> listaDadosEpi = new List<DadosEpi>();
            List<string> epcList = listaEpc.Split('|').ToList();

            foreach (string epc in epcList)
            {
                // Busca pelo EPC na tabela L_PRODUTOS_ITENS um registro que corresponda ao EPC iterado e o CNPJ
                var produto = dbo.L_PRODUTOS_ITENS.FirstOrDefault(x => x.EPC == epc && x.CNPJ_DESTINATARIO == cnpj);


                if (produto != null)
                {
                    DadosEpi dadosEpi = new DadosEpi();
                    dadosEpi.CodigoProduto = produto.COD_PRODUTO;
                    dadosEpi.Produto = produto.PRODUTO;
                    dadosEpi.Quantidade = 1;
                    dadosEpi.CodigoFornecedor = produto.COD_FORNECEDOR;
                    dadosEpi.Epc = produto.EPC;

                    listaDadosEpi.Add(dadosEpi);
                }

                else
                {
                    listaDadosEpi.Add(new DadosEpi { CodigoProduto = "0", Produto = "", Quantidade = 0, CodigoFornecedor = "", Epc = "" });
                }


                var listaCrachas = dbo.L_ATRIBUICAOCRACHA.Where(x => x.CODIGO_CRACHA == epc).ToList();

                if (listaCrachas.Any())
                {
                    int fks = Convert.ToInt32(listaCrachas[0].FK_FUNCIONARIO.ToString());
                    var nome = dbo.L_FUNCIONARIOS.Where(x => x.ID == fks && x.FK_CLIENTE == fkCliente).ToList();

                    if (nome.Count > 0)
                    {
                        listaDadosEpi.Add(new DadosEpi { CodigoProduto = "Matricula=" + listaCrachas[0].MATRICULA, Produto = "Funcionario=" + nome[0].NOME + " " + nome[0].SOBRENOME, Quantidade = 1, CodigoFornecedor = "", Epc = listaCrachas[0].CODIGO_CRACHA });
                    }
                }
            }

            return listaDadosEpi;
        }

        private string[] statusOcorrencia(string mensagem)
        {
            try
            {
                string[] latLong = new string[2];
                var sep = mensagem.Split(':')[1];
                sep = sep.Split('(')[0].ToUpper().Trim();
                var res = dbo.L_STATUSOCORRENCIA.Where(x => x.DESC_STATUS.ToUpper().Trim() == sep).ToList();
                if (res.Count > 0)
                {
                    latLong[0] = res[0].STATUS.ToString();
                    latLong[1] = res[0].DESC_STATUS;
                    return latLong;
                }
                else
                {
                    string[] arr2 = { "", "" };
                    return arr2;
                }
            }
            catch
            {
                string[] arr2 = { "", "" };
                return arr2;
            }
        }

        private string[] latitudeLongitudeMensagem(string mensagem)
        {
            try
            {
                string[] latLong = new string[2];
                //EXEMPLO MENSAGEM Leal 2: modulo desativado. (-23.5640773,-46.7084159)
                var sep = mensagem.Split('(')[1];
                latLong[0] = sep.Split(',')[0];
                latLong[1] = sep.Split(',')[1].Replace(")", "");
                return latLong;
            }
            catch
            {
                string[] arr2 = { "", "" };
                return arr2;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> retornarListaMensagem()
        {
            try
            {

                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var result = dbo.L_MENSAGEM_OCORRENCIA.OrderByDescending(x => x.DATA_MENSAGEM).ToList();
                result = result.Where(x => x.OCORRENCIA == null).ToList();
                if (result != null)
                {
                    foreach (var mv in result)
                    {
                        DateTime dt = DateTime.Parse(mv.DATA_MENSAGEM.Value.ToString());
                        mov.Add(new RESULTADOMOV { Resultado = mv.MENSAGEM, EPC = mv.NUMERO, DataMovimentacao = dt, Produto = mv.ID.ToString(), CorAviso = "" });
                    }

                }

                return mov;
            }
            catch { return null; }
        }

        [WebMethod]
        public List<RESULTADOMOV> retornarListaMensagemSupervisor()
        {
            try
            {

                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var result = dbo.L_MENSAGEM_OCORRENCIA.OrderByDescending(x => x.DATA_MENSAGEM).ToList();

                if (result != null)
                {
                    foreach (var mv in result)
                    {
                        DateTime dt = DateTime.Parse(mv.DATA_MENSAGEM.Value.ToString());
                        mov.Add(new RESULTADOMOV
                        {
                            Resultado = mv.MENSAGEM,
                            EPC = mv.NUMERO + " - 1234 - Andre Graciano Local: R.Agostinho Cantu - Lat:-23.5705321 Long:-46.7064147 ",
                            DataMovimentacao = dt,
                            Produto = mv.ID.ToString(),
                            CorAviso = isNull(mv.OCORRENCIA)
                        });
                    }

                }

                return mov;
            }
            catch { return null; }
        }

        private string isNull(string oCORRENCIA)
        {
            if (oCORRENCIA == null)
            {
                return "A";
            }
            else
            {
                return oCORRENCIA;
            }
        }

        /*
        [WebMethod]
        public string alterarStatus(int id, string ocorrencia)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                L_MENSAGEM_OCORRENCIA lmo = dbo.L_MENSAGEM_OCORRENCIA.First(x => x.ID == id);
                lmo.OCORRENCIA = ocorrencia;
                dbo.SaveChanges();

                var rst = dbo.P_OCORRENCIA.Where(x => x.ID_MENSAGEM_OCORRENCIA == id).ToList();

                if (rst != null)
                {
                    if (rst.Count > 0)
                    {
                        P_HISTORICOATENDIMENTO pha = new P_HISTORICOATENDIMENTO();
                        pha.DATA = DateTime.Now;
                        pha.DESCRICAO = descStatus(ocorrencia);
                        pha.ID_OCORRENCIA = rst[0].ID_OCORRENCIA;
                        pha.ID_USUARIO = 1;
                        pha.HORA = TimeSpan.Parse(DateTime.Now.ToShortTimeString());
                        dbo.P_HISTORICOATENDIMENTO.Add(pha);
                        dbo.SaveChanges();
                    }
                }

                return "1";

            }
            catch (Exception ER){
                return "0";
            }
        }
        */

        private string descStatus(string v)
        {
            switch (v)
            {
                case "A": return "Aguardando Confirmação de Leitura da Ocorrência";
                case "1": return "Leitura Confirmada";
                case "2": return "No Local";
                case "3": return "Funcionario Socorrido";
                case "4": return "Aguardando Socorro Imediato";
            }

            return "Aguardando Confirmação de Leitura da Ocorrência";
        }

        [WebMethod]
        public string dataDif(string dt1)
        {
            return DateTime.Now.Subtract(DateTime.Parse(dt1)).Days.ToString();
        }


        [WebMethod]
        public List<DadosEpi> retornarDadosEpiValidarRecebimento(string listaEPCS)
        {
            List<DadosEpi> mov = new List<DadosEpi>();
            string[] lines = listaEPCS.Split('|');
            string html = "";
            int count = 0;
            string query = "";
            int crachaQTD = 0;
            foreach (var epc in lines)
            {
                if (count == 0)
                {
                    html += "'" + epc + "'";
                    count++;
                }
                else
                {
                    html += ",'" + epc + "'";
                }

                var cracha = dbo.L_ATRIBUICAOCRACHA.Where(x => x.CODIGO_CRACHA == epc).ToList();
                if (cracha != null)
                {
                    if (cracha.Count > 0)
                    {
                        int fks = Convert.ToInt32(cracha[0].FK_FUNCIONARIO.ToString());
                        var nome = dbo.L_FUNCIONARIOS.Where(x => x.ID == fks).ToList();
                        if (nome.Count > 0)
                        {
                            if (crachaQTD == 0)
                            {
                                crachaQTD++;
                                mov.Add(new DadosEpi { CodigoProduto = "Matricula=" + cracha[0].MATRICULA, Produto = "Funcionario=" + nome[0].NOME + " " + nome[0].SOBRENOME, Quantidade = 1, CodigoFornecedor = "", Epc = cracha[0].CODIGO_CRACHA });
                            }
                            else
                            {
                                mov.Add(new DadosEpi { CodigoProduto = "Seleciona Apenas 1 Cracha", Produto = "", Quantidade = 0, CodigoFornecedor = "", Epc = "" });
                                return mov;
                            }
                        }
                    }
                }
            }


            query += "SELECT COD_PRODUTO,PRODUTO,COUNT(*) AS QTD,COD_FORNECEDOR " +
            "FROM L_PRODUTOS_ITENS " +
            "WHERE EPC IN(" + html + ") GROUP BY COD_PRODUTO,PRODUTO,COD_FORNECEDOR";
            var result = dbo.Database.SqlQuery<PRODUTOSITENS>(query).ToList();
            if (result != null)
            {
                foreach (var epcs in result)
                {
                    mov.Add(new DadosEpi { CodigoProduto = epcs.COD_PRODUTO, Produto = epcs.PRODUTO, Quantidade = epcs.QTD, CodigoFornecedor = epcs.COD_FORNECEDOR, Epc = "" });

                }
            }
            else
            {
                mov.Add(new DadosEpi { CodigoProduto = "0", Produto = "", Quantidade = 0, CodigoFornecedor = "", Epc = "" });
            }





            return mov;
        }

        [WebMethod]
        public void restListaAssinatura(int? FK_FUNCIONARIO, string documento)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                if (client == null)
                {
                    var lfunc = dbo.L_FUNCIONARIOS.Where(x => x.ID == FK_FUNCIONARIO).ToList();
                    string telefone = lfunc[0].TELEFONE;
                    List<Signer> sgn = new List<Signer>();
                    sgn.Add(new Signer { act = "sign", email = lfunc[0].EMAIL, allow_method = "sms", phone_number = formatarTelefone(lfunc[0].TELEFONE) });
                    BdAssinatura SG = new BdAssinatura { message = "Ficha para Assinatura", skip_email = true, signers = sgn };
                    var jsonFuncionamento = Newtonsoft.Json.JsonConvert.SerializeObject(SG);
                    var stringContent = new StringContent(jsonFuncionamento, UnicodeEncoding.UTF8, "application/json");
                    client = new HttpClient();
                    client.BaseAddress = new Uri("https://api.clicksign.com/");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.PostAsJsonAsync<BdAssinatura>("/v1/documents/" + documento + "/list?access_token=de3861faf07687e8afefeaf8ad9bb466", SG).Result;
                    //PostAsJsonAsync("/v1/documents/673c8736-6299-4f78-a5c8-6b31516d863c/list?access_token=6e7717ba405a88754d3943b558ce2a1c", SG);
                    //PostAsJsonAsync<Signer>("/v1/documents/673c8736-6299-4f78-a5c8-6b31516d863c/list?access_token=6e7717ba405a88754d3943b558ce2a1c", SG);

                }

            }
            catch (Exception er)
            {
                throw new Exception(er.Message.ToString());
            }
        }

        [WebMethod]
        public string formatarTelefone(string telefone)
        {
            try
            {
                return StdTools.Unformatted(telefone);
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public Clicksign.Document retornarDocumento(string key)
        {
            Clicksign.Clicksign cl = new Clicksign.Clicksign();
            var r = cl.Get(key);
            return r;
        }

        [WebMethod]
        public void updateStatusDocumento(string key, string status)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                L_DOCUMENTO_ASSINATURA lda = dbo.L_DOCUMENTO_ASSINATURA.First(x => x.CHAVE == key);
                lda.STATUS = status;
                dbo.SaveChanges();
            }
            catch
            {

            }
        }

        [WebMethod]
        public List<DADOS_ASSINATURA> retornaDocMatricula(string matricula)
        {
            try
            {
                DADOS_ASSINATURA da = new DADOS_ASSINATURA();
                List<DADOS_ASSINATURA> lda = new List<DADOS_ASSINATURA>();
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var lida = dbo.L_DOCUMENTO_ASSINATURA.Where(x => x.MATRICULA == matricula).ToList();
                var func = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                da.MATRICULA = lda[0].MATRICULA;
                da.EMAIL = func[0].EMAIL;
                da.TELEFONE = func[0].TELEFONE;
                da.CHAVE = lda[0].CHAVE;
                da.NOME = func[0].NOME + " " + func[0].SOBRENOME;
                foreach (var l in lida)
                {
                    lda.Add(new DADOS_ASSINATURA
                    {
                        MATRICULA = lda[0].MATRICULA,
                        EMAIL = func[0].EMAIL,
                        TELEFONE = func[0].TELEFONE,
                        CHAVE = lda[0].CHAVE,
                        NOME = func[0].NOME + " " + func[0].SOBRENOME
                    });
                }
                return lda;
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> envioParaHigienizacao(string listaEPCS, string estoque)
        {
            try
            {
                string[] lines = listaEPCS.Split('|');
                L_ESTOQUE le = new L_ESTOQUE();
                L_ENVIO_PARA_HIGIENIZACAO lteste = new L_ENVIO_PARA_HIGIENIZACAO();
                var gdi = Guid.NewGuid();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                List<L_ATRIBUICAOCRACHA> cracha = new List<L_ATRIBUICAOCRACHA>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                string codEstoque = "";
                string nomEstoque = "";
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {

                        var pti = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                        if (pti != null)
                        {
                            if (pti.Count > 0)
                            {
                                foreach (var itens in pti)
                                {
                                    try
                                    {
                                        var lEst = dbo.L_ESTOQUE.First(x => x.EPC == epc);
                                        if (lEst.STATUS != "T" && lEst.STATUS != "M" && lEst.STATUS != "Z")
                                        {
                                            var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == estoque).ToList();
                                            if (localEstoque != null)
                                            {
                                                if (localEstoque.Count > 0)
                                                {

                                                    codEstoque = localEstoque[0].CODIGO;
                                                    nomEstoque = localEstoque[0].NOME;
                                                }
                                                else
                                                {
                                                    codEstoque = "0";
                                                    nomEstoque = "Enviado para Higienização";
                                                }

                                            }
                                            else
                                            {
                                                codEstoque = "0";
                                                nomEstoque = "Enviado para Higienização";
                                            }
                                            if (lEst.ENTRADA_SAIDA == "E")
                                            {
                                                lEst.ENTRADA_SAIDA = "S";
                                                lEst.DATA_SAIDA = DateTime.Now.ToShortTimeString();
                                            }
                                            else
                                            {
                                                lEst.ENTRADA_SAIDA = "E";
                                                lEst.DATA_ENTRADA = DateTime.Now;
                                            }

                                            lEst.DATA_SAIDA = DateTime.Now.ToShortDateString();
                                            lEst.FK_FUNCIONARIO_SAIDA = null;
                                            lEst.MATRICULA = null;
                                            lEst.COD_ESTOQUE = codEstoque;
                                            lEst.ESTOQUE = nomEstoque;
                                            lEst.STATUS = "H";
                                            lEst.DESC_STATUS = "Enviado para Higienização";
                                            dbo.SaveChanges();

                                            lme.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lme.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                                            lme.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lme.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lme.EPC = lEst.EPC;
                                            lme.ESTOQUE = lEst.ESTOQUE;
                                            lme.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lme.QUANTIDADE = lEst.QUANTIDADE;
                                            lme.STATUS = "H";
                                            lme.DESC_STATUS = "Enviado para Higienização";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                            dbo.SaveChanges();

                                            lteste.DATA = DateTime.Now;
                                            lteste.EPC = lme.EPC;
                                            lteste.FK_MOVIMENTACAO = lme.ID;
                                            lteste.TESTE = "S";
                                            dbo.L_ENVIO_PARA_HIGIENIZACAO.Add(lteste);
                                            dbo.SaveChanges();

                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Enviado para Higienização";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = lEst.DESC_STATUS;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ff7f7f";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                    }
                                    catch
                                    {
                                        if (DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days < 0)
                                        {

                                            var localEstoque = dbo.L_LOCALESTOQUE.Where(x => x.CODIGO == estoque).ToList();
                                            if (localEstoque != null)
                                            {
                                                if (localEstoque.Count > 0)
                                                {

                                                    codEstoque = localEstoque[0].CODIGO;
                                                    nomEstoque = localEstoque[0].NOME;
                                                }
                                                else
                                                {
                                                    codEstoque = "0";
                                                    nomEstoque = "Enviado para Higienização";
                                                }

                                            }
                                            else
                                            {
                                                codEstoque = "0";
                                                nomEstoque = "Enviado para Higienização";
                                            }

                                            L_ESTOQUE lEst = new L_ESTOQUE();
                                            lEst.COD_PRODUTO = itens.COD_PRODUTO;
                                            lEst.DESC_PRODUTO = itens.PRODUTO;
                                            lEst.EPC = itens.EPC;
                                            lEst.COD_RECEBIMENTO = Guid.NewGuid();
                                            lEst.FK_PRODUTO = dbo.L_PRODUTOS.Where(x => x.COD_PRODUTO == itens.COD_PRODUTO).ToList()[0].ID;
                                            lEst.QUANTIDADE = 1;
                                            lEst.FK_FUNCIONARIO_SAIDA = null;
                                            lEst.MATRICULA = null;
                                            lEst.COD_ESTOQUE = codEstoque;
                                            lEst.ESTOQUE = nomEstoque;
                                            lEst.STATUS = "H";
                                            lEst.DESC_STATUS = "Enviado para Higienização";
                                            lEst.ENTRADA_SAIDA = "S";
                                            lEst.DATA_SAIDA = DateTime.Now.ToShortDateString();
                                            dbo.L_ESTOQUE.Add(lEst);
                                            dbo.SaveChanges();

                                            L_MOVIMENTACAO_ESTOQUE lmes = new L_MOVIMENTACAO_ESTOQUE();
                                            lmes.ENTRADA_SAIDA = "S";
                                            lmes.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lmes.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lmes.DATA_MOVIMENTACAO = DateTime.Now;
                                            lmes.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lmes.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lmes.EPC = lEst.EPC;
                                            lmes.ESTOQUE = lEst.ESTOQUE;
                                            lmes.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lmes.QUANTIDADE = lEst.QUANTIDADE;
                                            lmes.STATUS = "H";
                                            lmes.DESC_STATUS = "Enviado para Higienização";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lmes);
                                            dbo.SaveChanges();

                                            L_ENVIO_PARA_HIGIENIZACAO ltst = new L_ENVIO_PARA_HIGIENIZACAO();
                                            ltst.DATA = DateTime.Now;
                                            ltst.EPC = lEst.EPC;
                                            ltst.FK_MOVIMENTACAO = lmes.ID;
                                            ltst.TESTE = "S";
                                            dbo.L_ENVIO_PARA_HIGIENIZACAO.Add(ltst);
                                            dbo.SaveChanges();


                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Enviado para Higienização";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });


                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Data de Validade Vencida";
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }


                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não existe em nossa Base de dados";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });

                        }

                    }
                }

                return mov;
            }
            catch (Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> recebimentoHigienizacao(string listaEPCS)
        {
            try
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                L_MOVIMENTACAO_ESTOQUE lme = new L_MOVIMENTACAO_ESTOQUE();
                L_ENVIO_PARA_HIGIENIZACAO lteste = new L_ENVIO_PARA_HIGIENIZACAO();
                string[] lines = listaEPCS.Split('|');
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {
                        var estoque = dbo.L_ESTOQUE.Where(x => x.EPC == epc && x.STATUS == "H").ToList();
                        if (estoque != null)
                        {
                            if (estoque.Count > 0)
                            {
                                if (estoque[0].STATUS == "H")
                                {
                                    var epcTeste = dbo.L_ENVIO_PARA_HIGIENIZACAO.Where(x => x.EPC == epc).OrderByDescending(x => x.DATA).ToList();
                                    if (epcTeste.Count > 0)
                                    {

                                        var item = epcTeste[0];

                                        if (item.TESTE == "S")
                                        {
                                            var lEst = dbo.L_ESTOQUE.First(x => x.EPC == epc);


                                            lEst.ENTRADA_SAIDA = "E";
                                            lEst.DATA_ENTRADA = DateTime.Now;



                                            //lEst.ENTRADA_SAIDA = "E";
                                            //lEst.DATA_ENTRADA = DateTime.Now;
                                            lEst.FK_FUNCIONARIO_SAIDA = lEst.FK_FUNCIONARIO_SAIDA;
                                            lEst.MATRICULA = lEst.MATRICULA;
                                            lEst.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lEst.ESTOQUE = lEst.ESTOQUE;
                                            lEst.STATUS = "A";
                                            lEst.DESC_STATUS = "Recebimento de Itens Higienizado";
                                            dbo.SaveChanges();

                                            lme.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                            lme.COD_PRODUTO = lEst.COD_PRODUTO;
                                            lme.DATA_MOVIMENTACAO = DateTime.Now;
                                            lme.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                            lme.ENTRADA_SAIDA = lEst.ENTRADA_SAIDA;
                                            lme.EPC = lEst.EPC;
                                            lme.ESTOQUE = lEst.ESTOQUE;
                                            lme.FK_PRODUTO = lEst.FK_PRODUTO;
                                            lme.QUANTIDADE = lEst.QUANTIDADE;
                                            lme.STATUS = "A";
                                            lme.DESC_STATUS = "Recebimento de Itens Higienizado";
                                            lme.LAT = "-23.5705321";
                                            lme.LONG = "-46.7064147";
                                            //lme.COD_DISTRIBUICAO = gdi;
                                            dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                            dbo.SaveChanges();

                                            L_ENVIO_PARA_HIGIENIZACAO ltest = dbo.L_ENVIO_PARA_HIGIENIZACAO.First(x => x.ID == item.ID);
                                            ltest.TESTE = "N";
                                            ltest.DATA_RETORNO = DateTime.Now;
                                            dbo.SaveChanges();


                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = "Item Recebido com Sucesso";
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.CorAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Este item não esta em Higienização";
                                            mv.EPC = epc;
                                            mv.CorAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                        }

                                    }
                                    else
                                    {
                                        mv.Produto = "";
                                        mv.Resultado = "Este item não esta em Higienização";
                                        mv.EPC = epc;
                                        mv.CorAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não esta em Higienização";
                                    mv.EPC = epc;
                                    mv.CorAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não esta em Higienização";
                                mv.EPC = epc;
                                mv.CorAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não esta em Higienização";
                            mv.EPC = epc;
                            mv.CorAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                        }
                    }
                }

                return mov;

            }
            catch
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = "Erro";
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.CorAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, CorAviso = mv.CorAviso });
                return mov;
            }
        }

        [WebMethod]
        public string testeIndexOF(string nomeProduto)
        {
            foreach (var i in UsuarioLogado.categoriaInspecao)
            {
                var UPP = i.ToUpper();
                if (nomeProduto.IndexOf(UPP) != -1)
                {
                    return "ACHEI";
                }
            }

            return "NAO";
        }


    }

    public class UsuarioLogado
    {
        public static string[] categoriaInspecao = new string[] { "Cinto Paraquedista", "Talabarte", "Talabarte de Posicionamento",
        "Fita de Ancoragem",
        "Trava Quedas",
        "Conectores",
        "Cordas"};
    }

    public class Signer
    {
        public string email { get; set; }
        public string act { get; set; }
        public string allow_method { get; set; }
        public string phone_number { get; set; }
        public string display_name { get; set; }
        public string documentation { get; set; }
        public string birthday { get; set; }
    }

    public class BdAssinatura
    {
        public List<Signer> signers { get; set; }
        public string message { get; set; }
        public bool skip_email { get; set; }
    }
    public class Signature
    {
        public string display_name { get; set; }
        public object title { get; set; }
        public object company_name { get; set; }
        public string documentation { get; set; }
        public string key { get; set; }
        public string act { get; set; }
        public string decision { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public DateTime signed_at { get; set; }
        public string allow_method { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object skip_documentation { get; set; }
        public string phone_number { get; set; }
    }

    public class List
    {
        public DateTime started_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string user_key { get; set; }
        public List<Signature> signatures { get; set; }
    }

    public class Document
    {
        public string key { get; set; }
        public string original_name { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string user_key { get; set; }
        public List list { get; set; }
    }

    public class RootObject
    {
        public Document document { get; set; }
    }



}
