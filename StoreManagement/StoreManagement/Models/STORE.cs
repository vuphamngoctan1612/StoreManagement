//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class STORE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STORE()
        {
            this.BILLs = new HashSet<BILL>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public Nullable<int> TYPEOFSTORE { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string DISTRICT { get; set; }
        public Nullable<System.DateTime> CHECKIN { get; set; }
        public Nullable<long> DEBT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BILL> BILLs { get; set; }
        public virtual TYPEOFSTORE TYPEOFSTORE1 { get; set; }
    }
}