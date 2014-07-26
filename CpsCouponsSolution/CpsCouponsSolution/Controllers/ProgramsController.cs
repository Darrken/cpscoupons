using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.Controllers
{
	public class ProgramsController : ApiController
	{
		public HttpResponseMessage CreateProgram()
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetProgramById(int programId)
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetProgramByRetailer(string urlGuid)
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetRetailersByProgram(int programId)
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetProgramList()
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetMallList()
		{
			List<string> mallList;
			try
			{
				mallList = GetMallNames(false);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error retrieving the mall list."});
			}
			
			return Request.CreateResponse(HttpStatusCode.OK, mallList);
		}

		public HttpResponseMessage GetMallList(bool isAll)
		{
			List<string> mallNames;
			try
			{
				mallNames = GetMallNames(isAll);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error retrieving the mall list." });
			}

			return Request.CreateResponse(HttpStatusCode.OK, mallNames);
		}

		private static List<string> GetMallNames(bool isAll)
		{
			List<string> mallNames;
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Malls.Select(m => new {Name = m.Name + " - " + m.State.Abbreviation});

				if (!isAll)
					malls = malls.Where(m => !m.Name.Contains("?")
					                         && !m.Name.ToLower().StartsWith("test")
					                         && !m.Name.ToLower().StartsWith("zz"));

				mallNames = malls.OrderBy(m => m.Name).Select(m => m.Name).ToList();
			}
			return mallNames;
		}

		public HttpResponseMessage SignUp()
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		public HttpResponseMessage GetRetailersByMall(int mallId)
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}