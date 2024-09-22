namespace HomeScreen.Web.Common.Server.Entities;

public class PaginatedMediaItem
{
    public required MediaItem MediaItem { get; set; }
public required ulong TotalPages { get; set; }
}
