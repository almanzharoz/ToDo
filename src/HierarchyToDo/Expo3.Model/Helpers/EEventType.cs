using System.Runtime.Serialization;

namespace Expo3.Model.Helpers
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