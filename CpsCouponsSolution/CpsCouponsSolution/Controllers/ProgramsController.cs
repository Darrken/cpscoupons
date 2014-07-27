﻿using System;
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
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error creating the program.  " + ex.Message });
			}
			
			return Request.CreateResponse(HttpStatusCode.OK, new {Id = programId});
		}

		[HttpPost]
		public HttpResponseMessage DeleteProgram(int programId)
		{
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
			ProgramDTO program;
			try
			{
				var programsService = new ProgramsService();
				program = programsService.GetProgramByRetailerGuId(guid);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { wasSuccessful = false, message = "There was an error retrieving the program for the retailer.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, program);
		}

		public HttpResponseMessage GetRetailersByProgram(int programId)
		{
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

		public HttpResponseMessage SignUp(ProgramDTO programData)
		{
			SignUpResult signUpResult;
			try
			{
				var programsService = new ProgramsService();
				signUpResult = programsService.RetailerSignUp(programData);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "There was an error saving the retailer data.  " + ex.Message });
			}

			return Request.CreateResponse(HttpStatusCode.OK, signUpResult);
		}

		public HttpResponseMessage GetRetailersByMall(int mallId)
		{
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