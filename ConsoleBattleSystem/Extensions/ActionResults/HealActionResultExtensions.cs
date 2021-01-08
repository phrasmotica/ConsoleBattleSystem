using BattleSystem.Core.Actions.Heal;
using BattleSystem.Core.Items;

namespace ConsoleBattleSystem.Extensions.ActionResults
{
    /// <summary>
    /// Extension methods for <see cref="HealActionResult{TSource}"/>.
    /// </summary>
    public static class HealActionResultExtensions
    {
        /// <summary>
        /// Returns a string describing this heal result.
        /// </summary>
        /// <param name="heal">The heal result.</param>
        public static string Describe<TSource>(this HealActionResult<TSource> heal)
        {
            var target = heal.Target;

            if (heal.IsSelfInflicted)
            {
                return heal.Source switch
                {
                    Item item => $"{target.Name}'s {item.Name} restored {heal.Amount} health to them!",
                    _ => $"{target.Name} recovered {heal.Amount} health!",
                };
            }

            var user = heal.User;

            return heal.Source switch
            {
                Item item => $"{user.Name}'s {item.Name} restored {heal.Amount} health to {target.Name}!",
                _ => $"{user.Name} restored {heal.Amount} health to {target.Name}!",
            };
        }
    }
}
