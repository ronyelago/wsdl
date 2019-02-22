using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebServiceIntegracao
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

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string importarDadosNotaFornecedor(string json)
        {
            
            //var uri = @"E:\Projetos\LEAL\Json\Json_130718_1416609.json";
            //var jsonF = System.IO.File.ReadAllText(uri);

            webservicetwos3Entities dbo = new webservicetwos3Entities();
            List<RootObject> bsObj = JsonConvert.DeserializeObject<List<RootObject>>(json);
            List<RESULTADO_IMPORTACAO> r = new List<RESULTADO_IMPORTACAO>();
            string erro = "";
            int contador = 0;
            foreach(var it in bsObj)
            {
                erro = "";
                r.Add(new RESULTADO_IMPORTACAO(it.Numero_da_Nota));
                if (it.CNPJ_Emitente == "")
                {
                    erro = "CNPJ Emitente em Branco";
                }

                if (it.CNPJ_Destinatario == "")
                {
                    erro = ";CNPJ Destinatario em Branco";
                }

                if (it.Produtos.Count == 0)
                {
                    erro = erro + ";Não Existe Itens no Arquivo";
                }
                else
                {
                    foreach(var produtoImportacao in it.Produtos)
                    {
                        //if(mI.CA == "")
                        //{
                        //    erro = erro + ";CA em Branco";
                        //}

                        if (produtoImportacao.Codigo_do_Produto == "")
                        {
                            erro = erro + ";Codigo do Produto em Branco";
                        }

                        if (produtoImportacao.Cod_Fornecedor == "")
                        {
                            erro = erro + ";Cod. Fornecedor em Branco";
                        }

                        if (produtoImportacao.Data_Fabricacao == "")
                        {
                            erro = erro + ";Dt. Fabricação em Branco";
                        }

                        if (produtoImportacao.Data_Validade == "")
                        {
                            erro = erro + ";Dt. Validade em Branco";
                        }

                        if (produtoImportacao.Desc_Produto == "")
                        {
                            erro = erro + ";Desc. Produto em Branco";
                        }

                        if (produtoImportacao.EPC == "")
                        {
                            erro = erro + ";EPC em Branco";
                        }

                        if (produtoImportacao.ID < 1)
                        {
                            erro = erro + ";ID em Branco";
                        }

                        if (produtoImportacao.Numero_Lote == "")
                        {
                            erro = erro + ";Numero Lote em Branco";
                        }

                        if (produtoImportacao.Validade_Teste == "")
                        {
                            erro = erro + ";Validade de Teste em Branco";
                        }

                        if(erro == "")
                        {
                            string epc = produtoImportacao.EPC;
                            string mensagem = "";
                            string codigoProduto = produtoImportacao.Codigo_do_Produto;
                            try
                            {
                                var lpr = dbo.L_PRODUTOS.Where(x => x.COD_PRODUTO == codigoProduto && x.CNPJ_DESTINATARIO == it.CNPJ_Destinatario).ToList();
                                int idCod = lpr[0].ID;
                                L_PRODUTOS lc = dbo.L_PRODUTOS.First<L_PRODUTOS>(x => x.ID == idCod);
                                lc.COD_PRODUTO = codigoProduto;
                                lc.PRODUTO = produtoImportacao.Desc_Produto;
                                lc.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lc.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                dbo.SaveChanges();
                            }
                            catch
                            {
                                L_PRODUTOS lc = new L_PRODUTOS();
                                lc.COD_PRODUTO = codigoProduto;
                                lc.PRODUTO = produtoImportacao.Desc_Produto;
                                lc.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lc.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                dbo.L_PRODUTOS.Add(lc);
                                dbo.SaveChanges();
                            }

                            try
                            {
                                L_PRODUTOS_ITENS lp = dbo.L_PRODUTOS_ITENS.First<L_PRODUTOS_ITENS>(x => x.EPC == epc);
                                lp.CA = produtoImportacao.CA;
                                lp.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lp.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                lp.COD_FORNECEDOR = produtoImportacao.Cod_Fornecedor;
                                lp.COD_PRODUTO = produtoImportacao.Codigo_do_Produto;
                                lp.DATA_ENTRADA = DateTime.Now;
                                lp.DT_FABRICACAO = DateTime.Parse(produtoImportacao.Data_Fabricacao);
                                lp.DT_VALIDADE = DateTime.Parse(produtoImportacao.Data_Validade);
                                lp.EPC = produtoImportacao.EPC;
                                lp.NUMERO_LOTE = produtoImportacao.Numero_Lote;
                                lp.NUMERO_NOTA = it.Numero_da_Nota;
                                lp.PRODUTO = produtoImportacao.Desc_Produto;
                                lp.VALIDADE_TESTE = DateTime.Parse(produtoImportacao.Validade_Teste);
                                lp.GRUPO_CLIENTE = produtoImportacao.Grupo;
                                lp.NORMA = produtoImportacao.Norma;
                                dbo.SaveChanges();
                                mensagem = "Atualizado";
                            }
                            catch
                            {
                                L_PRODUTOS_ITENS lp = new L_PRODUTOS_ITENS();
                                lp.CA = produtoImportacao.CA;
                                lp.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lp.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                lp.COD_FORNECEDOR = produtoImportacao.Cod_Fornecedor;
                                lp.COD_PRODUTO = produtoImportacao.Codigo_do_Produto;
                                lp.DATA_ENTRADA = DateTime.Now;
                                lp.DT_FABRICACAO = DateTime.Parse(produtoImportacao.Data_Fabricacao);
                                lp.DT_VALIDADE = DateTime.Parse(produtoImportacao.Data_Validade);
                                lp.EPC = produtoImportacao.EPC;
                                lp.NUMERO_LOTE = produtoImportacao.Numero_Lote;
                                lp.NUMERO_NOTA = it.Numero_da_Nota;
                                lp.PRODUTO = produtoImportacao.Desc_Produto;
                                lp.VALIDADE_TESTE = DateTime.Parse(produtoImportacao.Validade_Teste);
                                lp.GRUPO_CLIENTE = produtoImportacao.Grupo;
                                lp.NORMA = produtoImportacao.Norma;
                                dbo.L_PRODUTOS_ITENS.Add(lp);
                                dbo.SaveChanges();
                                mensagem = "OK";
                            }

                          


                            r[contador].Add(new ItensResultado
                            {
                                ID = produtoImportacao.ID,
                                Cod_Produto = produtoImportacao.Codigo_do_Produto,
                                EPC = produtoImportacao.EPC,
                                Mensagem = mensagem
                            });
                        }
                        else
                        {
                            r[contador].Add(new ItensResultado
                            {
                                ID = produtoImportacao.ID,
                                Cod_Produto = produtoImportacao.Codigo_do_Produto,
                                EPC = produtoImportacao.EPC,
                                Mensagem = erro
                            });
                        }
                    }

                    L_IMPORTACAO_NOTACLIENTE notaImportacao = new L_IMPORTACAO_NOTACLIENTE();
                    notaImportacao.CNPJ_CLIENTE = it.CNPJ_Destinatario;
                    notaImportacao.DATA = DateTime.Now;
                    notaImportacao.NOTA = it.Numero_da_Nota;
                    notaImportacao.QTD_ITENS = it.Produtos.Count;
                    notaImportacao.NOTIFICACAO = "N";
                    dbo.L_IMPORTACAO_NOTACLIENTE.Add(notaImportacao);
                    dbo.SaveChanges();
                }
                contador++;
            }

            return JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public string consultaNotaImportada(int numeroNota)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var itensNota = dbo.L_PRODUTOS_ITENS.Where(x => x.NUMERO_NOTA == numeroNota).ToList();
                return JsonConvert.SerializeObject(itensNota, Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return "Nota não encontrada";
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string testeIntegracao()
        {

            var uri = @"E:\Projetos\LEAL\Json\Json_130718_1496553.json";
            var jsonF = System.IO.File.ReadAllText(uri);
            //InsertTeste.Client it = new InsertTeste.Client();
            //var result = it.importarDadosNotaFornecedor(jsonF);
            var result = importarDadosNotaFornecedor(jsonF);
            return result;



        }


    }

    public class RESULTADO_IMPORTACAO : List<ItensResultado>
    {
        public RESULTADO_IMPORTACAO()
        {

        }
        public RESULTADO_IMPORTACAO(int numero_da_nota)
        {
            Numero_da_Nota = numero_da_nota;
        }

        public int Numero_da_Nota { get;  set; }
       
    }

    public class Produto
    {
        public int ID { get; set; }
        public string EPC { get; set; }
        public string Codigo_do_Produto { get; set; }
        public string Data_Validade { get; set; }
        public string Desc_Produto { get; set; }
        public string Cod_Fornecedor { get; set; }
        public string Data_Fabricacao { get; set; }
        public string CA { get; set; }
        public string Numero_Lote { get; set; }
        public string Validade_Teste { get; set; }

        public string Grupo { get; set; }

        public string Norma { get; set; }
    }

    public class RootObject
    {
        public int Numero_da_Nota { get; set; }
        public string CNPJ_Emitente { get; set; }

        public string CNPJ_Destinatario { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
