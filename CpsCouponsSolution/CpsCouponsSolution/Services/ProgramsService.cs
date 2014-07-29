using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Management;
using CpsCouponsSolution.DTO;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.Services
{
	public class ProgramsService
	{
		public List<MallDTO> GetMallNames(bool isAll)
		{
			List<MallDTO> mallList;
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Malls.Select(m => new MallDTO {
																							Id = m.ID, 
																							Name = m.Name, 
																							StateId = m.State.ID, 
																							StateName = m.State.Abbreviation
																						});

				if (!isAll)
					malls = malls.Where(m => !m.Name.Contains("?")
													 && !m.Name.ToLower().StartsWith("test")
													 && !m.Name.ToLower().StartsWith("zz"));

				mallList = malls.OrderBy(m => m.Name).ToList();
			}

			return mallList;
		}		
		
		public List<MallDTO> GetProgramMalls(int programId)
		{
			List<MallDTO> mallList;
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Programs.Where(p => p.Id == programId).SelectMany(p => p.Malls.Select(m => new MallDTO
				{
					Id = m.ID,
					Name = m.Name,
					StateId = m.State.ID,
					StateName = m.State.Abbreviation
				}));

				mallList = malls.OrderBy(m => m.Name).ToList();
			}

			return mallList;
		}

		public void DeleteProgram(int programId)
		{
			using (var dbContext = new ToolkitEntities())
			{
				var programToRemove = dbContext.Programs.SingleOrDefault(p => p.Id == programId);
				
				if(programToRemove == null)
					throw new Exception("Program to delete not found.");

				dbContext.Entry(programToRemove).State = EntityState.Deleted;
				dbContext.SaveChanges();
			};
		}

		public int CreateProgram(ProgramDTO programData)
		{
			if(programData == null)
				throw new ArgumentNullException("programData");
			if (string.IsNullOrEmpty(programData.Disclaimer))
				throw new ArgumentNullException("Disclaimer");
			if (string.IsNullOrEmpty(programData.Description))
				throw new ArgumentNullException("Description");
			if (string.IsNullOrEmpty(programData.Name))
				throw new ArgumentNullException("Name");
			if (programData.Retailers == null || !programData.Retailers.Any())
				throw new ArgumentNullException("Retailers");
			if (programData.ParticipatingMalls == null || !programData.ParticipatingMalls.Any())
				throw new ArgumentNullException("ParticipatingMalls");

			Program newProgram;
			using (var dbContext = new ToolkitEntities())
			{
				newProgram = new Program()
				             {
					             CouponWordCount = programData.CouponWordCount,
					             Description = programData.Description,
					             Disclaimer = programData.Disclaimer,
					             Name = programData.Name
				             };

				if (programData.Fields != null && programData.Fields.Any())
				{
					foreach (var fieldDto in programData.Fields)
					{
						newProgram.Program_Fields.Add(new Program_Fields()
						                              {
							                              Name = fieldDto.Name,
							                              ProgramId = newProgram.Id
						                              });
					}
				}

				foreach (var retailerDto in programData.Retailers)
				{
					newProgram.Program_Retailers.Add(new Program_Retailers()
					{
						Email = retailerDto.Email,
						ProgramId = newProgram.Id,
						UrlGuid = Guid.NewGuid()
					}); 
				}

				foreach (var mallId in programData.ParticipatingMalls)
				{
					newProgram.Malls.Add(dbContext.Malls.First(m => m.ID == mallId)); 
				}

				dbContext.Programs.Add(newProgram);
				dbContext.SaveChanges();
			}

			return newProgram.Id;
		}

		public ProgramDTO GetProgramById(int programId)
		{
			ProgramDTO programDto;

			using (var dbContext = new ToolkitEntities())
			{
				var selectedProgram =dbContext.Programs.SingleOrDefault(p => p.Id == programId);

				if (selectedProgram == null)
					throw new Exception("Program not found.");

				programDto = new ProgramDTO(selectedProgram);
			}

			return programDto;
		}

		public ProgramDTO GetProgramByRetailerGuId(string urlGuid)
		{
			ProgramDTO programDto;
			Guid retailerGuid;
			
			if(!Guid.TryParse(urlGuid, out retailerGuid))
				throw new ArgumentException("Guid invalid.");

			using (var dbContext = new ToolkitEntities())
			{
				var selectedProgram = dbContext.Programs.SingleOrDefault(p => p.Program_Retailers.Any(r => r.UrlGuid == retailerGuid));

				if (selectedProgram == null)
					return null;

				programDto = new ProgramDTO(selectedProgram);
			}

			return programDto;
		}

		public List<RetailerDTO> GetRetailersByProgramId(int programId)
		{
			List<RetailerDTO> retailers;
			
			using (var dbContext = new ToolkitEntities())
			{
				var retailerList = dbContext.Program_Retailers
					.Where(r => r.ProgramId == programId)
					.ToList();

				if (!retailerList.Any())
					return new List<RetailerDTO>();

				retailers = retailerList.Select(r => new RetailerDTO(r)).ToList();
			}

			return retailers;
		}

		public List<ProgramDTO> GetProgramList()
		{
			List<ProgramDTO> programs;

			using (var dbContext = new ToolkitEntities())
			{
				var programList = dbContext.Programs
					.ToList();

				if(!programList.Any())
					return new List<ProgramDTO>();

				programs = programList.Select(program => new ProgramDTO(program)).ToList();
			}

			return programs;
		}

		public List<RetailerDTO> GetRetailersByMallId(int mallId)
		{
			List<RetailerDTO> retailers;

			using (var dbContext = new ToolkitEntities())
			{
				var retailerList = dbContext.Program_Retailers
					.Where(r => r.Program_Retailer_Selected_Malls.Any(rm => rm.MallId == mallId))
					.ToList();

				if (!retailerList.Any())
					return new List<RetailerDTO>();

				retailers = retailerList.Select(r => new RetailerDTO(r)).ToList();
			}

			return retailers;
		}

		public ResponseResult RetailerSignUp(RetailerDTO retailerData)
		{
			if (retailerData == null)
				throw new ArgumentNullException("retailerData1");
			
			if(retailerData == null)
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Retailer submitted to update." };

			if(string.IsNullOrEmpty(retailerData.Offer))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Offer details submitted." };

			if(string.IsNullOrEmpty(retailerData.StoreName))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No StoreName submitted." };

			if(retailerData.SelectedMalls == null || !retailerData.SelectedMalls.Any())
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Malls were selected." };
			
			using (var dbContext = new ToolkitEntities())
			{
				var retailer = dbContext.Program_Retailers.SingleOrDefault(pr => pr.Id == retailerData.Id && pr.ProgramId == retailerData.ProgramId);

				if(retailer == null)
					return new ResponseResult(){WasSuccessful = false, FailureReason = "Retailer not found."};

				UpdateRetailerData(retailer, retailerData);

				dbContext.Entry(retailer).State = EntityState.Modified;
				dbContext.SaveChanges();
			}

			return new ResponseResult() { WasSuccessful = true }; 
		}

		private void UpdateRetailerData(Program_Retailers retailer, RetailerDTO retailerData)
		{
			if (retailerData.FieldValues != null && retailerData.FieldValues.Any())
			{
				foreach (var fieldValueDto in retailerData.FieldValues)
				{
					retailer.Program_Field_Values.Add(new Program_Field_Values()
					                                  {
						                                  ProgramFieldId = fieldValueDto.Id,
						                                  ProgramRetailerId = retailer.Id,
						                                  Value = fieldValueDto.Value
					                                  });
				}
			}

			foreach (var mallId in retailerData.SelectedMalls)
			{
				retailer.Program_Retailer_Selected_Malls.Add(new Program_Retailer_Selected_Malls()
				{
					ProgramRetailerId = retailerData.Id,
					MallId = mallId
				});
			}
		}
	}

	public class ResponseResult
	{
		public bool WasSuccessful { get; set; }
		public string FailureReason { get; set; }
	}
}