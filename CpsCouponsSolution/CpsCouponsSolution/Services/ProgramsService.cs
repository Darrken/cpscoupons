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

		public void DeleteProgram(int programId)
		{
			using (var dbContext = new ToolkitEntities())
			{
				var programToRemove = dbContext.Programs.SingleOrDefault(p => p.Id == programId);
				
				dbContext.Entry(programToRemove).State = EntityState.Deleted;
				dbContext.SaveChanges();
			};
		}

		public int CreateProgram(ProgramDTO programData)
		{
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

				foreach (var fieldDto in programData.Fields)
				{
					newProgram.Program_Fields.Add(new Program_Fields()
					{
						Name = fieldDto.Name,
						ProgramId = newProgram.Id
					});
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

				programDto = new ProgramDTO(selectedProgram);
			}

			return programDto;
		}

		public ProgramDTO GetProgramByRetailerGuId(string urlGuid)
		{
			ProgramDTO programDto;
			var retailerGuid = Guid.Parse(urlGuid);

			using (var dbContext = new ToolkitEntities())
			{
				var selectedProgram = dbContext.Programs.SingleOrDefault(p => p.Program_Retailers.Any(r => r.UrlGuid == retailerGuid));

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

				retailers = retailerList.Select(r => new RetailerDTO(r)).ToList();
			}

			return retailers;
		}

		public SignUpResult RetailerSignUp(ProgramDTO programData)
		{
			using (var dbContext = new ToolkitEntities())
			{
				var retailerData = programData.Retailers.First();
				var retailer = dbContext.Program_Retailers.SingleOrDefault(pr => pr.Id == retailerData.Id && pr.ProgramId == programData.Id);

				if(retailer == null)
					return new SignUpResult(){WasSuccessful = false, FailureReason = "Retailer not found."};

				UpdateRetailerData(retailer, retailerData);

				dbContext.Entry(retailer).State = EntityState.Modified;
				dbContext.SaveChanges();
			}

			return new SignUpResult() { WasSuccessful = true }; 
		}

		private void UpdateRetailerData(Program_Retailers retailer, RetailerDTO retailerData)
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

	public class SignUpResult
	{
		public bool WasSuccessful { get; set; }
		public string FailureReason { get; set; }
	}
}