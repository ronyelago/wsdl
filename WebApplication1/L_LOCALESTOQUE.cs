//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class L_LOCALESTOQUE
    {
        public int ID { get; set; }
        public string CODIGO { get; set; }
        public string NOME { get; set; }
        public string DESCRICAO { get; set; }
        public Nullable<System.DateTime> DATA_CADASTRO { get; set; }
        public Nullable<int> FK_CLIENTE { get; set; }
    }
}
