using System;
using System.Drawing;
using System.Linq;
using Xceed.Words.NET;
using System.Web;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class DocumentService
    {
        public void convertHtmlDocx(int? FK_FUNCIONARIO, string codDistribuicao, string nomeArquivo)
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

                var dadosFicha = retornarDadosFicha(FK_FUNCIONARIO);

                //var nomeArquivo = FK_FUNCIONARIO + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() +
                //    DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                //    DateTime.Now.Second.ToString();

                string path = HttpContext.Current.Server.MapPath("Doc_FichaCadastral");

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
                doc.InsertParagraph("CONTROLE DE ENTREGA DE EPI'S\nEQUIPAMENTOS DE PROTEÇÃO INDIVIDUAL", false, titleFormat)
                    .Alignment = Alignment.center;
                doc.InsertParagraph("", false, titleFormat).Position(20);
                doc.InsertParagraph("Eu " + dadosFicha.Split('|')[0] + " Registro No " +
                    dadosFicha.Split('|')[1] + " Função " + dadosFicha.Split('|')[2] +
                    " declaro para  todos  os  efeitos  legais  que  recebi  da Leal Equipamentos de Proteção , " +
                    "os equipamentos de proteção individual (EPI) relacionados abaixo, bem como as instruções para " +
                    "sua correta utilização, obrigando-me:");

                doc.InsertParagraph("1) usar o EPI e uniforme indicado, apenas às finalidades a que se destina;");
                doc.InsertParagraph("2) comunicar o setor de obras /segurança do trabalho, " +
                    "qualquer alteração no EPI que o torne parcialmente ou totalmente danificado;");
                doc.InsertParagraph("3) responsabilizar-me pelos danos do EPI, quando usados " +
                    "de modo inadequado ou fora das atividades a que se destina, bem como pelo seu extravio;");
                doc.InsertParagraph("4) devolvê-lo quando da troca por outro ou no meu desligamento da empresa.").Position(20);

                int count = 0;
                var listaEpis = retornarLista(FK_FUNCIONARIO, Guid.Parse(codDistribuicao));
                Table t = doc.AddTable((listaEpis.Count + 1), 7);
                t.Alignment = Alignment.center;
 
                Formatting column = new Formatting();
                column.Bold = true;
                column.Size = 8d;

                t.Rows[count].Cells[0].Paragraphs[0].Append("Qtd.", column).Alignment = Alignment.center;
                t.Rows[count].Cells[1].Paragraphs[0].Append("EPI'S", column).Alignment = Alignment.center;
                t.Rows[count].Cells[2].Paragraphs[0].Append("Data de Entrega", column).Alignment = Alignment.center;
                t.Rows[count].Cells[3].Paragraphs[0].Append("EPC", column).Alignment = Alignment.center;
                t.Rows[count].Cells[4].Paragraphs[0].Append("Data de Devolução", column).Alignment = Alignment.center;
                t.Rows[count].Cells[5].Paragraphs[0].Append("Assinatura de Devolução Colaborador", column).Alignment = Alignment.center;
                t.Rows[count].Cells[6].Paragraphs[0].Append("Assinatura de Devolução Responsável", column).Alignment = Alignment.center;
                
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

                doc.InsertTable(t);

                doc.InsertParagraph("", false, titleFormat).Position(20);
                doc.InsertParagraph("Declaro para todos os efeitos legais que recebi todos os Equipamentos de " +
                    "Proteção Individual constantes da lista acima, novos e em perfeitas condições de uso, e que estou " +
                    "ciente das obrigações descritas na NR 06, baixada pela Portaria MTB 3214 / 78, sub - item 6.7.1, a saber: ");
                doc.InsertParagraph("a) usar, utilizando-o apenas para a finalidade a que se destina;");
                doc.InsertParagraph("b) responsabilizar-se pela guarda e conservação; ");
                doc.InsertParagraph("c) comunicar ao empregador qualquer alteração que o torne impróprio para uso; e ");
                doc.InsertParagraph("d) cumprir as determinações do empregador sobre o uso adequado.");
                doc.InsertParagraph("Declaro, também, que estou ciente das disposições do Art. 462 e § 1º da CLT, e autorizo " +
                    "o desconto salarial proporcional ao custo de reparação do dano que os EPI’s aos meus cuidados venham apresentar.");
                doc.InsertParagraph("Declaro, ainda estar ciente de que o uso é obrigatório, sob pena " +
                    "de ser punido conforme Lei nº 6.514, de 27/12/77, artigo 158. ");
                doc.InsertParagraph("Declaro, ainda, que recebi treinamento referente ao uso do E.P.I. " +
                    "e as Normas de Segurança do Trabalho.");
                doc.InsertParagraph("", false, titleFormat).Position(20);
                titleFormat.FontColor = Color.Red;
                titleFormat.Size = 11;
                titleFormat.Bold = false;
                doc.InsertParagraph("Data: " + dadosFicha.Split('|')[3], false, titleFormat).Position(10);
                doc.InsertParagraph("Local: _______________________________", false, titleFormat).Position(10);
                doc.InsertParagraph("ASSINATURA: _______________________________").Position(10);
                doc.Save();
            }
            catch (Exception E)
            {
                throw new Exception(E.Message.ToString());
            }
        }

        private static string retornarDadosFicha(int? FK_FUNCIONARIO)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();
                var l = dbo.L_FICHACADASTRAL.Where(x => x.FK_FUNCIONARIO == FK_FUNCIONARIO).ToList();

                if (l.Count > 0)
                {
                    var matri = dbo.L_FUNCIONARIOS.Where(x => x.ID == FK_FUNCIONARIO).ToList();
                    return matri[0].NOME + " " + matri[0].SOBRENOME + "|" + matri[0].MATRICULA + "|" + matri[0].FUNCAO + "|" + DateTime.Now.ToLongDateString() + "|" + matri[0].CNPJ + "|" + matri[0].FK_CLIENTE;
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

        private List<RESULTADO> retornarLista(int? FK_FUNCIONARIO, Guid codDistribuicao)
        {
            try
            {
                webservicetwos3Entities dbo = new webservicetwos3Entities();

                var result = dbo.L_MOVIMENTACAO_ESTOQUE.Where(x => x.FK_FUNCIONARIO == FK_FUNCIONARIO
                    && x.COD_DISTRIBUICAO == codDistribuicao).ToList();

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
    }
}