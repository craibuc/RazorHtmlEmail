using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using RazorHtmlEmail.lib;
using RazorHtmlEmail.Models;
using RazorHtmlEmail.Models.Settings;
using RazorHtmlEmail.Models.ViewModels;

namespace RazorHtmlEmail.Controllers
{

    public class SmtpController : Controller
    {

        private readonly SmtpSettings _smtpSettings;
        private readonly IRazorViewToStringRenderer _renderer;

        public SmtpController(
            IOptions<SmtpSettings> smtpSettings,
            IRazorViewToStringRenderer renderer
        )
        {
            _smtpSettings = smtpSettings.Value;
            _renderer = renderer;
        }

        public async Task SendTemplate()
        {

            // dummy list of people
            List<Person> persons = new List<Person>
            {
                new Person { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@company.com" }
            };

            persons.Add(new Person { FirstName = "Jane", LastName = "Doe", EmailAddress = "jane.doe@company.com" });

            // send a targeted email to each person

            foreach (Person person in persons)
            {
                //
                // TODO: substitute different layout
                //

                //
                // render Razor page as HTML
                //
                var body = await _renderer.RenderViewToStringAsync("/Views/Smtp/Template.cshtml", person);

                // create message
                using (MailMessage mailMessage = new MailMessage
                {
                    // send mail from system account
                    From = new MailAddress(_smtpSettings.SmtpFromAddress, _smtpSettings.SmtpFromName),
                    Subject = "Lorem Ipsum",
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await SendMessage(mailMessage);
                }

            } // foreach

        } // SendTemplate

        // GET: /Smtp/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Smtp/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Recipients,Subject,Body")] SmtpCreateViewModel model)
        {

            if (ModelState.IsValid)
            {
            
                // TODO: allow Markdown to be used in the form's body; convert Markdown to HTML here.

                // create message
                using (MailMessage mailMessage = new MailMessage
                {
                    // send mail from system account
                    From = new MailAddress(_smtpSettings.SmtpFromAddress, _smtpSettings.SmtpFromName),
                    Subject = model.Subject,
                    Body = model.Body, 
                    IsBodyHtml = true
                })
                {

                    // parse field; add recipients
                    foreach (string recipient in model.Recipients.Split(new char[] { ',', ';', ' ' }))
                    {
                        mailMessage.To.Add(new MailAddress(recipient));
                    }

                    try
                    {
                        await SendMessage(mailMessage);

                        @TempData["Message"] = "Message sent successfully.";
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                    }
                }

            }

            // show form
            return View(model);


        } // POST: /Smtp/Create

        public async Task SendMessage(MailMessage mailMessage)
        {
            using (SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpSettings.SmtpServer,
                Port = _smtpSettings.SmtpPort,
                Credentials = new NetworkCredential(_smtpSettings.SmtpAccount, _smtpSettings.SmtpPassword),
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                EnableSsl = _smtpSettings.SmtpUseSsl,
                Timeout = 5 * 1000 // 5,000 milliseconds -> 5 seconds
            })
            {
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch
                {
                    throw;
                }

            } // using

        } // SendMessage

    }

}
