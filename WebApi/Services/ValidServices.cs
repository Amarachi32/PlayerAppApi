using WebApi.Entities;
using WebApi.Interface;

namespace WebApi.Services
{
    public class ValidServices : IValidServices
    {
        public bool IsValidPlayer(Player player)
        {
            var validPositions = new List<string> { "defender", "midfielder", "forward" };
            if (!validPositions.Contains(player.Position.ToLower()))
            {
                return false;
            }

            if (player.PlayerSkills == null || !player.PlayerSkills.Any())
            {
                return false;
            }

            var validSkills = new List<string> { "defense", "attack", "speed", "strength", "stamina" };
            foreach (var skill in player.PlayerSkills)
            {
                if (!validSkills.Contains(skill.Skill.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsValidSkill(string skillName)
        {
            var validSkills = new List<string> { "defense", "attack", "speed", "strength", "stamina" };
            return validSkills.Contains(skillName.ToLower());
        }

        public bool IsValidPosition(string position)
        {
            var validPositions = new List<string> { "defender", "midfielder", "forward" };
            return validPositions.Contains(position.ToLower());
        }

        public int GetHighestSkillValue(Player player)
        {
            if (player == null || player.PlayerSkills == null || player.PlayerSkills.Count == 0)
            {
                return 0;
            }

            return player.PlayerSkills.Max(skill => skill.Value);
        }

        public int GetSkillValue(Player player, string skillName)
        {
            if (player == null || player.PlayerSkills == null)
            {
                return -1; // Indicate that the player does not have the skill you are seeking for.
            }

            var matchingSkills = player.PlayerSkills
                .Where(skill => skill.Skill == skillName)
                .ToList();

            if (matchingSkills.Count == 0)
            {
                return -1;
            }

            var highestSkillValue = matchingSkills.Max(skill => skill.Value);

            return highestSkillValue;
        }


    }
}
