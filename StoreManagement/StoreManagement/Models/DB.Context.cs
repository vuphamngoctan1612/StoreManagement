﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StoreManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class STOREMANAGEMENT_Entities : DbContext
    {
        public STOREMANAGEMENT_Entities()
            : base("name=STOREMANAGEMENT_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACCOUNT> ACCOUNTs { get; set; }
        public virtual DbSet<BILL> BILLs { get; set; }
        public virtual DbSet<BILLINFO> BILLINFOes { get; set; }
        public virtual DbSet<GOODS> GOODS { get; set; }
        public virtual DbSet<STORE> STOREs { get; set; }
        public virtual DbSet<TYPEOFSTORE> TYPEOFSTOREs { get; set; }
    }
}
