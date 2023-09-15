using WebApi.Entities;

namespace WebApi.Interface
{
    public interface ITeamServices
    {
        Task<List<Player>> SelectTeamsAsync(List<TeamRequirement> requirements);
    }
}
