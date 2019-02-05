using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Services;
using Xceed.Words.NET;
using System.Net.Http.Headers;
using System.Text;

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
    public class Client : System.Web.Services.WebService
    {
        webservicetwos3Entities dbo = new webservicetwos3Entities();
        HttpClient client;

        [WebMethod]
        public List<L_LOCALESTOQUE> retornaLocalEstoque()
        {
            try
            {

                return dbo.L_LOCALESTOQUE.ToList();
                //return pr.retornaLocalEstoque().ToList();
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
            foreach(var epc in lines)
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
                        mv.corAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                            mv.corAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                        var pti = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                        var existe = dbo.L_ESTOQUE.Where(x => x.EPC == epc).ToList();
                        if (existe.Count == 0)
                        {
                            if (pti != null)
                            {
                                if (pti.Count > 0)
                                {
                                    foreach (var itens in pti)
                                    {
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
                                            mv.corAviso = "#ffffff";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = itens.PRODUTO;
                                            mv.Resultado = resultado;
                                            mv.EPC = epc;
                                            mv.corAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não existe em nossa Base de dados";
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                                }
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item ja foi recebido";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }

        }


        [WebMethod]
        public List<RESULTADOMOV> recebimentoEstoqueCnpj(string listaEPCS, string cnpj)
        {
            try
            {
                var gdi = Guid.NewGuid();
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

                                if(cnpj != itens.CNPJ_DESTINATARIO)
                                {
                                    erro = true;
                                    resultado = resultado + "\nEste item esta vinculado para outra Empresa";
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
                                    mv.corAviso = "#ffffff";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                }
                                else
                                {
                                    mv.Produto = itens.PRODUTO;
                                    mv.Resultado = resultado;
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                }



                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item ja foi recebido";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }

        }

        [WebMethod]
        public List<RESULTADOMOV> atribuicaoCracha(string matricula, string cracha)
        {
            try
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                var listaFuncionarios = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                L_ATRIBUICAOCRACHA lac = new L_ATRIBUICAOCRACHA();
                var crac = dbo.L_ATRIBUICAOCRACHA.Where(x => x.CODIGO_CRACHA == cracha).ToList();
                if (crac.Count > 0)
                {
                    List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                    mv.DataMovimentacao = DateTime.Now;
                    mv.EPC = cracha;
                    mv.Resultado = "Este Cracha já esta atribuido!";
                    mv.corAviso = "#ffffff";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }

                if (listaFuncionarios.Count > 0)
                {
                    lac.ATIVO = "S";
                    lac.CODIGO_CRACHA = cracha;
                    if (listaFuncionarios.Count > 1)
                    {
                        // BUG FIX PLACEHOLDER
                        // (Crachá Multiempresas: 2 funcionarios com mesma matricula em empresas diferentes, aqui refere-se sempre ao primeiro, independentemente da empresa)
                        // precisa-se que seja atualizado o cracha apenas do Funcionário da empresa atualmente logada
                        // como o método de login foi alterado desde essa versão extremamente desatualizada do código, é melhor não tentar fazer grandes alterações nessa parte
                        // esse if-else é de minha autoria
                        // (Rodolfo Cortese)
                        lac.FK_FUNCIONARIO = listaFuncionarios[0].ID;
                        lac.MATRICULA = listaFuncionarios[0].MATRICULA;
                    }
                    else
                    {
                        lac.FK_FUNCIONARIO = listaFuncionarios[0].ID;
                        lac.MATRICULA = listaFuncionarios[0].MATRICULA;
                    }
                    lac.DATA_ATRIBUICAO = DateTime.Now;
                    dbo.L_ATRIBUICAOCRACHA.Add(lac);
                    dbo.SaveChanges();

                    List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                    mv.DataMovimentacao = DateTime.Now;
                    mv.EPC = cracha;
                    mv.Produto = listaFuncionarios[0].MATRICULA + " - " + listaFuncionarios[0].NOME;
                    mv.Resultado = "Atribuido com Sucesso!";
                    mv.corAviso = "#ffffff";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;

                }
                else
                {
                    List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                    mv.DataMovimentacao = DateTime.Now;
                    mv.EPC = cracha;
                    mv.Resultado = "Não existe esta Matricula em nossa base de dados";
                    mv.corAviso = "#ffffff";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }
            }
            catch {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.DataMovimentacao = DateTime.Now;
                mv.EPC = cracha;
                mv.Resultado = "Erro";
                mv.corAviso = "#ffffff";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<RESULTADOMOV> distribuicaoEPI(string listaEPCS, string matricula)
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
                string matric = String.Empty;
                bool enviarArquivo = false;
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
                                matric = cracha[0].MATRICULA;
                                existeCracha = true;
                            }
                        }
                    }
                }

                if (codCracha != "")
                {
                    //if (matricula != matric)
                    //{
                    //    mv.Produto = "";
                    //    mv.Resultado = "O cracha deve ter a mesma Matricula do Login";
                    //    mv.EPC = "Matricula do Cracha:" + codCracha + " Matricula do Login:" + matric;
                    //    mv.corAviso = "#ff7f7f";
                    //    mv.DataMovimentacao = DateTime.Now;
                    //    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    //    return mov;
                    //}

                    foreach (var epc in lines)
                    {
                        enviarArquivo = false;
                        if (epc != "")
                        {
                            if (epc != codCracha)
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

                                                var dtValidadeVencida = DateTime.Now.Subtract(itens.DT_VALIDADE.Value).Days;
                                                var dtValTesteVencida = DateTime.Now.Subtract(itens.VALIDADE_TESTE.Value).Days;

                                                if (dtValidadeVencida < 0)
                                                {

                                                    if (dtValTesteVencida < 0)
                                                    {

                                                        var lEst = dbo.L_ESTOQUE.First(x => x.EPC == epc);
                                                        if (lEst.STATUS == "R" || lEst.STATUS == "B" || lEst.STATUS == "A" || lEst.STATUS == "O" || lEst.STATUS == "E")
                                                        {
                                                            if (lEst.ENTRADA_SAIDA != "S"  || lEst.STATUS == "O")
                                                            {
                                                                lEst.ENTRADA_SAIDA = "S";
                                                                lEst.DATA_SAIDA = DateTime.Now.ToLongTimeString();
                                                                lEst.FK_FUNCIONARIO_SAIDA = cracha[0].FK_FUNCIONARIO;
                                                                lEst.MATRICULA = cracha[0].MATRICULA;
                                                                lEst.STATUS = "O";
                                                                lEst.COD_RECEBIMENTO = gdi;
                                                                lEst.DESC_STATUS = "Aguardando Assinatura";
                                                                dbo.SaveChanges();

                                                                lme.COD_ESTOQUE = lEst.COD_ESTOQUE;
                                                                lme.COD_PRODUTO = lEst.COD_PRODUTO;
                                                                lme.DATA_MOVIMENTACAO = DateTime.Now;
                                                                lme.DESC_PRODUTO = lEst.DESC_PRODUTO;
                                                                lme.ENTRADA_SAIDA = "S";
                                                                lme.EPC = lEst.EPC;
                                                                lme.ESTOQUE = lEst.ESTOQUE;
                                                                lme.FK_PRODUTO = lEst.FK_PRODUTO;
                                                                lme.QUANTIDADE = lEst.QUANTIDADE;
                                                                lme.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                                                lme.FK_FUNCIONARIO = cracha[0].FK_FUNCIONARIO;
                                                                lme.COD_DISTRIBUICAO = gdi;
                                                                lme.STATUS = "O";
                                                                lme.DESC_STATUS = "Aguardando Assinatura";
                                                                dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                                                dbo.SaveChanges();

                                                                mv.Produto = lEst.DESC_PRODUTO + " - Funcionario=" + lEst.MATRICULA;
                                                                mv.Resultado = "Aguardando Assinatura";
                                                                mv.EPC = epc;
                                                                mv.DataMovimentacao = DateTime.Now;
                                                                mv.corAviso = "#ffffff";
                                                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                                                                L_FICHACADASTRAL lfc = new L_FICHACADASTRAL();
                                                                lfc.DATA = DateTime.Now;
                                                                lfc.FUNCIONARIO = cracha[0].MATRICULA;
                                                                lfc.MATRICULA = cracha[0].MATRICULA;
                                                                lfc.IMPRESSO = "N";
                                                                dbo.L_FICHACADASTRAL.Add(lfc);
                                                                dbo.SaveChanges();

                                                                

                                                            }
                                                            else
                                                            {
                                                                mv.Produto = lEst.DESC_PRODUTO + " - Funcionario=" + lEst.MATRICULA;
                                                                mv.Resultado = "Este Item já esta Atribuido!";
                                                                mv.EPC = epc;
                                                                mv.DataMovimentacao = DateTime.Now;
                                                                mv.corAviso = "#ff7f7f";
                                                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            mv.Produto = mv.Produto;
                                                            if (lEst.DESC_STATUS != "Distribuição de EPI")
                                                            {
                                                                mv.Resultado = lEst.DESC_STATUS;
                                                            }
                                                            else
                                                            {
                                                                mv.Resultado = "Este Item já esta Atribuido!";
                                                            }
                                                            mv.EPC = epc;
                                                            mv.corAviso = "#ff7f7f";
                                                            mv.DataMovimentacao = DateTime.Now;
                                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        mv.Produto = mv.Produto;
                                                        mv.Resultado = "Data de Teste Vencida";
                                                        mv.EPC = epc;
                                                        mv.corAviso = "#ff7f7f";
                                                        mv.DataMovimentacao = DateTime.Now;
                                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                                    }
                                                }
                                                else
                                                {
                                                    mv.Produto = mv.Produto;
                                                    mv.Resultado = "Data de Validade Vencida";
                                                    mv.EPC = epc;
                                                    mv.corAviso = "#ff7f7f";
                                                    mv.DataMovimentacao = DateTime.Now;
                                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                                }
                                            }
                                            catch
                                            {
                                                le.COD_PRODUTO = itens.COD_PRODUTO;
                                                le.COD_RECEBIMENTO = gdi;
                                                le.DATA_ENTRADA = DateTime.Now;
                                                le.DESC_PRODUTO = itens.PRODUTO;
                                                le.ENTRADA_SAIDA = "S";
                                                le.EPC = itens.EPC;
                                                le.QUANTIDADE = 1;
                                                le.MATRICULA = cracha[0].MATRICULA;
                                                le.FK_FUNCIONARIO_SAIDA = cracha[0].FK_FUNCIONARIO;
                                                le.DATA_SAIDA = DateTime.Now.ToLongDateString();
                                                le.FK_PRODUTO = dbo.L_PRODUTOS.Where(x => x.COD_PRODUTO == itens.COD_PRODUTO).ToList()[0].ID;
                                                le.STATUS = "O";
                                                le.DESC_STATUS = "Aguardando Assinatura";
                                                dbo.L_ESTOQUE.Add(le);
                                                dbo.SaveChanges();


                                                lme.COD_ESTOQUE = le.COD_ESTOQUE;
                                                lme.COD_PRODUTO = le.COD_PRODUTO;
                                                lme.DATA_MOVIMENTACAO = DateTime.Now;
                                                lme.DESC_PRODUTO = le.DESC_PRODUTO;
                                                lme.ENTRADA_SAIDA = "S";
                                                lme.EPC = le.EPC;
                                                lme.ESTOQUE = le.ESTOQUE;
                                                lme.FK_PRODUTO = le.FK_PRODUTO;
                                                lme.QUANTIDADE = le.QUANTIDADE;
                                                lme.COD_FUNCIONARIO = cracha[0].MATRICULA;
                                                lme.FK_FUNCIONARIO = cracha[0].FK_FUNCIONARIO;
                                                lme.COD_DISTRIBUICAO = gdi;
                                                lme.STATUS = "O";
                                                lme.DESC_STATUS = "Aguardando Assinatura";
                                                dbo.L_MOVIMENTACAO_ESTOQUE.Add(lme);
                                                dbo.SaveChanges();

                                                mv.Produto = le.DESC_PRODUTO + " - Funcionario=" + le.MATRICULA;
                                                mv.Resultado = "Aguardando Assinatura";
                                                mv.EPC = epc;
                                                mv.DataMovimentacao = DateTime.Now;
                                                mv.corAviso = "#ffffff";
                                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                                                L_FICHACADASTRAL lfc = new L_FICHACADASTRAL();
                                                lfc.DATA = DateTime.Now;
                                                lfc.FUNCIONARIO = cracha[0].MATRICULA;
                                                lfc.MATRICULA = cracha[0].MATRICULA;
                                                lfc.IMPRESSO = "N";
                                                dbo.L_FICHACADASTRAL.Add(lfc);
                                                dbo.SaveChanges();


                                            }



                                        }

                                        
                                    }
                                    else
                                    {
                                        mv.Produto = "";
                                        mv.Resultado = "Este item não existe em nossa Base de dados";
                                        mv.EPC = epc;
                                        mv.corAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        return mov;
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não existe em nossa Base de dados";
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                    return mov;
                                }
                            }
                        }
                    }

                }
                else
                {
                    mv.Produto = "";
                    mv.DataMovimentacao = DateTime.Now;
                    mv.EPC = "";
                    mv.Resultado = "Não Foi Encontrado Nenhum Codigo de Cracha para realizar a Distribuição";
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }

                if (lme.COD_DISTRIBUICAO != null)
                {
                    if (lme.COD_DISTRIBUICAO.ToString() != "")
                    {
                        var docKey = convertHtmlDocx(cracha[0].MATRICULA, gdi.ToString());
                        string matriculas = cracha[0].MATRICULA;
                        var lfunc = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matriculas).ToList();
                        criarHookArquivo(docKey);
                        mv.Produto = "chave";
                        mv.Resultado = lfunc[0].TELEFONE + "|" + lfunc[0].EMAIL;
                        mv.EPC = docKey;
                        mv.corAviso = "#ffffff";
                        mv.DataMovimentacao = DateTime.Now;
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    }
                }
                return mov;
            }
            catch(Exception er)
            {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }

            
        }

        private void criarHookArquivo(string docKey)
        {
            var clicksign = new Clicksign.Clicksign();
            var document = new Clicksign.Document { Key = docKey };
            var hook = clicksign.CreateHook(document, "http://easyepi.com.br/HookApi/api/Hook");
        }

        [WebMethod]
        public List<RESULTADOMOV> envioParaTeste(string listaEPCS, string estoque)
        {
            try
            {
                string[] lines = listaEPCS.Split('|');
                string codCracha = "";
                L_ESTOQUE le = new L_ESTOQUE();
                L_ENVIO_PARA_TESTE lteste = new L_ENVIO_PARA_TESTE();
                var gdi = Guid.NewGuid();
                bool existeCracha = false;
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = lEst.DESC_STATUS;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.corAviso = "#ff7f7f";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });


                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Data de Validade Vencida";
                                            mv.EPC = epc;
                                            mv.corAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }


                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não existe em nossa Base de dados";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Este item não esta em Teste";
                                            mv.EPC = epc;
                                            mv.corAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }

                                    }
                                    else
                                    {
                                        mv.Produto = "";
                                        mv.Resultado = "Este item não esta em Teste";
                                        mv.EPC = epc;
                                        mv.corAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não esta em Teste";
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não esta em Teste";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não esta em Teste";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                    }
                }

                return mov;

            }
            catch {
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = "Erro";
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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

                                        if(statusEstoque!= null && statusEstoque.Count > 0)
                                        {

                                            if(statusEstoque[0].STATUS == "B")
                                            {

                                                erro = true;
                                                mensagem = mensagem + "EPI não atribuido para um funcionario, foi devolvido ao Estoque;";

                                                #region
                                                //mv.Produto = "";
                                                //mv.Resultado = "EPI não atribuido para um funcionario, foi devolvido ao Estoque";
                                                //mv.EPC = epc;
                                                //mv.corAviso = "#ff7f7f";
                                                //mv.DataMovimentacao = DateTime.Now;
                                                //mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                                                //mv.Produto = "";
                                                //mv.Resultado = "EPI não atribuido para um funcionario, aguardando assinatura digital";
                                                //mv.EPC = epc;
                                                //mv.corAviso = "#ff7f7f";
                                                //mv.DataMovimentacao = DateTime.Now;
                                                //mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                                                //mv.Produto = "";
                                                //mv.Resultado = "EPI não atribuido para um funcionario, " + statusEstoque[0].DESC_STATUS;
                                                //mv.EPC = epc;
                                                //mv.corAviso = "#ff7f7f";
                                                //mv.DataMovimentacao = DateTime.Now;
                                                //mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                                                mv.corAviso = "#ffffff";
                                            }
                                            else
                                            {
                                                mv.corAviso = "#ff7f7f";
                                            }

                                            mv.Produto = itens.PRODUTO + " - Funcionario=" + cracha[0].MATRICULA;
                                            mv.Resultado = mensagem;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;

                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }



                                    }


                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este EPI não existe em nossa Base de dados";
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    mv.corAviso = "#ff7f7f";
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                        mv.corAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                        mv.corAviso = "#ffffff";
                        mv.DataMovimentacao = DateTime.Now;
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                        mv.corAviso = "#ffffff";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                            mv.corAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = "";
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;

                }
                else
                {
                    mv.Produto = "";
                    mv.Resultado = "Este cracha não esta atribuido";
                    mv.EPC = "";
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }
            }
            catch(Exception er) {

                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                mv.Produto = "";
                mv.Resultado = er.Message.ToString();
                mv.EPC = "";
                mv.DataMovimentacao = DateTime.Now;
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }
        }

        [WebMethod]
        public bool FuncionarioTemSenha(string listaEPCS)
        {
            webservicetwos3Entities dbo = new webservicetwos3Entities();
            string[] lines = listaEPCS.Split('|');
            foreach(var epc in lines)
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
                    mv.corAviso = "#ffffff";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;

                }
                else
                {
                    mv.Produto = "";
                    mv.Resultado = "Este cracha não esta atribuido";
                    mv.EPC = "";
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                return mov;
            }
        }

        [WebMethod]
        public List<DADOSLOGIN> loginFuncionario(string matricula, string senha)
        {
            try
            {
                DADOSLOGIN mv = new DADOSLOGIN();
                List<DADOSLOGIN> mov = new List<DADOSLOGIN>();
                var login = dbo.L_LOGIN.Where(x => x.USUARIO == matricula && x.SENHA == senha).ToList();
                if (login.Count > 0)
                {
                    int fkFuncionario = Convert.ToInt32(login[0].FK_USUARIO.ToString());
                    var dadosFuncionarios = dbo.L_USUARIO_COALBORADOR.Where(x => x.id == fkFuncionario).ToList();
                    int fkCliente = Convert.ToInt32(dadosFuncionarios[0].FK_CLIENTE.ToString());
                    var dadosEmpresa = dbo.L_CLIENTE.Where(x => x.ID == fkCliente).ToList();

                    mv.Produto = "1";
                    mv.Resultado = "OK";
                    mv.EPC = matricula;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ffffff";
                    mv.Empresa = dadosEmpresa[0].NOME;
                    mv.Nome = dadosFuncionarios[0].nome;
                    mv.FkCliente = Convert.ToInt32(dadosFuncionarios[0].FK_CLIENTE.ToString());
                    mv.Cnpj = dadosFuncionarios[0].cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
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
                    return mov;
                }
                else
                {
                    mv.Produto = "2";
                    mv.Resultado = "Matricula ou Senha Incorreta";
                    mv.EPC = matricula;
                    mv.DataMovimentacao = DateTime.Now;
                    mv.corAviso = "#ff7f7f";
                    mov.Add(new DADOSLOGIN { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                    return mov;
                }
            }
            catch
            {
                DADOSLOGIN mv = new DADOSLOGIN();
                List<DADOSLOGIN> mov = new List<DADOSLOGIN>();
                mv.Produto = "3";
                mv.Resultado = "Verifique sua Conexão!";
                mv.EPC = matricula;
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

                        if (result[0].STATUS == "M" || result[0].STATUS =="D")
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
                            mv.corAviso = "#ffffff";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        else
                        {
                            mv.Resultado = result[0].DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result[0].DESC_PRODUTO;
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        
                    }
                    else
                    {
                        mv.Resultado = "Epi Não Existe no Estoque";
                        mv.EPC = epc;
                        mv.DataMovimentacao = DateTime.Now;
                        mv.Produto = "";
                        mv.corAviso = "#ff7f7f";
                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                            mv.corAviso = "#ffffff";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        else
                        {
                            mv.Resultado = result.DESC_STATUS;
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = result.DESC_PRODUTO;
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                mv.corAviso = "#ffffff";
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                            }
                            else
                            {
                                mv.Resultado = "Este item não existe na base de dados";
                                mv.EPC = epc;
                                mv.DataMovimentacao = DateTime.Now;
                                mv.Produto = "";
                                mv.corAviso = "#ff7f7f";
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                            }
                        }
                        else
                        {
                            mv.Resultado = "Este item não existe na base de dados";
                            mv.EPC = epc;
                            mv.DataMovimentacao = DateTime.Now;
                            mv.Produto = "";
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                        }
                        
                    }

                    
                }
            }

            return mov;
        }

        [WebMethod]
        public List<RESULTADOMOV> consultaEPI(string listaEPCS, string cnpj)
        {
            try
            {
                var gdi = Guid.NewGuid();
                L_ESTOQUE le = new L_ESTOQUE();
                RESULTADOMOV mv = new RESULTADOMOV();
                List<RESULTADOMOV> mov = new List<RESULTADOMOV>();
                string[] lines = listaEPCS.Split('|');
                string resultado = "";
                List<L_FUNCIONARIOS> dadosFuncionario = new List<L_FUNCIONARIOS>();
                bool erro = false;
                foreach (var epc in lines)
                {
                    if (epc != "")
                    {

                        var pte = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc && x.CNPJ_DESTINATARIO == cnpj).ToList();
                        
                        if (pte != null)
                        {
                            if (pte.Count > 0)
                            {
                                foreach (var itens in pte)
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

                                    mv.Produto = itens.PRODUTO;
                                    mv.Resultado = resultado;
                                    mv.EPC = epc;
                                    if (erro)
                                    {
                                        mv.corAviso = "#ff7f7f";
                                    }
                                    else
                                    {
                                        mv.corAviso = "#ffffff";
                                    }
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não existe em nossa Base de dados";
                            mv.EPC = "";
                            mv.DataMovimentacao = DateTime.Now;
                            mv.corAviso = "#ff7f7f";
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                        mv.corAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                    }
                                    else
                                    {

                                        mv.Produto = itens.DESC_PRODUTO;
                                        mv.Resultado = "Nenhum Usuario Atribuido a este EPI";
                                        mv.EPC = epc;
                                        mv.corAviso = "#ffffff";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
        public List<DADOSEPI> retornarDadosEpiValidar(string listaEPCS)
        {
            List<DADOSEPI> mov = new List<DADOSEPI>();
            string[] lines = listaEPCS.Split('|');
            foreach (var epc in lines)
            {
                var result = dbo.L_PRODUTOS_ITENS.Where(x => x.EPC == epc).ToList();
                if (result != null)
                {
                    if (result.Count > 0) {
                        mov.Add(new DADOSEPI { CodProduto = result[0].COD_PRODUTO, Produto = result[0].PRODUTO, Qtd = 1, CodFornecedor = result[0].COD_FORNECEDOR, EPC = result[0].EPC });
                    }
                    else
                    {
                        mov.Add(new DADOSEPI { CodProduto = "0", Produto = "", Qtd = 0, CodFornecedor = "", EPC = "" });
                    }
                }
                else
                {
                    mov.Add(new DADOSEPI { CodProduto = "0", Produto = "", Qtd = 0, CodFornecedor = "", EPC = "" });
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
                                mov.Add(new DADOSEPI { CodProduto = "Matricula=" + cracha[0].MATRICULA, Produto = "Funcionario=" + nome[0].NOME + " " + nome[0].SOBRENOME, Qtd = 1, CodFornecedor = "", EPC = cracha[0].CODIGO_CRACHA });
                            }
                        }
                    }
                

            }

            return mov;
        }

        /*
        [WebMethod]
        public bool salvarMensagemOcorrencia(string number, string mensagem)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                L_MENSAGEM_OCORRENCIA lmo = new L_MENSAGEM_OCORRENCIA();
                string[] latLong = new string[2];
                int idPai = 0;
                if(mensagem.IndexOf("Ocorrencias Numero") != -1)
                {
                    return true;
                }

                if (mensagem.IndexOf("Leal") != -1)
                {

                    var dt = DateTime.Now.ToShortDateString();
                    var retorno = dbo.Database.SqlQuery<L_MENSAGEM_OCORRENCIA>("SELECT * FROM L_MENSAGEM_OCORRENCIA " +
                      "WHERE CONVERT(DATE, DATA_MENSAGEM, 103) BETWEEN " +
                      "CONVERT(DATE, '" + dt + "', 103) AND CONVERT(DATE, '" + dt + "', 103) " +
                      "AND NUMERO = '" + number + "'").ToList();
                    if (retorno.Count > 0)
                    {
                        idPai = retorno.Min(x => x.ID);
                    }


                    lmo.DATA_MENSAGEM = DateTime.Now;
                    lmo.NUMERO = number;
                    lmo.MENSAGEM = mensagem;
                    lmo.OCORRENCIA = idPai.ToString();
                    dbo.L_MENSAGEM_OCORRENCIA.Add(lmo);
                    dbo.SaveChanges();

                    if(idPai == 0)
                    {
                        idPai = lmo.ID;
                    }

                    latLong = latitudeLongitudeMensagem(mensagem);
                    var status = statusOcorrencia(mensagem);

                    P_OCORRENCIA po = new P_OCORRENCIA();
                    po.DESCRICAO = status[1];
                    po.ENDERECO = "";
                    po.LATITUDE = latLong[0];
                    po.LONGITUDE = latLong[1];
                    po.ENDERECO = "";
                    po.ID_STATUSOCORRENCIA = Convert.ToInt32(status[0]);
                    po.ID_USUARIO = 1;
                    po.ID_PRODUTO = "1";
                    po.ID_MENSAGEM_OCORRENCIA = idPai;
                    dbo.P_OCORRENCIA.Add(po);
                    dbo.SaveChanges();

                    
                    try
                    {
                        P_HISTORICOATENDIMENTO pha = dbo.P_HISTORICOATENDIMENTO.First(x => x.ID_OCORRENCIA == idPai);
                        pha.DATA = lmo.DATA_MENSAGEM;
                        pha.DESCRICAO = "Aguardando Confirmação de Leitura";
                        pha.ID_OCORRENCIA = idPai;
                        pha.ID_USUARIO = 1;
                        pha.STATUS_MENSAGEM = Convert.ToInt32(status[0]);
                        pha.DESC_STATUS = status[1];
                        pha.HORA = TimeSpan.Parse(lmo.DATA_MENSAGEM.Value.ToShortTimeString());
                        dbo.SaveChanges();
                    }
                    catch
                    {
                        P_HISTORICOATENDIMENTO pha = new P_HISTORICOATENDIMENTO();
                        pha.DATA = lmo.DATA_MENSAGEM;
                        pha.DESCRICAO = "Aguardando Confirmação de Leitura";
                        pha.ID_OCORRENCIA = idPai;
                        pha.ID_USUARIO = 1;
                        pha.STATUS_MENSAGEM = Convert.ToInt32(status[0]);
                        pha.DESC_STATUS = status[1];
                        pha.HORA = TimeSpan.Parse(lmo.DATA_MENSAGEM.Value.ToShortTimeString());
                        dbo.P_HISTORICOATENDIMENTO.Add(pha);
                        dbo.SaveChanges();
                    }


                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
         */

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
                string[] arr2 = { "", ""};
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
                        mov.Add(new RESULTADOMOV { Resultado = mv.MENSAGEM, EPC = mv.NUMERO, DataMovimentacao = dt, Produto = mv.ID.ToString(), corAviso = "" });
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
                            corAviso = isNull(mv.OCORRENCIA)
                        });
                    }

                }

                return mov;
            }
            catch { return null; }
        }

        private string isNull(string oCORRENCIA)
        {
            if(oCORRENCIA== null)
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
        public List<DADOSEPI> retornarDadosEpiValidarRecebimento(string listaEPCS)
        {
            List<DADOSEPI> mov = new List<DADOSEPI>();
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
                                mov.Add(new DADOSEPI { CodProduto = "Matricula=" + cracha[0].MATRICULA, Produto = "Funcionario=" + nome[0].NOME + " " + nome[0].SOBRENOME, Qtd = 1, CodFornecedor = "", EPC = cracha[0].CODIGO_CRACHA });
                            }
                            else
                            {
                                mov.Add(new DADOSEPI { CodProduto = "Seleciona Apenas 1 Cracha", Produto = "", Qtd = 0, CodFornecedor = "", EPC = "" });
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
                    mov.Add(new DADOSEPI { CodProduto = epcs.COD_PRODUTO, Produto = epcs.PRODUTO, Qtd = epcs.QTD, CodFornecedor = epcs.COD_FORNECEDOR, EPC = "" });

                }
            }
            else
            {
                mov.Add(new DADOSEPI { CodProduto = "0", Produto = "", Qtd = 0, CodFornecedor = "", EPC = "" });
            }





            return mov;
        }

        [WebMethod]
        public static string retornarDadosFicha(string matricula)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var l = dbo.L_FICHACADASTRAL.Where(x => x.MATRICULA == matricula).ToList();
                if (l.Count > 0)
                {
                    var matri = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
                    return matri[0].NOME + " " + matri[0].SOBRENOME + "|" + l[0].MATRICULA + "|" + matri[0].FUNCAO + "|" + DateTime.Now.ToLongDateString() + "|" + matri[0].CNPJ + "|" + matri[0].FK_CLIENTE;
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
            }
        }

        [WebMethod]
        public List<RESULTADO> retornarLista(string matricula, Guid codDistribuicao)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                
                var result = dbo.L_MOVIMENTACAO_ESTOQUE.Where(x => x.COD_FUNCIONARIO == matricula && x.COD_DISTRIBUICAO == codDistribuicao).ToList();
                RESULTADO mv = new RESULTADO();
                List<RESULTADO> mov = new List<RESULTADO>();
                if (result.Count > 0)
                {
                    foreach (var rst in result)
                    {
                        mov.Add(new RESULTADO { DATA = rst.DATA_MOVIMENTACAO.Value.ToShortDateString(), Produto = rst.DESC_PRODUTO, EPC = rst.EPC });
                    }

                    return mov;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public string convertHtmlDocx(string matricula, string codDistribuicao)
        {
            try
            {
                #region Title Format
                //Formatting Title  
                Formatting titleFormat = new Formatting();
                //Specify font family  
                //titleFormat.FontFamily = new Xceed.Words.NET.Font("Batang");
                //Specify font size  
                titleFormat.Size = 18D;
                titleFormat.Position = 20;
                Color _color = System.Drawing.ColorTranslator.FromHtml("#4B718D");
                titleFormat.FontColor = _color;
                titleFormat.Bold = true;
                #endregion

                var dadosFicha = retornarDadosFicha(matricula);

                var nomeArquivo = matricula + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string path = Server.MapPath("Doc_FichaCadastral");
                string fileName = path + "\\" + nomeArquivo + ".docx";
                var doc = DocX.Create(fileName);
                
                doc.InsertParagraph("FICHA DE EPI'S:", false, titleFormat).Alignment = Alignment.center;
                titleFormat.Size = 14D;
                titleFormat.Position = 20;
                titleFormat.Bold = false;
                doc.InsertParagraph("LEAL EQUIPAMENTOS DE PROTEÇÃO", false, titleFormat).Alignment = Alignment.center;
                Color _color2 = System.Drawing.ColorTranslator.FromHtml("#000000");
                titleFormat.FontColor = _color2;
                titleFormat.Position = 0;
                doc.InsertParagraph("CONTROLE DE ENTREGA DE EPI'S\nEQUIPAMENTOS DE PROTEÇÃO INDIVIDUAL", false, titleFormat).Alignment = Alignment.center;
                doc.InsertParagraph("", false, titleFormat).Position(20);
                doc.InsertParagraph("Eu " + dadosFicha.Split('|')[0] + " Registro No "+ dadosFicha.Split('|')[1] + " Função "+ dadosFicha.Split('|')[2] + " declaro para  todos  os  efeitos  legais  que  recebi  da Leal Equipamentos de Proteção , os equipamentos de proteção individual (EPI) relacionados abaixo, bem como as instruções para sua correta utilização, obrigando-me:");

                doc.InsertParagraph("1) usar o EPI e uniforme indicado, apenas às finalidades a que se destina;");
                doc.InsertParagraph("2) comunicar o setor de obras /segurança do trabalho, qualquer alteração no EPI que o torne parcialmente ou totalmente danificado;");
                doc.InsertParagraph("3) responsabilizar-me pelos danos do EPI, quando usados de modo inadequado ou fora das atividades a que se destina, bem como pelo seu extravio;");
                doc.InsertParagraph("4) devolvê-lo quando da troca por outro ou no meu desligamento da empresa.").Position(20);

                int count = 0;
                var listaEpis = retornarLista(matricula, Guid.Parse(codDistribuicao));
                Table t = doc.AddTable((listaEpis.Count + 1), 7);
                t.Alignment = Alignment.center;



                //t.SetColumnWidth(1, 100);

                //Fill cells by adding text.  
                Formatting column = new Formatting();
                column.Bold = true;
                column.Size = 8d;
                
                t.Rows[count].Cells[0].Paragraphs[0].Append("Qtd.", column).Alignment = Alignment.center ;
                t.Rows[count].Cells[1].Paragraphs[0].Append("EPI'S", column).Alignment = Alignment.center;
                t.Rows[count].Cells[2].Paragraphs[0].Append("Data de Entrega", column).Alignment = Alignment.center;
                t.Rows[count].Cells[3].Paragraphs[0].Append("EPC", column).Alignment = Alignment.center;
                t.Rows[count].Cells[4].Paragraphs[0].Append("Data de Devolução", column).Alignment = Alignment.center;
                t.Rows[count].Cells[5].Paragraphs[0].Append("Assinatura de Devolução Colaborador", column).Alignment = Alignment.center;
                t.Rows[count].Cells[6].Paragraphs[0].Append("Assinatura de Devolução Responsável", column).Alignment = Alignment.center;
                //t.ColumnWidths[1] = 100;
                foreach (var epi in listaEpis)
                {
                    count++;
                    t.Rows[count].Cells[0].Paragraphs[0].Append("1").FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[1].Paragraphs[0].Append(epi.Produto).FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[2].Paragraphs[0].Append(epi.DATA).FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[3].Paragraphs[0].Append(epi.EPC).FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[4].Paragraphs[0].Append("").FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[5].Paragraphs[0].Append("").FontSize(8d).Alignment = Alignment.center;
                    t.Rows[count].Cells[6].Paragraphs[0].Append("").FontSize(8d).Alignment = Alignment.center;
                }
                
                //t.SetWidths(new float[] { 1, 1, 1, 1, 1, 1, 1 });
                doc.InsertTable(t);


                doc.InsertParagraph("", false, titleFormat).Position(20);
                doc.InsertParagraph("Declaro para todos os efeitos legais que recebi todos os Equipamentos de Proteção Individual constantes da lista acima, novos e em perfeitas condições de uso, e que estou ciente das obrigações descritas na NR 06, baixada pela Portaria MTB 3214 / 78, sub - item 6.7.1, a saber: ");
                doc.InsertParagraph("a) usar, utilizando-o apenas para a finalidade a que se destina;");
                doc.InsertParagraph("b) responsabilizar-se pela guarda e conservação; ");
                doc.InsertParagraph("c) comunicar ao empregador qualquer alteração que o torne impróprio para uso; e ");
                doc.InsertParagraph("d) cumprir as determinações do empregador sobre o uso adequado.");
                doc.InsertParagraph("Declaro, também, que estou ciente das disposições do Art. 462 e § 1º da CLT, e autorizo o desconto salarial proporcional ao custo de reparação do dano que os EPI’s aos meus cuidados venham apresentar.");
                doc.InsertParagraph("Declaro, ainda estar ciente de que o uso é obrigatório, sob pena de ser punido conforme Lei nº 6.514, de 27/12/77, artigo 158. ");
                doc.InsertParagraph("Declaro, ainda, que recebi treinamento referente ao uso do E.P.I. e as Normas de Segurança do Trabalho.");
                doc.InsertParagraph("", false, titleFormat).Position(20);
                titleFormat.FontColor = System.Drawing.Color.Red;
                titleFormat.Size = 11;
                titleFormat.Bold = false;
                doc.InsertParagraph("Data: " + dadosFicha.Split('|')[3], false, titleFormat).Position(10);
                doc.InsertParagraph("Local: _______________________________", false, titleFormat).Position(10);
                doc.InsertParagraph("ASSINATURA: _______________________________").Position(10);
                doc.Save();
                var docs = enviarDocumento(fileName, matricula);
                documentoAssinado(codDistribuicao,nomeArquivo, matricula, dadosFicha.Split('|')[4], dadosFicha.Split('|')[5], docs.Key);
                return docs.Key;
            }
            catch(Exception E)
            {
                throw new Exception(E.Message.ToString());
            }
        }

        [WebMethod]
        public void documentoAssinado(string codDistribuicao, string nomeArquivo, string matricula, string cnpj, string fkCliente, string chave)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                L_DOCUMENTO_ASSINATURA lda = new L_DOCUMENTO_ASSINATURA();
                lda.CLIENTE = fkCliente;
                lda.DATA_ENVIO = DateTime.Now;
                lda.DOWNLOAD = "N";
                lda.MATRICULA = matricula;
                lda.NOME_DOCUMENTO = nomeArquivo;
                lda.CHAVE = chave;
                lda.COD_DISTRIBUICAO = Guid.Parse(codDistribuicao);
                dbo.L_DOCUMENTO_ASSINATURA.Add(lda);
                dbo.SaveChanges();
               

            }
            catch
            {

            }
        }

        [WebMethod]
        public Clicksign.Document enviarDocumento(string path, string matricula)
        {
            var clicksign = new Clicksign.Clicksign();

            //Envio através do caminho do arquivo
            //string path = Server.MapPath("pdf");
            //var uri = path + "\\" + 1 + ".pdf";
            clicksign.Upload(path);
            var doc = clicksign.Document;
            restListaAssinatura(matricula, doc.Key);

            return clicksign.Document;
        }

        [WebMethod]
        public void restListaAssinatura(string matricula, string documento)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                if (client == null)
                {
                    var lfunc = dbo.L_FUNCIONARIOS.Where(x => x.MATRICULA == matricula).ToList();
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
        public string formatarTelefone(string tELEFONE)
        {
            try
            {
                //11 96678 - 704
                if (tELEFONE.IndexOf('-') != -1)
                {
                    tELEFONE = tELEFONE.Replace("-", "");
                    tELEFONE = tELEFONE.Replace(" ", "");
                    return tELEFONE;
                }
                else
                {
                    tELEFONE = tELEFONE.Replace(" ", "");
                    return tELEFONE;
                }
            }
            catch
            {
                return tELEFONE;
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
            catch {

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
                string codCracha = "";
                L_ESTOQUE le = new L_ESTOQUE();
                L_ENVIO_PARA_HIGIENIZACAO lteste = new L_ENVIO_PARA_HIGIENIZACAO();
                var gdi = Guid.NewGuid();
                bool existeCracha = false;
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = lEst.DESC_PRODUTO;
                                            mv.Resultado = lEst.DESC_STATUS;
                                            mv.EPC = epc;
                                            mv.DataMovimentacao = DateTime.Now;
                                            mv.corAviso = "#ff7f7f";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });


                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Data de Validade Vencida";
                                            mv.EPC = epc;
                                            mv.corAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }


                                    }
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não existe em nossa Base de dados";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não existe em nossa Base de dados";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });

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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                                            mv.corAviso = "#ffffff";
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }
                                        else
                                        {
                                            mv.Produto = "";
                                            mv.Resultado = "Este item não esta em Higienização";
                                            mv.EPC = epc;
                                            mv.corAviso = "#ff7f7f";
                                            mv.DataMovimentacao = DateTime.Now;
                                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                        }

                                    }
                                    else
                                    {
                                        mv.Produto = "";
                                        mv.Resultado = "Este item não esta em Higienização";
                                        mv.EPC = epc;
                                        mv.corAviso = "#ff7f7f";
                                        mv.DataMovimentacao = DateTime.Now;
                                        mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                    }
                                }
                                else
                                {
                                    mv.Produto = "";
                                    mv.Resultado = "Este item não esta em Higienização";
                                    mv.EPC = epc;
                                    mv.corAviso = "#ff7f7f";
                                    mv.DataMovimentacao = DateTime.Now;
                                    mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                                }
                            }
                            else
                            {
                                mv.Produto = "";
                                mv.Resultado = "Este item não esta em Higienização";
                                mv.EPC = epc;
                                mv.corAviso = "#ff7f7f";
                                mv.DataMovimentacao = DateTime.Now;
                                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
                            }
                        }
                        else
                        {
                            mv.Produto = "";
                            mv.Resultado = "Este item não esta em Higienização";
                            mv.EPC = epc;
                            mv.corAviso = "#ff7f7f";
                            mv.DataMovimentacao = DateTime.Now;
                            mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
                mv.corAviso = "#ff7f7f";
                mov.Add(new RESULTADOMOV { Resultado = mv.Resultado, EPC = mv.EPC, DataMovimentacao = mv.DataMovimentacao, Produto = mv.Produto, corAviso = mv.corAviso });
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
