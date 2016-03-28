using System;
using System.Xml.Serialization;

namespace HoloXPLOR.Data.DataForge
{
    [XmlRoot(ElementName = "BTInput_Ship_TurnToTarget_Target_BB")]
    public partial class BTInput_Ship_TurnToTarget_Target_BB : BTInput_Ship_TurnToTarget_Target
    {
        [XmlAttribute(AttributeName = "bbPath")]
        public String BbPath { get; set; }

    }
}
