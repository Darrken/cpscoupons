using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class MallDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? StateId { get; set; }
		public string StateName { get; set; }

		public MallDTO()
		{}

		public MallDTO(Mall mall)
		{
			Id = mall.ID;
			Name = mall.Name;
			StateId = mall.State.ID;
			StateName = mall.State.Abbreviation;
		}
	}
}