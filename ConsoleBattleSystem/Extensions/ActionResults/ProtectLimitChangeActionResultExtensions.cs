using BattleSystem.Core.Actions.ProtectLimitChange;
using BattleSystem.Core.Items;

namespace ConsoleBattleSystem.Extensions.ActionResults
{
    /// <summary>
    /// Extension methods for <see cref="ProtectLimitChangeActionResult{TSource}"/>.
    /// </summary>
    public static class ProtectLimitChangeActionResultExtensions
    {
        /// <summary>
        /// Returns a string describing this protect limit change result.
        /// </summary>
        /// <param name="result">The protect limit change result.</param>
        public static string Describe<TSource>(this ProtectLimitChangeActionResult<TSource> result)
        {
            var target = result.Target;
            if (target.IsDead)
            {
                return null;
            }

            if (result.IsSelfInflicted)
            {
                if (result.Amount > 0)
                {
                    return result.Source switch
                    {
                        Item item => $"{target.Name}'s {item.Name} increased their protection limit by {result.Amount}!",
                        _ => $"{target.Name} increased their protection limit by {result.Amount}!",
                    };
                }

                return result.Source switch
                {
                    Item item => $"{target.Name}'s {item.Name} decreased their protection limit by {-result.Amount}!",
                    _ => $"{target.Name} decreased their protection limit by {-result.Amount}!",
                };
            }

            var user = result.User;

            if (result.Amount > 0)
            {
                return result.Source switch
                {
                    Item item => $"{user.Name}'s {item.Name} increased {target.Name}'s protection limit by {result.Amount}!",
                    _ => $"{user.Name} increased {target.Name}'s protection limit by {result.Amount}!",
                };
            }

            return result.Source switch
            {
                Item item => $"{user.Name}'s {item.Name} decreased {target.Name}'s protection limit by {-result.Amount}!",
                _ => $"{user.Name} decreased {target.Name}'s protection limit by {-result.Amount}!",
            };
        }
    }
}
