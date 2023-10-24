using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Your Name Must Be At Least {2},And Maximum {1} Characters")]
        public string Name { get; set; }
    }
}
