//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CpsCouponsSolution.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Program_Retailers
    {
        public Program_Retailers()
        {
            this.Program_Field_Values = new HashSet<Program_Field_Values>();
            this.Program_Retailer_Selected_Malls = new HashSet<Program_Retailer_Selected_Malls>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
        public System.Guid UrlGuid { get; set; }
        public int ProgramId { get; set; }
        public string StoreName { get; set; }
        public string Offer { get; set; }
        public string Restrictions { get; set; }
    
        public virtual ICollection<Program_Field_Values> Program_Field_Values { get; set; }
        public virtual ICollection<Program_Retailer_Selected_Malls> Program_Retailer_Selected_Malls { get; set; }
        public virtual Program Program { get; set; }
    }
}
