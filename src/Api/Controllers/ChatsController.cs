using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IChatGroupMemberService _chatGroupMemberService;
        private readonly IChatMessageService _chatMessageService;

        public ChatsController(
            IChatGroupService chatGroupService,
            IChatGroupMemberService chatGroupMemberService,
            IChatMessageService chatMessageService)
        {
            _chatGroupService = chatGroupService;
            _chatGroupMemberService = chatGroupMemberService;
            _chatMessageService = chatMessageService;
        }

        // GET: api/chats/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserChats(Guid userId)
        {
            var memberGroups = await _chatGroupMemberService.Where(m => m.UserId == userId)
                .Include(m => m.Group)
                    .ThenInclude(g => g!.Trip)
                .Include(m => m.Group)
                    .ThenInclude(g => g!.Members)
                .ToListAsync();

            var result = memberGroups.Select(m => new
            {
                id = m.GroupId,
                groupName = m.Group?.GroupName,
                avatar = m.Group?.Avatar,
                tripId = m.Group?.TripId,
                trip = m.Group?.Trip != null ? new
                {
                    title = m.Group.Trip.Title,
                    image = m.Group.Trip.Image
                } : null,
                memberCount = m.Group?.Members?.Count ?? 0,
                lastActivity = m.Group?.UpdatedAt ?? m.Group?.CreatedAt,
                userRole = m.Role
            });

            return Ok(result);
        }

        // GET: api/chats/{groupId}
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetChat(Guid groupId)
        {
            var group = await _chatGroupService.Where(g => g.Id == groupId)
                .Include(g => g.Trip)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound(new { message = "Chat group not found" });

            var result = new
            {
                id = group.Id,
                groupName = group.GroupName,
                avatar = group.Avatar,
                tripId = group.TripId,
                trip = group.Trip != null ? new
                {
                    title = group.Trip.Title,
                    image = group.Trip.Image,
                    dateStart = group.Trip.DateStart,
                    dateEnd = group.Trip.DateEnd
                } : null,
                members = group.Members?.Select(m => new
                {
                    userId = m.UserId,
                    name = m.User?.Name,
                    avatar = m.User?.Avatar,
                    role = m.Role,
                    joinedAt = m.JoinedAt
                }).ToList(),
                createdAt = group.CreatedAt
            };

            return Ok(result);
        }

        // GET: api/chats/{groupId}/messages
        [HttpGet("{groupId}/messages")]
        public async Task<IActionResult> GetMessages(Guid groupId, [FromQuery] int limit = 50)
        {
            var messages = await _chatMessageService.Where(m => m.GroupId == groupId)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();

            var result = messages.OrderBy(m => m.CreatedAt).Select(m => new
            {
                id = m.Id,
                senderId = m.SenderId,
                senderName = m.Sender?.Name,
                senderAvatar = m.Sender?.Avatar,
                text = m.Text,
                attachmentUrl = m.AttachmentUrl,
                attachmentType = m.AttachmentType,
                createdAt = m.CreatedAt
            });

            return Ok(result);
        }

        // POST: api/chats
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var group = new ChatGroup
            {
                Id = Guid.NewGuid(),
                GroupName = request.GroupName,
                Avatar = request.Avatar,
                TripId = request.TripId ?? Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _chatGroupService.AddAsync(group);

            // Add creator as admin
            var member = new ChatGroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = request.CreatorId,
                Role = "admin",
                JoinedAt = DateTime.UtcNow
            };

            await _chatGroupMemberService.AddAsync(member);

            return CreatedAtAction(nameof(GetChat), new { groupId = group.Id }, group);
        }

        // POST: api/chats/{groupId}/messages
        [HttpPost("{groupId}/messages")]
        public async Task<IActionResult> SendMessage(Guid groupId, [FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify user is member
            if (!await _chatGroupMemberService.AnyAsync(m => m.GroupId == groupId && m.UserId == request.SenderId))
                return Forbid();

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                SenderId = request.SenderId,
                Text = request.Text,
                AttachmentUrl = request.AttachmentUrl,
                AttachmentType = request.AttachmentType,
                CreatedAt = DateTime.UtcNow
            };

            await _chatMessageService.AddAsync(message);

            // Update group last activity
            var group = await _chatGroupService.Where(g => g.Id == groupId).FirstOrDefaultAsync();
            if (group != null)
            {
                group.UpdatedAt = DateTime.UtcNow;
                await _chatGroupService.UpdateAsync(group);
            }

            return Ok(message);
        }

        // POST: api/chats/{groupId}/members
        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(Guid groupId, [FromBody] AddMemberRequest request)
        {
            // Check if already a member
            if (await _chatGroupMemberService.AnyAsync(m => m.GroupId == groupId && m.UserId == request.UserId))
                return BadRequest(new { message = "User is already a member" });

            var member = new ChatGroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                UserId = request.UserId,
                Role = "member",
                JoinedAt = DateTime.UtcNow
            };

            await _chatGroupMemberService.AddAsync(member);
            return Ok(member);
        }

        // DELETE: api/chats/{groupId}/members/{userId}
        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            var member = await _chatGroupMemberService.Where(m => m.GroupId == groupId && m.UserId == userId)
                .FirstOrDefaultAsync();

            if (member == null)
                return NotFound(new { message = "Member not found" });

            await _chatGroupMemberService.RemoveAsync(member);
            return NoContent();
        }
    }

    public class CreateChatRequest
    {
        public required string GroupName { get; set; }
        public string? Avatar { get; set; }
        public Guid? TripId { get; set; }
        public required Guid CreatorId { get; set; }
    }

    public class SendMessageRequest
    {
        public required Guid SenderId { get; set; }
        public required string Text { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }
    }

    public class AddMemberRequest
    {
        public required Guid UserId { get; set; }
    }
}
