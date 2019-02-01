using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1
{
    public class ControleSessao
    {
        webservicetwos3Entities dbo = new webservicetwos3Entities();
        public bool verificarSeExisteSessao(string sessionID)
        {
            try
            {
                var list = dbo.SessaoUsuario.First(x => x.SessionId == sessionID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public SessaoUsuario retornarDadosPorSessao(string sessionID)
        {
            try
            {
                var list = dbo.SessaoUsuario.First(x => x.SessionId == sessionID);
                return list;
            }
            catch
            {
                return null;
            }
        }

        public string retornarCampoSessaoPorSessionId(string v, string sessionID)
        {
            try
            {
                var result = dbo.Database.SqlQuery<string>("SELECT " + v + " FROM SessaoUsuario WHERE SessionId='" + sessionID + "'").ToList();
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        return result[0];
                    }
                    else
                    {
                        return null;
                    }
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

        public SessaoUsuario criarSessao(string sessionID)
        {
            try
            {
                SessaoUsuario su = new SessaoUsuario();
                su.SessionId = sessionID;
                dbo.SessaoUsuario.Add(su);
                dbo.SaveChanges();
                return su;
            }
            catch
            {
                return null;
            }
        }

        public bool incluirDadosdeSessa(SessaoUsuario inserir)
        {
            try {
                SessaoUsuario sus = dbo.SessaoUsuario.First(x => x.ID == inserir.ID);
                sus.NomeEmpresa = inserir.NomeEmpresa;
                sus.nomeFuncionario = inserir.nomeFuncionario;
                sus.pAcesso = inserir.pAcesso;
                sus.senhaFuncionario = inserir.senhaFuncionario;
                sus.SessionId = inserir.SessionId;
                sus.autenticado = inserir.autenticado;
                dbo.SaveChanges();
                return true;
            }
            catch { return false; }
        }
    }
}