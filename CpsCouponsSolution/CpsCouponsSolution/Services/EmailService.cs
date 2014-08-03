using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace CpsCouponsSolution.Services
{
	public class EmailService
	{
		public MailMessage CreateMessage(string to, string subject, string body, string cc = null, string bcc = null)
		{
			var from = ConfigurationManager.AppSettings["EmailFromAddress"];
			var msg = new MailMessage(from, to, subject, body);
			
			if (!string.IsNullOrEmpty(cc) && cc.Trim().Length > 0)
				msg.CC.Add(cc);

			if (!string.IsNullOrEmpty(bcc) && bcc.Trim().Length > 0)
				msg.Bcc.Add(bcc);

			return msg;
		}

		public void Send(MailMessage mailMessage)
		{
			var areSmtpCredentialsRequired = Convert.ToBoolean(ConfigurationManager.AppSettings["AreSmtpCredentialsRequired"]);
			var smtpHost = ConfigurationManager.AppSettings["EmailHost"];
			var username = ConfigurationManager.AppSettings["SmtpUsername"];
			var password = ConfigurationManager.AppSettings["SmtpPassword"];

			var client = new SmtpClient { Host = smtpHost };
			if (areSmtpCredentialsRequired)
				client.Credentials = new NetworkCredential(username, password);

			client.Send(mailMessage);
		}

		public bool ValidateEmailAddress(string emailAddress)
		{
			try
			{
				var testAddress = new MailAddress(emailAddress);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}