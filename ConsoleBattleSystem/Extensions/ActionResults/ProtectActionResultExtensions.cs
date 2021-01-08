using BattleSystem.Core.Actions.Protect;
using BattleSystem.Core.Items;

namespace ConsoleBattleSystem.Extensions.ActionResults
{
    /// <summary>
    /// Extension methods for <see cref="ProtectActionResult{TSource}"/>.
    /// </summary>
    public static class ProtectActionResultExtensions
    {
        /// <summary>
        /// Returns a string describing this protect result.
        /// </summary>
        /// <param name="protect">The protect result.</param>
        public static string Describe<TSource>(this ProtectActionResult<TSource> protect)
        {
            var target = protect.Target;
            if (target.IsDead)
            {
                return null;
            }

            if (protect.IsSelfInflicted)
            {
                return protect.Source switch
                {
                    Item item => $"{target.Name}'s {item.Name} protected them!",
                    _ => $"{target.Name} became protected!",
                };
            }

            var user = protect.User;

            return protect.Source switch
            {
                Item item => $"{user.Name}'s {item.Name} protected {target.Name}!",
                _ => $"{user.Name} protected {target.Name}!",
            };
        }
    }
}
