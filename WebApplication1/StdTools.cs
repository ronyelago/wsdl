using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class StdTools
    {

        public static string Unformatted(string input)
        {
            return input.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "");
        }

    }
}