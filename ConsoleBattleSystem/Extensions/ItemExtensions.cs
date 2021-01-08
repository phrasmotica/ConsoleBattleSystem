using BattleSystem.Core.Items;

namespace ConsoleBattleSystem.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Item"/>.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Returns a string summarising this item.
        /// </summary>
        /// <param name="item">The item.</param>
        public static string Summarise(this Item item)
        {
            return $"{item.Name}: {item.Description}";
        }
    }
}
