using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Common.Server.Entities;

public class PaginatedMediaItem
{
    public required MediaItem MediaItem { get; set; }

    [Required]
    public required ulong TotalPages { get; set; }
}
