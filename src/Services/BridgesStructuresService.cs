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
using System.Threading.Tasks;

namespace bridges_structures_service.Services
{
    public class BridgesStructuresService : IBridgesStructuresService
    {
        private readonly IVerintServiceGateway _VerintServiceGateway;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BridgesStructuresService> _logger;
        private readonly IOptions<BridgesStructuresListConfiguration> _bridgesStructuresConfig;
        private readonly IMailHelper _mailHelper;

        public BridgesStructuresService(IVerintServiceGateway verintServiceGateway
                                        , IConfiguration iConfig
                                        , ILogger<BridgesStructuresService> logger
                                        , IOptions<BridgesStructuresListConfiguration> bridgesStructuresConfig
                                        , IMailHelper mailHelper)
        {
            _VerintServiceGateway = verintServiceGateway;
            _configuration = iConfig;
            _logger = logger;
            _bridgesStructuresConfig = bridgesStructuresConfig;
            _mailHelper = mailHelper;
        }

        public async Task<string> CreateCase(BridgesStructuresReport bridgesStructuresReport)
        {
            Case crmCase = CreateCrmCaseObject(bridgesStructuresReport);

            try
            {
                StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string> response = await _VerintServiceGateway.CreateCase(crmCase);

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

                _mailHelper.SendEmail(person, EMailTemplate.BridgesStructuresReport, response.ResponseContent);
                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateBridgesOrStructuresService an exception has occured while creating the case in verint service", ex);
            }
        }

        private Case CreateCrmCaseObject(BridgesStructuresReport bridgesStructuresReport)
        {
            List<BridgesStructuresConfiguration> events = _bridgesStructuresConfig.Value.BridgesStructuresConfigurations;

            BridgesStructuresConfiguration bridgesStructuresInfo = new BridgesStructuresConfiguration();
            foreach (BridgesStructuresConfiguration e in events)
            {
                if (e.AffectedStructure == bridgesStructuresReport.StructureAffected &&
                    e.TypeOfRequest == bridgesStructuresReport.TypeOfRequest)
                {
                    bridgesStructuresInfo.AffectedStructure = e.AffectedStructure;
                    bridgesStructuresInfo.TypeOfRequest = e.TypeOfRequest;
                    bridgesStructuresInfo.Code = e.Code;
                    bridgesStructuresInfo.Name = e.Name;
                    bridgesStructuresInfo.Classification = e.Classification;
                    break;
                }
            }

            Case crmCase = new Case
            {
                EventCode = int.Parse(bridgesStructuresInfo.Code),
                EventTitle = _configuration.GetSection("CrmCaseSettings").GetSection("EventTitle").Value,
                Classification = bridgesStructuresInfo.Classification,
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
            //For code review: Prefer this way or the other
            //StringBuilder description = new StringBuilder();
            //description.Append($"Enquiry Subject: {bridgesStructuresReport.GeneralEnquiry}");
            //description.Append(Environment.NewLine);
            //description.Append($"Damage additional information: {bridgesStructuresReport.Details}");
            //description.Append(Environment.NewLine);
            //description.Append($"Location additional information: {bridgesStructuresReport.FurtherInformation}");
            //return description.ToString();

            string description = $"Enquiry Subject: {bridgesStructuresReport.GeneralEnquiry} " +
                $"\nDamage additional information: {bridgesStructuresReport.Details} " +
                $"\nLocation additional information: {bridgesStructuresReport.FurtherInformation}";

            return description;
        }
    }
}
