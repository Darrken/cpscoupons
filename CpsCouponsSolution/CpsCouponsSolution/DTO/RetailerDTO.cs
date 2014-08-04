using System;
using System.Collections.Generic;
using System.Linq;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class RetailerDTO
	{
		public RetailerDTO()
		{ }

		public RetailerDTO(Program_Retailers retailer)
		{
			Id = retailer.Id;
			ProgramId = retailer.ProgramId;
			ProgramName = retailer.Program.Name;
			Email = retailer.Email;
			UrlGuid = retailer.UrlGuid;
			StoreName = retailer.StoreName;
			Offer = retailer.Offer;
			Restrictions = retailer.Restrictions;
			ContactName = retailer.ContactName;
			Address = retailer.Address;
			City = retailer.City;
			State = retailer.State;
			Zip = retailer.Zip;
			RepName = retailer.RepName;
			Phone = retailer.Phone;
			HasSignedUp = retailer.Program_Field_Values.Any() || retailer.Program_Retailer_Selected_Malls.Any();
			FieldValues = retailer.Program_Field_Values.Select(v => new FieldValueDTO(v)).ToList();
			SelectedMalls = retailer.Program_Retailer_Selected_Malls
				.Select(m => new MallDTO
				{
					Id = m.MallId,
					Name = m.Mall.Name,
					StateId = m.Mall.StateID,
					StateName = m.Mall.State.Name
				}).ToList();
		}

		public List<FieldValueDTO> FieldValues { get; set; }
		public List<MallDTO> SelectedMalls { get; set; }
		public int Id { get; set; }
		public int ProgramId { get; set; }
		public string ProgramName { get; set; }
		public string Email { get; set; }
		public Guid UrlGuid { get; set; }
		public bool HasSignedUp { get; set; }
		public string StoreName { get; set; }
		public string Offer { get; set; }
		public string Restrictions { get; set; }
		public string ContactName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string RepName { get; set; }
		public string Phone { get; set; }
	}
}