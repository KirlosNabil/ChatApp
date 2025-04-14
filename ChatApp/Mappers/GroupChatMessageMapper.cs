using ChatApp.DTOs;
using ChatApp.Models;

namespace ChatApp.Mappers
{
    public static class GroupChatMessageMapper
    {
        public static GroupChatMessageDTO ToDTO(GroupChatMessage message, UserDTO userDTO)
        {
            return new GroupChatMessageDTO
            {
                GroupId = message.GroupId,
                MessageId = message.Id,
                MessageContent = message.Message,
                SenderId = message.Sender.Id,
                SenderFullName = userDTO.FirstName + " " + userDTO.LastName,
                SenderProfilePicturePath = userDTO.ProfilePicturePath
            };
        }
    }
}
