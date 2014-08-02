using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class ProgramDTO
	{
		public ProgramDTO()
		{}

		public ProgramDTO(Program program)
		{
			Id = program.Id;
			Name = program.Name;
			Description = program.Description;
			Disclaimer = program.Disclaimer;
			DeadlineCoupon = program.DeadlineCoupon;
			DeadlineInMall = program.DeadlineInMall;
			CouponWordCount = program.CouponWordCount;
			Retailers = program.Program_Retailers.Select(r => new RetailerDTO(r)).ToList();
			Fields = program.Program_Fields.Select(f => new FieldDTO(f)).ToList();
			ParticipatingMalls = program.Malls.Select(m =>new MallDTO(m)).ToList();
			Header = program.Header;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Disclaimer { get; set; }
		public DateTime? DeadlineCoupon { get; set; }
		public DateTime? DeadlineInMall { get; set; }
		public int? CouponWordCount { get; set; }
		public List<RetailerDTO> Retailers { get; set; }
		public List<FieldDTO> Fields { get; set; }
		public List<MallDTO> ParticipatingMalls { get; set; }
		public string Header { get; set; }
	}
}