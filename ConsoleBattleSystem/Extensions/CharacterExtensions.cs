using BattleSystem.Core.Characters;

namespace ConsoleBattleSystem.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Character"/> and its sub-types.
    /// </summary>
    public static class CharacterExtensions
    {
        /// <summary>
        /// Returns a string containing this given character's name and health.
        /// </summary>
        /// <param name="character">The character.</param>
        public static string Summarise(this Character character)
        {
            return $"{character.Name}: {character.CurrentHealth}/{character.MaxHealth} HP";
        }
    }
}
