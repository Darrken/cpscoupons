using System;
using System.Collections.Generic;
using System.Linq;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class RetailerDTO
	{
		public RetailerDTO()
		{}

		public RetailerDTO(Program_Retailers retailer)
		{
			Id = retailer.Id;
			Email = retailer.Email;
			UrlGuid = retailer.UrlGuid;
			HasSignedUp = retailer.Program_Field_Values.Any() || retailer.Program_Retailer_Selected_Malls.Any();
			FieldValues = retailer.Program_Field_Values.Select(v => new FieldValueDTO(v)).ToList();
			SelectedMalls = retailer.Program_Retailer_Selected_Malls.Select(m => m.MallId).ToList();
		}

		public List<FieldValueDTO> FieldValues { get; set; }
		public List<int> SelectedMalls { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }
		public Guid UrlGuid { get; set; }
		public bool HasSignedUp { get; set; }
		public string StoreName { get; set; }
		public string Offer { get; set; }
		public string Restrictions { get; set; }
	}
}