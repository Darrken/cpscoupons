using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.Controllers
{
	public class CouponsController : ApiController
	{
		public IEnumerable<string> GetSomeMalls()
		{
			using (var dbContext = new ToolkitEntities())
			{
				return dbContext.Malls
									.Where(m => !m.Name.Contains("?") 
												&& !m.Name.ToLower().StartsWith("test")
												&& !m.Name.ToLower().StartsWith("zz"))
									.OrderBy(m => m.Name)
									.Select(m => m.Name + " - " + m.State.Abbreviation)
									.ToList();
			}
		}
		public IEnumerable<string> GetAllMallNames()
		{
			using (var dbContext = new ToolkitEntities())
			{
				return dbContext.Malls
									.OrderBy(m => m.Name)
									.Select(m => m.Name + " - " + m.State.Abbreviation)
									.ToList();
			}
		}

		//// GET api/values/5
		//public string Get(int id)
		//{
		//	return "value";
		//}

		//// POST api/values
		//public void Post([FromBody] string value)
		//{
		//}

		//// PUT api/values/5
		//public void Put(int id, [FromBody] string value)
		//{
		//}

		//// DELETE api/values/5
		//public void Delete(int id)
		//{
		//}
	}
}