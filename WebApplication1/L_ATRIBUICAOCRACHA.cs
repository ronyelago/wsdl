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

    public partial class L_ATRIBUICAOCRACHA
    {
        public int ID { get; set; }
        public Nullable<int> FK_FUNCIONARIO { get; set; }
        public string MATRICULA { get; set; }
        public string CODIGO_CRACHA { get; set; }
        public string ATIVO { get; set; }
        public Nullable<System.DateTime> DATA_ATRIBUICAO { get; set; }
    }
}
