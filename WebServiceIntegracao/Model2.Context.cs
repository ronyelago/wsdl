﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class webservicetwos3Entities : DbContext
    {
        public webservicetwos3Entities()
            : base("name=webservicetwos3Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<L_ATRIBUICAOCRACHA> L_ATRIBUICAOCRACHA { get; set; }
        public virtual DbSet<L_DESCARTE> L_DESCARTE { get; set; }
        public virtual DbSet<L_ENVIO_PARA_TESTE> L_ENVIO_PARA_TESTE { get; set; }
        public virtual DbSet<L_EPCPERDIDOS> L_EPCPERDIDOS { get; set; }
        public virtual DbSet<L_ESTOQUE> L_ESTOQUE { get; set; }
        public virtual DbSet<L_FICHACADASTRAL> L_FICHACADASTRAL { get; set; }
        public virtual DbSet<L_FUNCAO_PRODUTOS> L_FUNCAO_PRODUTOS { get; set; }
        public virtual DbSet<L_FUNCIONARIOS> L_FUNCIONARIOS { get; set; }
        public virtual DbSet<L_FUNCOES> L_FUNCOES { get; set; }
        public virtual DbSet<L_INSPECAOFUNCIONARIO> L_INSPECAOFUNCIONARIO { get; set; }
        public virtual DbSet<L_INSPITEM> L_INSPITEM { get; set; }
        public virtual DbSet<L_LOCALESTOQUE> L_LOCALESTOQUE { get; set; }
        public virtual DbSet<L_LOGIN> L_LOGIN { get; set; }
        public virtual DbSet<L_LOGIN_FUNCIONARIO> L_LOGIN_FUNCIONARIO { get; set; }
        public virtual DbSet<L_MANUTENCAOEPI> L_MANUTENCAOEPI { get; set; }
        public virtual DbSet<L_MENSAGEM_OCORRENCIA> L_MENSAGEM_OCORRENCIA { get; set; }
        public virtual DbSet<L_MOVIMENTACAO_ESTOQUE> L_MOVIMENTACAO_ESTOQUE { get; set; }
        public virtual DbSet<L_NAOCONFORMIDADE> L_NAOCONFORMIDADE { get; set; }
        public virtual DbSet<L_PERFIL> L_PERFIL { get; set; }
        public virtual DbSet<L_STATUSOCORRENCIA> L_STATUSOCORRENCIA { get; set; }
        public virtual DbSet<L_TELAS> L_TELAS { get; set; }
        public virtual DbSet<L_TELAS_PERFIL> L_TELAS_PERFIL { get; set; }
        public virtual DbSet<L_USUARIO_COLABORADOR> L_USUARIO_COLABORADOR { get; set; }
        public virtual DbSet<L_USUARIOS> L_USUARIOS { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public virtual DbSet<Counter> Counter { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobParameter> JobParameter { get; set; }
        public virtual DbSet<JobQueue> JobQueue { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<DATA_VALIDADE_1> DATA_VALIDADE_1 { get; set; }
        public virtual DbSet<DATA_VALIDADE_1_TESTE> DATA_VALIDADE_1_TESTE { get; set; }
        public virtual DbSet<DATA_VALIDADE_15> DATA_VALIDADE_15 { get; set; }
        public virtual DbSet<DATA_VALIDADE_15_TESTE> DATA_VALIDADE_15_TESTE { get; set; }
        public virtual DbSet<DATA_VALIDADE_2> DATA_VALIDADE_2 { get; set; }
        public virtual DbSet<DATA_VALIDADE_3> DATA_VALIDADE_3 { get; set; }
        public virtual DbSet<DATA_VALIDADE_30DIAS> DATA_VALIDADE_30DIAS { get; set; }
        public virtual DbSet<DATA_VALIDADE_30DIAS_TESTE> DATA_VALIDADE_30DIAS_TESTE { get; set; }
        public virtual DbSet<DATA_VALIDADE_4> DATA_VALIDADE_4 { get; set; }
        public virtual DbSet<DATA_VALIDADE_5> DATA_VALIDADE_5 { get; set; }
        public virtual DbSet<DATA_VALIDADE_6> DATA_VALIDADE_6 { get; set; }
        public virtual DbSet<DATA_VALIDADE_7> DATA_VALIDADE_7 { get; set; }
        public virtual DbSet<DATA_VALIDADE_7_TESTE> DATA_VALIDADE_7_TESTE { get; set; }
        public virtual DbSet<DATA_VALIDADE_VENCIDA> DATA_VALIDADE_VENCIDA { get; set; }
        public virtual DbSet<DATA_VALIDADE_VENCIDA_TESTE> DATA_VALIDADE_VENCIDA_TESTE { get; set; }
        public virtual DbSet<ITENS_COM_VALIDADE_A_VENCER> ITENS_COM_VALIDADE_A_VENCER { get; set; }
        public virtual DbSet<ITENS_COM_VALIDADE_DE_TESTE_A_VENCER> ITENS_COM_VALIDADE_DE_TESTE_A_VENCER { get; set; }
        public virtual DbSet<L_CLIENTE> L_CLIENTE { get; set; }
        public virtual DbSet<L_PRODUTOS> L_PRODUTOS { get; set; }
        public virtual DbSet<L_PRODUTOS_ITENS> L_PRODUTOS_ITENS { get; set; }
        public virtual DbSet<L_IMPORTACAO_NOTACLIENTE> L_IMPORTACAO_NOTACLIENTE { get; set; }
    }
}
