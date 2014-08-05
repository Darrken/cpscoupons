using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace CpsCouponsSolution.Services
{
	public class EmailService
	{
		public MailMessage CreateMessage(string to, string subject, EmailType emailType, Dictionary<string, string> replacementDictionary, string cc = null, string bcc = null)
		{
			var from = ConfigurationManager.AppSettings["EmailFromAddress"];

			var body = GetEmailBody(emailType, replacementDictionary);
			
			var msg = new MailMessage(from, to, subject, body);
			msg.IsBodyHtml = true;

			if (!string.IsNullOrEmpty(cc) && cc.Trim().Length > 0)
				msg.CC.Add(cc);

			if (!string.IsNullOrEmpty(bcc) && bcc.Trim().Length > 0)
				msg.Bcc.Add(bcc);

			return msg;
		}

		private string GetEmailBody(EmailType emailType, Dictionary<string, string> replacments)
		{
			var body = string.Empty;
			var templateName = string.Empty;

			switch (emailType)
			{
				case EmailType.Confirm:
					templateName = "ReservationConfirmation.html";
					break;
				case EmailType.Invite:
					templateName = "ReservationInvite.html";
					break;
				case EmailType.Update:
					templateName = "ReservationUpdate.html";
					break;
			}

			using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/" + templateName)))
			{
				body = reader.ReadToEnd();
			}

			foreach (var replacment in replacments)
			{
				body = body.Replace(replacment.Key, replacment.Value);
			}

			return body;
		}

		public void Send(MailMessage mailMessage)
		{
			var areSmtpCredentialsRequired = Convert.ToBoolean(ConfigurationManager.AppSettings["AreSmtpCredentialsRequired"]);
			var smtpHost = ConfigurationManager.AppSettings["EmailHost"];
			var username = ConfigurationManager.AppSettings["SmtpUsername"];
			var password = ConfigurationManager.AppSettings["SmtpPassword"];
			var isSmtpServerAvailable = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSmtpServerAvailable"]);

			var client = new SmtpClient { Host = smtpHost };
			if (areSmtpCredentialsRequired)
				client.Credentials = new NetworkCredential(username, password);
			
			if(isSmtpServerAvailable)
				client.Send(mailMessage);
		}

		public bool ValidateEmailAddress(string emailAddress)
		{
			// Pattern from: http://www.w3.org/TR/html5/forms.html#valid-e-mail-address
			var pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

			var regex = new Regex(pattern, RegexOptions.IgnoreCase);

			return regex.IsMatch(emailAddress);
		}
	}

	public enum EmailType
	{
		Invite,
		Confirm,
		Update
	}
}