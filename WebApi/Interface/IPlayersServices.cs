using WebApi.DTO;
using WebApi.Entities;

namespace WebApi.Interface
{
    public interface IPlayersServices
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player> CreatePlayerAsync(CreatePlayerDto createPlayerDto);
        Task<Player> UpdatePlayerAsync(int id, CreatePlayerDto playerDto);
        Task<Player> DeletePlayerAsync(int id);
    }
}
