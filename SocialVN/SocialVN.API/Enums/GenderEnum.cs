using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Enums
{
    public enum GenderEnum
    {
        [Display(Name = "Nam")]
        Male,

        [Display(Name = "Nữ")]
        Female,

        [Display(Name = "Khác")]
        Other
    }
}
