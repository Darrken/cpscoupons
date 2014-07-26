using System.Collections.Generic;

namespace CpsCouponsSolution.DTO
{
	public class ProgramDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Disclaimer { get; set; }
		public int CouponWordCount { get; set; }
		public List<RetailerDTO> Retailers { get; set; }
		public List<FieldDTO> Fields { get; set; }
	}
}