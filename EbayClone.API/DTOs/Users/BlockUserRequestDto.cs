using System.ComponentModel.DataAnnotations;

namespace EbayClone.API.DTOs.Users;

public class BlockUserRequestDto
{
    [Required, MinLength(3)]
    public string Reason { get; set; } = string.Empty;
}
