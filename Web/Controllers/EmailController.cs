﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Models;

namespace Web.Controllers
{
    [EnableCors("EmailPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IOptions<EmailConfiguration> emailConfiguration;

        public EmailController(IOptions<EmailConfiguration> emailConfiguration)
        {
            this.emailConfiguration = emailConfiguration;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Email email)
        {
            bool success = SendEmail(email.EmailAddress, email.Subject, email.Message);

            if (success)
            {
                return Ok();
            }

            return Content("Failed Sending Reminder Email");
        }

        private bool SendEmail(string emailAddress, string subject, string message)
        {
            MailMessage mm = new MailMessage
            {
                From = new MailAddress("test@gmail.com", emailConfiguration.Value.NetworkCredName),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            mm.To.Add(new MailAddress(emailAddress));
            
            NetworkCredential networkCred = new NetworkCredential
            {
                UserName = emailConfiguration.Value.NetworkCredUserName,
                Password = emailConfiguration.Value.NetworkCredPassword,
            };

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = networkCred,
                Port = 587
            };

            try
            {
                smtp.Send(mm);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}