using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;

namespace bridges_structures_service.Models
{
    public class BridgesStructuresReport
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Details { get; set; }
        public string GeneralEnquiry { get; set; }
        public string FurtherInformation { get; set; }
        public string StructureAffected { get; set; }
        public string TypeOfRequest { get; set; }
        public Address StreetAddress { get; set; }
    }
}
