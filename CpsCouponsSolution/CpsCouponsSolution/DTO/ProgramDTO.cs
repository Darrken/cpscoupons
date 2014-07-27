﻿using System.Collections.Generic;
using System.Linq;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class ProgramDTO
	{
		public ProgramDTO(Program program)
		{
			Id = program.Id;
			Name = program.Name;
			Description = program.Description;
			Disclaimer = program.Disclaimer;
			CouponWordCount = program.CouponWordCount;
			Retailers = program.Program_Retailers.Select(r => new RetailerDTO(r)).ToList();
			Fields = program.Program_Fields.Select(f => new FieldDTO(f)).ToList();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Disclaimer { get; set; }
		public int? CouponWordCount { get; set; }
		public List<RetailerDTO> Retailers { get; set; }
		public List<FieldDTO> Fields { get; set; }
	}
}