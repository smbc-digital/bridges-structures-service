using bridges_structures_service.Config;
using bridges_structures_service.Helpers;
using bridges_structures_service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bridges_structures_service.Services
{
    public class BridgesStructuresService :IBridgesStructuresService
    {
        private readonly IVerintServiceGateway _VerintServiceGateway;
        private readonly IConfiguration configuration;
        private readonly ILogger<BridgesStructuresService> _logger;
        private readonly IOptions<BridgesStructuresListConfiguration> _bridgesStructuresConfig;
       // private readonly IMailHelper _mailHelper;

        public BridgesStructuresService(IVerintServiceGateway verintServiceGateway
                                        , IConfiguration iConfig
                                        , ILogger<BridgesStructuresService> logger
                                        , IOptions<BridgesStructuresListConfiguration> bridgesStructuresConfig)
                                        //, IMailHelper mailHelper)
        {
            _VerintServiceGateway = verintServiceGateway;
            configuration = iConfig;
            _logger = logger;
            _bridgesStructuresConfig = bridgesStructuresConfig;
            //_mailHelper = mailHelper;
        }

        public async Task<string> CreateCase(BridgesStructuresReport bridgesStructuresReport)
        {
            var crmCase = CreateCrmCaseObject(bridgesStructuresReport);

            try
            {
                var response = await _VerintServiceGateway.CreateCase(crmCase);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Status code not successful");
                }

                Person person = new Person
                {
                    FirstName = bridgesStructuresReport.FirstName,
                    LastName = bridgesStructuresReport.LastName,
                    Email = bridgesStructuresReport.Email,
                    Phone = bridgesStructuresReport.Phone,
                };

                //TODO
                //_mailHelper.SendEmail(person, EMailTemplate.BridgesStructuresReport, response.ResponseContent);
                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateBridgesOrStructuresService an exception has occured while creating the case in verint service", ex);
            }
        }

        private Case CreateCrmCaseObject(BridgesStructuresReport bridgesStructuresReport)
        {
            var events = _bridgesStructuresConfig.Value.BridgesStructuresConfigurations;
            //foreach (var item in events)
            //{

            //}

            //List<Structure> test = events.Where(e => e.ClassificationMap.Structures);

            //check that affected strcuture equals to event Name, then use the event code for that event name
            
            //var eventCode = events.FirstOrDefault(_ => _.EventName == bridgesStructuresReport.StructureAffected).EventCode;

            var crmCase = new Case
            {
                //EventCode = eventCode,
                EventTitle = configuration.GetSection("CrmCaseSettings").GetSection("EventTitle").Value,
                Classification = configuration.GetSection("CrmCaseSettings").GetSection("Classification").Value,
                Description = GenerateDescription(bridgesStructuresReport),
                Street = new Street
                {
                    Reference = bridgesStructuresReport.StreetAddress.PlaceRef
                }
            };

            if (!string.IsNullOrEmpty(bridgesStructuresReport.FirstName) && !string.IsNullOrEmpty(bridgesStructuresReport.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = bridgesStructuresReport.FirstName,
                    Surname = bridgesStructuresReport.LastName
                };

                if (!string.IsNullOrEmpty(bridgesStructuresReport.Email))
                {
                    crmCase.Customer.Email = bridgesStructuresReport.Email;
                }

                if (!string.IsNullOrEmpty(bridgesStructuresReport.Phone))
                {
                    crmCase.Customer.Telephone = bridgesStructuresReport.Phone;
                }
            }
            _logger.LogInformation(JsonConvert.SerializeObject(crmCase));
            return crmCase;
        }

        private string GenerateDescription(BridgesStructuresReport bridgesStructuresReport)
        {
            //StringBuilder description = new StringBuilder();
            //description.Append($"Enquiry Subject: {bridgesStructuresReport.GeneralEnquiry}");
            //description.Append(Environment.NewLine);
            //description.Append($"Damage additional information: {bridgesStructuresReport.Details}");
            //description.Append(Environment.NewLine);
            //description.Append($"Location additional information: {bridgesStructuresReport.FurtherInformation}");

            //return description.ToString();

            var description = $"Enquiry Subject: {bridgesStructuresReport.GeneralEnquiry} " +
                $"\nDamage additional information: {bridgesStructuresReport.Details} " +
                $"\nLocation additional information: {bridgesStructuresReport.FurtherInformation}";

            return description;
        }
    }
}
