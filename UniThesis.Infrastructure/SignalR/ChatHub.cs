using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace UniThesis.Infrastructure.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_user_{userId}");
                _logger.LogInformation("User {UserId} connected to ChatHub", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_user_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            _logger.LogDebug("User joined conversation: {ConversationId}", conversationId);
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            _logger.LogDebug("User left conversation: {ConversationId}", conversationId);
        }

        public async Task SendMessage(Guid conversationId, string content)
        {
            var userId = GetUserId();
            if (userId is null) return;

            // Broadcast to conversation participants
            await Clients.Group($"conversation_{conversationId}").SendAsync("NewMessage", new
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = content,
                SentAt = DateTime.UtcNow
            });
        }

        public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
        {
            var userId = GetUserId();
            if (userId is null) return;

            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                UserId = userId,
                IsTyping = isTyping
            });
        }

        private Guid? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
