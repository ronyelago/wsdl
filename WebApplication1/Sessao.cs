using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class Sessao
    {
        internal static string pathToFiles;

        public static string Cnpj { get;  set; }
        public static HttpCookie Cookies { get; internal set; }
        public static int FkCnpj { get;  set; }
    }
}