using System.Collections.Generic;

namespace bridges_structures_service.Config
{
    public class BridgesStructuresConfiguration
    {
        public string AffectedStructure { get; set; }
        public string TypeOfRequest { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Classification { get; set; }
    }

    public class BridgesStructuresListConfiguration
    {
        public List<BridgesStructuresConfiguration> BridgesStructuresConfigurations { get; set; }
    }
}