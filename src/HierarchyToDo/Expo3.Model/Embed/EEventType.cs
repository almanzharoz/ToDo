using System.Runtime.Serialization;

namespace Expo3.Model.Embed
{
    public enum EEventType
    {
        [EnumMember(Value = "concert")]
        Concert,
        [EnumMember(Value = "exhibition")]
        Exhibition,
        [EnumMember(Value = "excursion")]
        Excursion
    }
}