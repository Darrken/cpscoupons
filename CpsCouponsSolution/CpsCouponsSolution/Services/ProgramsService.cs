using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Management;
using CpsCouponsSolution.DTO;
using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.Services
{
	public class ProgramsService
	{
		public List<MallDTO> GetMallsWithSignups()
		{
			List<MallDTO> mallList;
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Program_Retailer_Selected_Malls.Select(p => new MallDTO
				{
					Id = p.Mall.ID,
					Name = p.Mall.Name,
					StateId = p.Mall.State.ID,
					StateName = p.Mall.State.Name
				}).Distinct();

				mallList = malls.OrderBy(m => m.Name).ToList();
			}

			return mallList;
		}

		public List<MallDTO> GetMallNames(bool isAll)
		{
			List<MallDTO> mallList;
			using (var dbContext = new ToolkitEntities())
			{
				var malls = dbContext.Malls.Select(m => new MallDTO
				{
					Id = m.ID,
					Name = m.Name,
					StateId = m.State.ID,
					StateName = m.State.Name
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

				if (programToRemove == null)
					throw new Exception("Program to delete not found.");

				dbContext.Entry(programToRemove).State = EntityState.Deleted;
				dbContext.SaveChanges();
			};
		}

		public int SaveProgram(ProgramDTO programData)
		{
			if (programData == null)
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

			Program newProgram = new Program();
			using (var dbContext = new ToolkitEntities())
			{
				if (dbContext.Programs.Any(p => p.Name == programData.Name && programData.Id == 0))
				{
					throw new DuplicateNameException();
				}

				if (programData.Id != 0)
				{
					newProgram = dbContext.Programs.Find(programData.Id);
				}

				newProgram.Header = programData.Header;
				newProgram.CouponWordCount = programData.CouponWordCount;
				newProgram.Description = programData.Description;
				newProgram.Disclaimer = programData.Disclaimer;
				newProgram.Name = programData.Name;
				newProgram.DeadlineCoupon = programData.DeadlineCoupon;
				newProgram.DeadlineInMall = programData.DeadlineInMall;

				if (programData.Fields != null && programData.Fields.Any())
				{
					foreach (var fieldDto in programData.Fields)
					{
						if (newProgram.Program_Fields.Any(f => f.Name == fieldDto.Name))
						{
							continue;
						}

						newProgram.Program_Fields.Add(new Program_Fields()
																					{
																						Name = fieldDto.Name,
																						ProgramId = newProgram.Id
																					});
					}
				}

				foreach (var mall in programData.ParticipatingMalls)
				{
					if (newProgram.Malls.Any(m => m.ID == mall.Id))
					{
						continue;
					}

					newProgram.Malls.Add(dbContext.Malls.First(m => m.ID == mall.Id));
				}

				var emailService = new EmailService();
				var retailerInviteEmailsToSend = new List<MailMessage>();
				foreach (var retailerDto in programData.Retailers)
				{
					if (newProgram.Program_Retailers.Any(r => r.Email == retailerDto.Email))
					{
						continue;
					}

					var urlGuid = Guid.NewGuid();
					newProgram.Program_Retailers.Add(new Program_Retailers()
					{
						Email = retailerDto.Email,
						ProgramId = newProgram.Id,
						UrlGuid = urlGuid,
						SubmittedTs = DateTime.UtcNow
					});

					var subject = ConfigurationManager.AppSettings["InviteSubject"];
					var inviteBaseUrl = ConfigurationManager.AppSettings["InviteBaseUrl"];
					var inviteUrl = inviteBaseUrl + urlGuid;
					var replacementDictionary = new Dictionary<string, string> { { "<%inviteUrl%>", inviteUrl } };

					retailerInviteEmailsToSend.Add(emailService.CreateMessage(retailerDto.Email, subject, EmailType.Invite, replacementDictionary));
				}

				if (newProgram.Id == 0)
				{
					dbContext.Programs.Add(newProgram);
				}
				else
				{
					dbContext.Entry(newProgram).State = EntityState.Modified;
				}

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
				var selectedProgram = dbContext.Programs.SingleOrDefault(p => p.Id == programId);

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

			if (!Guid.TryParse(urlGuid, out retailerGuid))
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

				if (!programList.Any())
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
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Retailer submitted to update." };

			if (string.IsNullOrEmpty(retailerData.Offer))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Offer details submitted." };

			if (string.IsNullOrEmpty(retailerData.StoreName))
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No StoreName submitted." };

			if (retailerData.SelectedMalls == null || !retailerData.SelectedMalls.Any())
				return new ResponseResult() { WasSuccessful = false, FailureReason = "No Malls were selected." };

			Program selectedProgram;
			using (var dbContext = new ToolkitEntities())
			{
				selectedProgram = dbContext.Programs.SingleOrDefault(p => p.Id == retailerData.ProgramId);

				var retailer = dbContext.Program_Retailers.SingleOrDefault(pr => pr.Id == retailerData.Id && pr.ProgramId == retailerData.ProgramId);

				if (retailer == null)
					return new ResponseResult() { WasSuccessful = false, FailureReason = "Retailer not found." };

				UpdateRetailerData(retailer, retailerData, dbContext);

				dbContext.Entry(retailer).State = EntityState.Modified;
				dbContext.SaveChanges();
			}

			// after successful signup, send email of successful reservation or possible update
			if (!retailerData.IsAdmin)
				SendReservationConfirmedEmail(retailerData, selectedProgram);

			if (retailerData.IsRetailerEmailNeeded)
				SendRetailerUpdateEmail(retailerData, selectedProgram);

			return new ResponseResult() { WasSuccessful = true };
		}

		private void SendRetailerUpdateEmail(RetailerDTO retailerData, Program selectedProgram)
		{
			var subject = ConfigurationManager.AppSettings["SignedUpUpdateEmailSubject"];
			var replacementDictionary = GetReplacementDictionary(retailerData, selectedProgram);

			SendEmail(retailerData, subject, EmailType.Update, replacementDictionary);
		}

		private void SendReservationConfirmedEmail(RetailerDTO retailerData, Program selectedProgram)
		{
			var subject = ConfigurationManager.AppSettings["SignedUpEmailSubject"];
			var replacementDictionary = GetReplacementDictionary(retailerData, selectedProgram);

			SendEmail(retailerData, subject, EmailType.Confirm, replacementDictionary);
		}

		private Dictionary<string, string> GetReplacementDictionary(RetailerDTO retailerData, Program selectedProgram)
		{
			var replacementDictionary = new Dictionary<string, string>
			                            {
				                            {"<%programName%>", selectedProgram.Name},
				                            {"<%programDescription%>", selectedProgram.Description},
				                            {"<%StoreName%>", retailerData.StoreName},
				                            {"<%Offer%>", retailerData.Offer},
				                            {"<%Restrictions%>", retailerData.Restrictions},
				                            {
					                            "<%MallNames%>", "<table>" + string.Concat(GetMallNames(false)
					                            .Where(m => retailerData.SelectedMalls.Select(mall => mall.Id).Contains(m.Id))
					                            .Select(m => "<tr><td>" + m.Name + "</td></tr>").ToList()) + "</table>"
				                            }
			                            };
			return replacementDictionary;
		}

		private void SendEmail(RetailerDTO retailerData, string subject, EmailType emailType, Dictionary<string, string> replacementDictionary)
		{
			var emailService = new EmailService();

			var emailMsg = emailService.CreateMessage(retailerData.Email, subject, emailType, replacementDictionary);
			emailService.Send(emailMsg);
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
			foreach (var mallDto in retailerData.SelectedMalls)
			{
				if (alreadySelectedMalls.All(m => m.MallId != mallDto.Id))
				{
					retailer.Program_Retailer_Selected_Malls.Add(new Program_Retailer_Selected_Malls()
					{
						ProgramRetailerId = retailerData.Id,
						MallId = mallDto.Id
					});
				}
			}

			var mallsToDelete = alreadySelectedMalls.Where(m => !retailerData.SelectedMalls.Select(mall => mall.Id).Contains(m.MallId)).ToList();
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