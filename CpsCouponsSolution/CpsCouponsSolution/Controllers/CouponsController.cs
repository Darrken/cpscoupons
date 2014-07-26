using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.Controllers
{
	public class CouponsController : ApiController
	{
		public HttpResponseMessage GetMalls()
		{
			return GetMalls(false);
		}

		public HttpResponseMessage GetMalls(bool isAll)
		{
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Malls.Select(m => new { Name =  m.Name + " - " + m.State.Abbreviation});

				if (!isAll)
					malls = malls.Where(m => !m.Name.Contains("?") 
											&& !m.Name.ToLower().StartsWith("test")
											&& !m.Name.ToLower().StartsWith("zz"));

				var mallNames = malls.OrderBy(m => m.Name).Select(m => m.Name).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, mallNames);
			}
		}

	}
}