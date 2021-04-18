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
    
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            this.InvoiceInfoes = new HashSet<InvoiceInfo>();
        }
    
        public int ID { get; set; }
        public Nullable<int> AgencyID { get; set; }
        public Nullable<System.DateTime> Checkout { get; set; }
        public Nullable<long> Debt { get; set; }
    
        public virtual Agency Agency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceInfo> InvoiceInfoes { get; set; }
    }
}