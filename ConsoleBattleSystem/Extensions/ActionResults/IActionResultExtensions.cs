using BattleSystem.Core.Actions;

namespace ConsoleBattleSystem.Extensions.ActionResults
{
    /// <summary>
    /// Extension methods for <see cref="IActionResult{TSource}"./>
    /// </summary>
    public static class IActionResultExtensions
    {
        /// <summary>
        /// Returns a string describing this action result being protected.
        /// </summary>
        /// <param name="result">The action result.</param>
        public static string DescribeProtected<TSource>(this IActionResult<TSource> result)
        {
            var target = result.Target;
            var protectUser = result.ProtectUser;

            if (target == protectUser)
            {
                return $"{target.Name} protected itself!";
            }

            return $"{target.Name} was protected by {protectUser.Name}!";
        }
    }
}
