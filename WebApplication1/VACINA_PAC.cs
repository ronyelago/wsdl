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
    
    public partial class VACINA_PAC
    {
        public int ID { get; set; }
        public Nullable<int> CODIGO_CARTEIRA { get; set; }
        public string COD_PRO { get; set; }
        public string NOME_PAC { get; set; }
        public string NUM_DOSE { get; set; }
        public string NOME_VACINA { get; set; }
        public string ESPECIFICACAO { get; set; }
        public Nullable<System.DateTime> DATA_APLICACAO { get; set; }
        public Nullable<bool> APLICADA { get; set; }
        public string PROF_APLICOU { get; set; }
        public string RG_PROF_APLICOU { get; set; }
        public Nullable<System.DateTime> DT_PREVISTA_APLIC { get; set; }
        public string CAMPANHA { get; set; }
        public string MOTIVO_VACINA { get; set; }
        public string TIPO_DOSE { get; set; }
        public Nullable<System.Guid> CODIGO_QR { get; set; }
        public Nullable<System.DateTime> DATA_INSERT { get; set; }
        public Nullable<System.DateTime> DT_ULT_ATUALIZACAO { get; set; }
    }
}
