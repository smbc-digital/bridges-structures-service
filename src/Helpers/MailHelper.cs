using bridges_structures_service.Models;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.MailingServiceGateway;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Mail;

namespace bridges_structures_service.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IMailingServiceGateway _mailingServiceGateway;

        public MailHelper(IMailingServiceGateway mailingServiceGateway)
        {
            _mailingServiceGateway = mailingServiceGateway;
        }

        public void SendEmail(Person person, EMailTemplate template, string caseReference)
        {
            _mailingServiceGateway.Send(new Mail
            {
                Template = template,
                Payload = JsonConvert.SerializeObject(new {
                    Subject = "Bridges or Structures Report - submission",
                    Reference = caseReference,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    RecipientAddress = person.Email
                })
            });
        }
    }
}