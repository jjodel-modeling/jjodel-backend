using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using jjodel_persistence.Models.Settings;
using jjodel_persistence.Models.Mail;

namespace jjodel_persistence.Services {

    public class MailService {

        private readonly ILogger<MailService> _logger;
        private readonly MailSettings _mailSettings;
        private readonly IFluentEmailFactory _email;


        public MailService(
            ILogger<MailService> logger,
            IOptions<MailSettings> mailSettings,
            IFluentEmailFactory email
            ) {
            _logger = logger;
            _mailSettings = mailSettings.Value;
            _email = email;
            
            // https://github.com/lukencode/FluentEmail
        }


        public async Task<bool> SendEmail(string from, List<string> to, string subject, string templateName, object viewModel, string language) {
            try {
                // result.
                SendResponse res = null;          

                // create factory
                var factory = _email.Create()
                    .SetFrom(from)
                    .To(this.Convert(to))
                    .BCC("a.perelli@capoweb.it") //TODO: remove.
                    .Subject(subject);

                factory.UsingTemplateFromFile(
                    Directory.GetCurrentDirectory() + 
                    "/Templates/" + 
                    templateName + 
                    "_" + 
                    language + 
                    ".cshtml", viewModel);             

                // send mail.
                res = await factory.SendAsync();
                if (res != null && res.Successful) {
                    return true;
                }
                else {
                    _logger.LogError($"Send mail error: {string.Join(",", res.ErrorMessages)}");
                    _logger.LogError($"Available directories: {string.Join(",", Directory.GetDirectories(Directory.GetCurrentDirectory()).ToList())}");
                }

            }
            catch(Exception ex) {
                _logger.LogError($"Send mail error: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> SendEmail(List<string> to, string subject, string templateName, object viewModel) {

            return await this.SendEmail(_mailSettings.FromDefault, to, subject, templateName, viewModel, "it");

        }




        #region Convert
        private List<FluentEmail.Core.Models.Address> Convert(List<string> to) {

            List<FluentEmail.Core.Models.Address> res = new List<FluentEmail.Core.Models.Address>();
            foreach (string t in to) {
                res.Add(
                    new FluentEmail.Core.Models.Address() {
                        EmailAddress = t
                    });
            }

            return res;

        }

        #endregion

    }
}
