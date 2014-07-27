using System;
using System.Collections.Generic;
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

		public int CreateProgram(ProgramDTO programData)
		{
			//using (var dbContext = new ToolkitEntities())
			//{
			//	var newProgram = new Program()
			//						  {
			//							  CouponWordCount = programData.CouponWordCount,
					                 
			//						  }
			//}

			throw new NotImplementedException();
		}
	}
}