using System.ComponentModel;
using System.Runtime.Serialization;

namespace SuperChecker.Core.Enum
{
    public enum Quarter
    {
        [EnumMember]
        [Description("Jan-March")]
        JanToMarch = 1,

        [EnumMember]
        [Description("Apr-June")]
        AprilToJune = 2,

        [EnumMember]
        [Description("Jul-Sept")]
        JulyToSept = 3,

        [EnumMember]
        [Description("Oct-Dec")]
        OctToDec = 4,
    }
}
