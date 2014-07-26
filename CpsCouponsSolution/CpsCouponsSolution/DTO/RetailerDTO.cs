using System;

namespace CpsCouponsSolution.DTO
{
	public class RetailerDTO
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public Guid UrlGuid { get; set; }
		public bool HasSignedUp { get; set; }
	}
}