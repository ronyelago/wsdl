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
            
            //var uri = @"E:\Projetos\LEAL\Json\Json_130718_1412222.json";
            //var jsonF = System.IO.File.ReadAllText(uri);

            webservicetwos3Entities dbo = new webservicetwos3Entities();
            List<RootObject> bsObj = JsonConvert.DeserializeObject<List<RootObject>>(json);
            List<RESULTADO_IMPORTACAO> r = new List<RESULTADO_IMPORTACAO>();
            string erro = "";
            int count = 0;
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
                    foreach(var mI in it.Produtos)
                    {
                        //if(mI.CA == "")
                        //{
                        //    erro = erro + ";CA em Branco";
                        //}

                        if (mI.Codigo_do_Produto == "")
                        {
                            erro = erro + ";Codigo do Produto em Branco";
                        }

                        if (mI.Cod_Fornecedor == "")
                        {
                            erro = erro + ";Cod. Fornecedor em Branco";
                        }

                        if (mI.Data_Fabricacao == "")
                        {
                            erro = erro + ";Dt. Fabricação em Branco";
                        }

                        if (mI.Data_Validade == "")
                        {
                            erro = erro + ";Dt. Validade em Branco";
                        }

                        if (mI.Desc_Produto == "")
                        {
                            erro = erro + ";Desc. Produto em Branco";
                        }

                        if (mI.EPC == "")
                        {
                            erro = erro + ";EPC em Branco";
                        }

                        if (mI.ID < 1)
                        {
                            erro = erro + ";ID em Branco";
                        }

                        if (mI.Numero_Lote == "")
                        {
                            erro = erro + ";Numero Lote em Branco";
                        }

                        if (mI.Validade_Teste == "")
                        {
                            erro = erro + ";Validade de Teste em Branco";
                        }

                        if(erro == "")
                        {
                            string epc = mI.EPC;
                            string mensa = "";
                            string codProduto = mI.Codigo_do_Produto;
                            try
                            {
                                L_PRODUTOS lc = dbo.L_PRODUTOS.First<L_PRODUTOS>(x => x.COD_PRODUTO == codProduto);
                                lc.COD_PRODUTO = codProduto;
                                lc.PRODUTO = mI.Desc_Produto;
                                dbo.SaveChanges();
                            }
                            catch
                            {
                                L_PRODUTOS lc = new L_PRODUTOS();
                                lc.COD_PRODUTO = codProduto;
                                lc.PRODUTO = mI.Desc_Produto;
                                dbo.L_PRODUTOS.Add(lc);
                                dbo.SaveChanges();
                            }

                            try
                            {
                                L_PRODUTOS_ITENS lp = dbo.L_PRODUTOS_ITENS.First<L_PRODUTOS_ITENS>(x => x.EPC == epc);
                                lp.CA = mI.CA;
                                lp.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lp.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                lp.COD_FORNECEDOR = mI.Cod_Fornecedor;
                                lp.COD_PRODUTO = mI.Codigo_do_Produto;
                                lp.DATA_ENTRADA = DateTime.Now;
                                lp.DT_FABRICACAO = DateTime.Parse(mI.Data_Fabricacao);
                                lp.DT_VALIDADE = DateTime.Parse(mI.Data_Validade);
                                lp.EPC = mI.EPC;
                                lp.NUMERO_LOTE = mI.Numero_Lote;
                                lp.NUMERO_NOTA = it.Numero_da_Nota;
                                lp.PRODUTO = mI.Desc_Produto;
                                lp.VALIDADE_TESTE = DateTime.Parse(mI.Validade_Teste);
                                lp.GRUPO_CLIENTE = mI.Grupo;
                                lp.NORMA = mI.Norma;
                                dbo.SaveChanges();
                                mensa = "Atualizado";
                            }
                            catch
                            {
                                L_PRODUTOS_ITENS lp = new L_PRODUTOS_ITENS();
                                lp.CA = mI.CA;
                                lp.CNPJ_EMITENTE = it.CNPJ_Emitente;
                                lp.CNPJ_DESTINATARIO = it.CNPJ_Destinatario;
                                lp.COD_FORNECEDOR = mI.Cod_Fornecedor;
                                lp.COD_PRODUTO = mI.Codigo_do_Produto;
                                lp.DATA_ENTRADA = DateTime.Now;
                                lp.DT_FABRICACAO = DateTime.Parse(mI.Data_Fabricacao);
                                lp.DT_VALIDADE = DateTime.Parse(mI.Data_Validade);
                                lp.EPC = mI.EPC;
                                lp.NUMERO_LOTE = mI.Numero_Lote;
                                lp.NUMERO_NOTA = it.Numero_da_Nota;
                                lp.PRODUTO = mI.Desc_Produto;
                                lp.VALIDADE_TESTE = DateTime.Parse(mI.Validade_Teste);
                                lp.GRUPO_CLIENTE = mI.Grupo;
                                lp.NORMA = mI.Norma;
                                dbo.L_PRODUTOS_ITENS.Add(lp);
                                dbo.SaveChanges();
                                mensa = "OK";
                            }

                            r[count].Add(new ItensResultado
                            {
                                ID = mI.ID,
                                Cod_Produto = mI.Codigo_do_Produto,
                                EPC = mI.EPC,
                                Mensagem = mensa
                            });
                        }
                        else
                        {
                            r[count].Add(new ItensResultado
                            {
                                ID = mI.ID,
                                Cod_Produto = mI.Codigo_do_Produto,
                                EPC = mI.EPC,
                                Mensagem = erro
                            });
                        }
                    }
                }
                count++;
            }

            return JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public string consultaNotaImportada(int numeroNota)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var r = dbo.L_PRODUTOS.Where(x => x.NUMERO_NOTA == numeroNota).ToList();
                return JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return "Nota não Importada ou Não encontrada";
            }
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
