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
    
    public partial class Program_Retailer_Selected_Malls
    {
        public int Id { get; set; }
        public int ProgramRetailerId { get; set; }
        public int MallId { get; set; }
    
        public virtual Program_Retailers Program_Retailers { get; set; }
        public virtual Mall Mall { get; set; }
    }
}
