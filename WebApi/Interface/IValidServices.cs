using WebApi.Entities;

namespace WebApi.Interface
{
    public interface IValidServices
    {
        bool IsValidPlayer(Player player);
        bool IsValidSkill(string skillName);
        bool IsValidPosition(string position);
        int GetSkillValue(Player player, string skillName);
        int GetHighestSkillValue(Player player);
    }
}
