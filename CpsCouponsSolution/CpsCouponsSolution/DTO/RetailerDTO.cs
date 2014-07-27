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
			HasSignedUp = retailer.Program_Field_Values.Any();
			FieldValues = retailer.Program_Field_Values.Select(v => new FieldValueDTO(v)).ToList();
		}

		public List<FieldValueDTO> FieldValues { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }
		public Guid UrlGuid { get; set; }
		public bool HasSignedUp { get; set; }
	}
}