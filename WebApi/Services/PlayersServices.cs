using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Interface;

namespace WebApi.Services
{
    public class PlayersServices : IPlayersServices
    {
        private readonly DataContext _context;
        private readonly IValidServices _validServices;

        public PlayersServices(DataContext context, IValidServices validServices)
        {
            _context = context;
            _validServices = validServices;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            var players = await _context.Players.Include(p => p.PlayerSkills).ToListAsync();

            if (players == null || !players.Any())
            {
                throw new ValidationException("players", "No players found.");

            }

            return players;
        }
        public async Task<Player> UpdatePlayerAsync(int id, CreatePlayerDto playerDto)
        {
            if (playerDto == null)
            {
                throw new ValidationException("request", "Invalid request data.");
            }

            Player existingPlayer = await _context.Players.Include(p => p.PlayerSkills).FirstOrDefaultAsync(p => p.Id == id);

            if (existingPlayer == null)
            {
                throw new ValidationException("Player not found.", "");
            }

            // Use existing player's values as defaults
            if (string.IsNullOrWhiteSpace(playerDto.Name))
            {
                playerDto.Name = existingPlayer.Name;
            }

            if (string.IsNullOrWhiteSpace(playerDto.Position))
            {
                playerDto.Position = existingPlayer.Position;
            }

            if (playerDto.PlayerSkills == null || !playerDto.PlayerSkills.Any())
            {
                // If no skills provided, use existing skills
                playerDto.PlayerSkills = existingPlayer.PlayerSkills.Select(skill => new PlayerSkillDto
                {
                    Skill = skill.Skill,
                    Value = skill.Value
                }).ToList();
            }

            if (!_validServices.IsValidPlayer(new Player
            {
                Name = playerDto.Name,
                Position = playerDto.Position,
                PlayerSkills = playerDto.PlayerSkills?.Select(skillDto => new PlayerSkill
                {
                    Skill = skillDto.Skill,
                    Value = skillDto.Value
                }).ToList()
            }))
            {
                // Handle validation errors here if needed
                throw new ValidationException("player", "Invalid player data.");
            }

            existingPlayer.Name = playerDto.Name;
            existingPlayer.Position = playerDto.Position;
            existingPlayer.PlayerSkills = playerDto.PlayerSkills?.Select(skillDto => new PlayerSkill
            {
                Skill = skillDto.Skill,
                Value = skillDto.Value
            }).ToList();

            await _context.SaveChangesAsync();

            return existingPlayer;
        }


        public async Task<Player> CreatePlayerAsync(CreatePlayerDto createPlayerDto)
        {
            if (createPlayerDto == null)
            {
                throw new ValidationException("request", "Invalid request data.");
            }

            var player = new Player
            {
                Name = createPlayerDto.Name,
                Position = createPlayerDto.Position,
                PlayerSkills = createPlayerDto.PlayerSkills?.Select(skillDto => new PlayerSkill
                {
                    Skill = skillDto.Skill,
                    Value = skillDto.Value
                }).ToList()
            };

            if (!_validServices.IsValidPlayer(player))
            {
                if (string.IsNullOrWhiteSpace(player.Position))
                {
                    throw new ValidationException("position", "Position is required.");
                }
                if (!_validServices.IsValidPosition(player.Position))
                {
                    throw new ValidationException("position", $"Invalid value for position: {player.Position}");
                }
                if (player.PlayerSkills == null || !player.PlayerSkills.Any())
                {
                    throw new ValidationException("skills", "At least one skill is required.");
                }
                foreach (var skill in player.PlayerSkills)
                {
                    if (!_validServices.IsValidSkill(skill.ToString()))
                    {
                        throw new ValidationException("skills", $"Invalid value for skill: {skill.Skill}");
                    }
                }

            }

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();

            return player;
        }




        public async Task<Player> DeletePlayerAsync(int id)
        {
            Player player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                throw new ValidationException("Player not found", "");
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return player;
        }

    }
}
