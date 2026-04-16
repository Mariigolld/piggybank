using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDto>> GetUserGroupsAsync(string userId);
        Task<GroupDetailsDto> GetGroupDetailsAsync(int groupId, string userId);
        Task<GroupDto> CreateAsync(CreateGroupDto dto, string userId);
        Task DeleteAsync(int groupId, string userId);
        Task<GroupDto> JoinByCodeAsync(string inviteCode, string userId);
        Task<GroupSettlementsDto> CalculateSettlementsAsync(int groupId, string userId);
        Task<GroupSettlementDto> RecordSettlementAsync(CreateGroupSettlementDto dto, string userId);
        Task<IEnumerable<GroupSettlementDto>> GetGroupSettlementsAsync(int groupId, string userId);
    }
}