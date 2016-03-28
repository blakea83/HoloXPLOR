using System;
using System.Xml.Serialization;

namespace HoloXPLOR.Data.DataForge
{
    [XmlRoot(ElementName = "BTInput_Character_ExactGoto_Speed_String")]
    public partial class BTInput_Character_ExactGoto_Speed_String : BTInput_Character_ExactGoto_Speed
    {
        [XmlAttribute(AttributeName = "value")]
        public BehaviorTree_CharacterSpeed Value { get; set; }

    }
}
