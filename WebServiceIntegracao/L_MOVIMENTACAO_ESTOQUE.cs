//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebServiceIntegracao
{
    using System;
    using System.Collections.Generic;
    
    public partial class L_MOVIMENTACAO_ESTOQUE
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> DATA_MOVIMENTACAO { get; set; }
        public string ENTRADA_SAIDA { get; set; }
        public Nullable<int> FK_PRODUTO { get; set; }
        public string DESC_PRODUTO { get; set; }
        public Nullable<int> FK_FUNCIONARIO { get; set; }
        public string NOME_FUNCIONARIO { get; set; }
        public string TRANSFERENCIA { get; set; }
        public string COD_ESTOQUE { get; set; }
        public string ESTOQUE { get; set; }
        public Nullable<int> QUANTIDADE { get; set; }
        public string COD_PRODUTO { get; set; }
        public string COD_FUNCIONARIO { get; set; }
        public Nullable<System.Guid> COD_DISTRIBUICAO { get; set; }
        public string EPC { get; set; }
        public string STATUS { get; set; }
        public string DESC_STATUS { get; set; }
        public string LAT { get; set; }
        public string LONG { get; set; }
    }
}
