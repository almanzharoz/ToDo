using System.Runtime.Serialization;

namespace Expo3.Model.Embed
{
    public enum EUserRole : byte
    {
        [EnumMember(Value="admin")]
        Admin = 1,
        [EnumMember(Value = "organizer")]
        Organizer,
        [EnumMember(Value = "user")]
        User
    }
}