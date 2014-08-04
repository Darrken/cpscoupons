﻿using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

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
}