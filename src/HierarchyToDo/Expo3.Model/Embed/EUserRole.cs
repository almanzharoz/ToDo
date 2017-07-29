using System.Runtime.Serialization;

namespace Expo3.Model.Embed
{
    public enum EUserRole
    {
        [EnumMember(Value="admin")]
        Admin,
        [EnumMember(Value = "organizer")]
        Organizer,
        [EnumMember(Value = "user")]
        User
    }
}