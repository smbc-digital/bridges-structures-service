using bridges_structures_service.Models;
using StockportGovUK.NetStandard.Models.Enums;

namespace bridges_structures_service.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(Person person, EMailTemplate template, string caseReference);
    }
}
