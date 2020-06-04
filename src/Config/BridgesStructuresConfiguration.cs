using System.Collections.Generic;
using System.Security.Policy;

namespace bridges_structures_service.Config
{
    public class BridgesStructuresConfiguration
    {
        public ClassificationMap ClassificationMap { get; set; }
    }

    public class ClassificationMap
    {
        public List<Structure> Structures { get; set; }
    }

    public class Structure
    {
        public string AffectedStructure { get; set; }
        public string TypeOfRequest { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BridgesStructuresListConfiguration
    {
        public List<BridgesStructuresConfiguration> BridgesStructuresConfigurations { get; set; }
    }
}