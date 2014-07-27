using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
	}
}