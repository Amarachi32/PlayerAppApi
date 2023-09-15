using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Interface;

namespace WebApi.Services
{
    public class TeamServices : ITeamServices
    {
        private readonly DataContext _context;
        private readonly IValidServices _validServices;

        public TeamServices(DataContext context, IValidServices validServices)
        {
            _context = context;
            _validServices = validServices;
        }

        public async Task<List<Player>> SelectTeamsAsync(List<TeamRequirement> requirements)
        {
            var selectedPlayers = new List<Player>();

            foreach (var requirement in requirements)
            {
                if (!_validServices.IsValidPosition(requirement.Position))
                {
                    throw new ValidationException("position", $"Invalid position: {requirement.Position}");
                }

                var playersWithPosition = await _context.Players
                    .Where(player => player.Position.ToLower() == requirement.Position.ToLower())
                    .Include(player => player.PlayerSkills)
                    .ToListAsync();

                if (playersWithPosition.Count == 0)
                {
                    throw new ValidationException("position", $"Insufficient number of players for position: {requirement.Position}");
                }

                var playersWithSkill = playersWithPosition
                    .Where(player => _validServices.GetSkillValue(player, requirement.MainSkill) > 0)
                    .ToList();

                if (playersWithSkill.Count == 0)
                {
                    var bestPlayer = playersWithPosition
                        .OrderByDescending(player => _validServices.GetHighestSkillValue(player))
                        .First();

                    selectedPlayers.Add(bestPlayer);
                }
                else
                {
                    var numberOfPlayersToSelect = requirement.NumberOfPlayers;

                    if (playersWithSkill.Count >= numberOfPlayersToSelect)
                    {
                        selectedPlayers.AddRange(playersWithSkill
                            .OrderByDescending(player => _validServices.GetSkillValue(player, requirement.MainSkill))
                            .Take(numberOfPlayersToSelect));
                    }
                    else
                    {
                        selectedPlayers.AddRange(playersWithSkill);
                        numberOfPlayersToSelect -= playersWithSkill.Count;
                        var playersWithoutSkill = playersWithPosition
                            .Where(player => _validServices.GetSkillValue(player, requirement.MainSkill) == 0)
                            .ToList();

                        if (playersWithoutSkill.Count == 0)
                        {
                            throw new ValidationException("position", $"No more players available for position: {requirement.Position}");
                        }
                        selectedPlayers.AddRange(playersWithoutSkill
                            .OrderByDescending(player => _validServices.GetSkillValue(player, requirement.MainSkill))
                            .Take(numberOfPlayersToSelect));
                    }
                }
            }

            return selectedPlayers;
        }

    }
}
