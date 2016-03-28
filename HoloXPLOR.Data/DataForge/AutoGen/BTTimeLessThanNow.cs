using System;
using System.Xml.Serialization;

namespace HoloXPLOR.Data.DataForge
{
    [XmlRoot(ElementName = "BTTimeLessThanNow")]
    public partial class BTTimeLessThanNow : BTNode
    {
        [XmlArray(ElementName = "Timestamp")]
        [XmlArrayItem(Type = typeof(BTInputTimestamp))]
        [XmlArrayItem(Type = typeof(BTInputTimestampVar))]
        [XmlArrayItem(Type = typeof(BTInputTimestampBB))]
        public BTInputTimestamp[] Timestamp { get; set; }

        [XmlArray(ElementName = "Interval")]
        [XmlArrayItem(Type = typeof(BTInputFloat))]
        [XmlArrayItem(Type = typeof(BTInputFloatValue))]
        [XmlArrayItem(Type = typeof(BTInputFloatVar))]
        [XmlArrayItem(Type = typeof(BTInputFloatBB))]
        public BTInputFloat[] Interval { get; set; }

    }
}
