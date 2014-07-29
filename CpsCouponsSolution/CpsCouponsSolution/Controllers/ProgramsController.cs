using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CpsCouponsSolution.DTO;
using CpsCouponsSolution.Models;
using CpsCouponsSolution.Services;

namespace CpsCouponsSolution.Controllers
{
	public class ProgramsController : ApiController
	{

		// TODO: Add DI and construct controller with ProgramsService

		public HttpResponseMessage CreateProgram(ProgramDTO programData)
		{
			int programId;
			try
			{
				var programsService = new ProgramsService();
				programId = programsService.CreateProgram(programData);
			}
			catch (ArgumentNullException ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Argument errors when trying to create the program.  " + ex.Message });
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error creating the program.  " + ex.Message });
			}
			
			return Request.CreateResponse(HttpStatusCode.OK, new {Id = programId});
		}

		[HttpPost]
		public HttpResponseMessage DeleteProgram(int programId)
		{
			if(programId <= 0)
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { wasSuccessful = false, message = "ProgramId invalid." });

			try
			{
				var programsService = new ProgramsService();
				programsService.DeleteProgram(programId);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error deleting the program.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, new { wasSuccessful = true });
		}

		public HttpResponseMessage GetProgramById(int programId)
		{
			if (programId <= 0)
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "ProgramId invalid." });
			
			ProgramDTO program;
			try
			{
				var programsService = new ProgramsService();
				program = programsService.GetProgramById(programId);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving the program.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, program);
		}

		public HttpResponseMessage GetProgramByRetailer(string guid)
		{
			if (string.IsNullOrEmpty(guid))
				return Request.CreateResponse(HttpStatusCode.BadRequest, new {  message = "Retailer Guid invalid." });

			ProgramDTO program;
			try
			{
				var programsService = new ProgramsService();
				program = programsService.GetProgramByRetailerGuId(guid);
				if(program == null)
					return Request.CreateResponse(HttpStatusCode.OK, new { message = "Retailer not found." });
			}
			catch (ArgumentException ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "There was an error retrieving program data.  " + ex.Message });
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving the program for the retailer.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, program);
		}

		public HttpResponseMessage GetRetailersByProgram(int programId)
		{
			if (programId <= 0)
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "ProgramId invalid." });

			List<RetailerDTO> retailers;
			try
			{
				var programsService = new ProgramsService();
				retailers = programsService.GetRetailersByProgramId(programId);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving retailers for the specified program.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, retailers);
		}

		public HttpResponseMessage GetProgramList()
		{
			List<ProgramDTO> programs;
			try
			{
				var programsService = new ProgramsService();
				programs = programsService.GetProgramList();
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving the program list.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, programs);
		}

		public HttpResponseMessage GetMallList()
		{
			List<MallDTO> mallList;
			try
			{
				var programsService = new ProgramsService();
				mallList = programsService.GetMallNames(false);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error retrieving the mall list.  " + ex.Message});
			}
			
			return Request.CreateResponse(HttpStatusCode.OK, mallList);
		}

		public HttpResponseMessage GetMallList(bool isAll)
		{
			List<MallDTO> mallNames;
			try
			{
				var programsService = new ProgramsService();
				mallNames = programsService.GetMallNames(isAll);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error retrieving the mall list.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, mallNames);
		}

		public HttpResponseMessage SignUp(RetailerDTO retailerDto)
		{
			ResponseResult responseResult;
			try
			{
				var programsService = new ProgramsService();
				responseResult = programsService.RetailerSignUp(retailerDto);
			}
			catch (ArgumentNullException ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Argument errors when trying to update retailer program data.  " + ex.Message });
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error saving the retailer data.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, responseResult);
		}

		public HttpResponseMessage GetRetailersByMall(int mallId)
		{
			if (mallId <= 0)
				return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Mall Id invalid." });

			List<RetailerDTO> retailers;
			try
			{
				var programsService = new ProgramsService();
				retailers = programsService.GetRetailersByMallId(mallId);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving retailers for the specified mall.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, retailers);
		}
	}
}