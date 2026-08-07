using System.ComponentModel.DataAnnotations;
using RagSystemFrontend.Core.DTOs;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class ChatIndexViewModel
{
    public List<CollectionDto> Collections { get; set; } = [];
}

public class ChatSendViewModel
{
    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string Question { get; set; } = "";

    public string? ConversationId { get; set; }

    public string? CollectionId { get; set; }
}

public class ChatFeedbackViewModel
{
    public long MessageId { get; set; }

    [Range(-1, 1)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}
