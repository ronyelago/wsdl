﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class inspecaoEntities : DbContext
    {
        public inspecaoEntities()
            : base("name=inspecaoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ANEISMETALICOSTATIL> ANEISMETALICOSTATIL { get; set; }
        public virtual DbSet<ANEISMETALICOSVISUAL> ANEISMETALICOSVISUAL { get; set; }
        public virtual DbSet<COSTURAVISUAL> COSTURAVISUAL { get; set; }
        public virtual DbSet<FITASTATIL> FITASTATIL { get; set; }
        public virtual DbSet<FITASVISUAL> FITASVISUAL { get; set; }
        public virtual DbSet<FIVELASVISUAL> FIVELASVISUAL { get; set; }
        public virtual DbSet<FUNCIONALIDADETATIL> FUNCIONALIDADETATIL { get; set; }
        public virtual DbSet<LINSPECAOMATERIAL> LINSPECAOMATERIAL { get; set; }
    }
}
