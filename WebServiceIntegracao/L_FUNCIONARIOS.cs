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
    
    public partial class L_FUNCIONARIOS
    {
        public int ID { get; set; }
        public string NOME { get; set; }
        public string SOBRENOME { get; set; }
        public string CPF { get; set; }
        public string FUNCAO { get; set; }
        public string FK_FUNCAO { get; set; }
        public string MATRICULA { get; set; }
        public byte[] SENHA { get; set; }
        public string TIPO_SANGUINEO { get; set; }
        public string STATUS { get; set; }
        public Nullable<int> FK_CLIENTE { get; set; }
        public string CNPJ { get; set; }
        public string TELEFONE { get; set; }
        public string EMAIL { get; set; }
    }
}
