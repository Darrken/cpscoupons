using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
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
					             Name = programData.Name,
									 DeadlineCoupon = programData.DeadlineCoupon,
									 DeadlineInMall = programData.DeadlineInMall
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

				foreach (var mall in programData.ParticipatingMalls)
				{
					newProgram.Malls.Add(dbContext.Malls.First(m => m.ID == mall.Id));
				}

				var emailService = new EmailService();
				var retailerInviteEmailsToSend = new List<MailMessage>();
				foreach (var retailerDto in programData.Retailers)
				{
					var urlGuid = Guid.NewGuid();
					newProgram.Program_Retailers.Add(new Program_Retailers()
					{
						Email = retailerDto.Email,
						ProgramId = newProgram.Id,
						UrlGuid = urlGuid
					});

					var body = ConfigurationManager.AppSettings["InviteBody"];
					var subject = ConfigurationManager.AppSettings["InviteSubject"];
					var inviteBaseUrl = ConfigurationManager.AppSettings["InviteBaseUrl"];

					var inviteUrl = inviteBaseUrl + urlGuid;
					body = body + "\n " + GetButtonHtml(inviteUrl);

					retailerInviteEmailsToSend.Add(emailService.CreateMessage(retailerDto.Email, subject, body));
				}

				dbContext.Programs.Add(newProgram);
				dbContext.SaveChanges();

				// send emails if program creation was successful
				foreach (var email in retailerInviteEmailsToSend)
				{
					emailService.Send(email);
				}
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
			if(retailerData == null)
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Retailer submitted to update." };

			if(string.IsNullOrEmpty(retailerData.Offer))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Offer details submitted." };

			if(string.IsNullOrEmpty(retailerData.StoreName))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No StoreName submitted." };

			if(retailerData.SelectedMalls == null || !retailerData.SelectedMalls.Any())
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Malls were selected." };

			Program selectedProgram;
			using (var dbContext = new ToolkitEntities())
			{
				selectedProgram = dbContext.Programs.SingleOrDefault(p => p.Id == retailerData.ProgramId);
				
				var retailer = dbContext.Program_Retailers.SingleOrDefault(pr => pr.Id == retailerData.Id && pr.ProgramId == retailerData.ProgramId);

				if(retailer == null)
					return new ResponseResult(){WasSuccessful = false, FailureReason = "Retailer not found."};

				UpdateRetailerData(retailer, retailerData, dbContext);

				dbContext.Entry(retailer).State = EntityState.Modified;
				dbContext.SaveChanges();
			}

			// after successful signup, send email of transaction
			SendReservationConfirmedEmail(retailerData, selectedProgram);

			return new ResponseResult() { WasSuccessful = true }; 
		}

		private void SendReservationConfirmedEmail(RetailerDTO retailerData, Program selectedProgram)
		{
			var emailService = new EmailService();
			var subject = ConfigurationManager.AppSettings["SignedUpEmailSubject"];
			var body = ConfigurationManager.AppSettings["SignedUpEmailBody"];

			body = body + "\n\n" + "Program name: \n\t" + selectedProgram.Name;
			body = body + "\n" + "Program description: \n\t" + selectedProgram.Description;
			body = body + "\n" + "Store name: \n\t" + retailerData.StoreName;
			body = body + "\n" + "Offer: \n\t" + retailerData.Offer;
			body = body + "\n" + "Restrictions: \n\t" +retailerData.Restrictions;
			body = body + "\n" + "Selected Malls: \n\t" + string.Concat(GetMallNames(false)
																		.Where(m => retailerData.SelectedMalls.Contains(m.Id))
																		.Select(m => m.Name + "\n\t").ToList());
			body = body + "\n Thank you for your participation.";
			body = body + "\n\n Mall Marketing Media.";
			body = body + "\n a division of.";
			body = body + "\n creative publishing solutions.";


			var emailMsg = emailService.CreateMessage(retailerData.Email, subject, body);
			emailService.Send(emailMsg);
		}

		private string GetButtonHtml(string inviteUrl)
		{
			return @"<div>
										<!--[if mso]>
											<v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href='" + inviteUrl + @"' style=""height:40px;v-text-anchor:middle;width:300px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#3071a9"">
												<w:anchorlock/>
												<center style=""color:#ffffff;font-family:sans-serif;font-size:16px;font-weight:bold;"">
												Button Text Here!
												</center>
											</v:roundrect>
											<![endif]-->
											<![if !mso]>
											<table cellspacing=""0"" cellpadding=""0""> <tr> 
											<td align=""center"" width=""300"" height=""40"" bgcolor=""#3071a9"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;"">
												<a href='" + inviteUrl +@"' style=""font-size:16px; font-weight: bold; font-family:sans-serif; text-decoration: none; line-height:40px; width:100%; display:inline-block"">
												<span style=""color: #ffffff;"">
												Click to start Reservation
												</span>
												</a>
											</td> 
											</tr> </table> 
											<![endif]>
										</div>
										";
		}

		private void UpdateRetailerData(Program_Retailers retailer, RetailerDTO retailerData, ToolkitEntities dbContext)
		{
			retailer.Offer = retailerData.Offer;
			retailer.StoreName = retailerData.StoreName;
			retailer.Restrictions = retailerData.Restrictions;
			retailer.Address = retailerData.Address;
			retailer.City = retailerData.City;
			retailer.ContactName = retailerData.ContactName;
			retailer.Phone = retailerData.Phone;
			retailer.RepName = retailerData.RepName;
			retailer.State = retailerData.State;
			retailer.Zip = retailerData.Zip;
			
			if (retailerData.FieldValues != null && retailerData.FieldValues.Any())
			{
				foreach (var fieldValueDto in retailerData.FieldValues)
				{
					var existingFieldValue = dbContext.Program_Field_Values.SingleOrDefault(
																									fv => fv.ProgramFieldId == fieldValueDto.Id
																									&& fv.ProgramRetailerId == retailerData.Id);
					if (existingFieldValue != null)
					{
						existingFieldValue.Value = fieldValueDto.Value;
					}
					else
					{
						retailer.Program_Field_Values.Add(new Program_Field_Values()
						{
							ProgramFieldId = fieldValueDto.Id,
							ProgramRetailerId = retailer.Id,
							Value = fieldValueDto.Value
						});	
					}
				}
			}
			
			var alreadySelectedMalls = dbContext.Program_Retailer_Selected_Malls.Where(rsm => rsm.ProgramRetailerId == retailerData.Id).ToList();
			foreach (var mallId in retailerData.SelectedMalls)
			{
				if (alreadySelectedMalls.All(m => m.MallId != mallId))
				{
					retailer.Program_Retailer_Selected_Malls.Add(new Program_Retailer_Selected_Malls()
					{
						ProgramRetailerId = retailerData.Id,
						MallId = mallId
					});	
				}
			}

			var mallsToDelete = alreadySelectedMalls.Where(m => !retailerData.SelectedMalls.Contains(m.MallId)).ToList();
			foreach (var mallToDelete in mallsToDelete)
			{
				dbContext.Entry(mallToDelete).State = EntityState.Deleted;
			}
		}
	}

	public class ResponseResult
	{
		public bool WasSuccessful { get; set; }
		public string FailureReason { get; set; }
	}
}