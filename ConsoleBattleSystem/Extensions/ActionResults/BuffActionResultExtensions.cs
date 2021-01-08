using System.Collections.Generic;
using BattleSystem.Core.Actions.Buff;
using BattleSystem.Core.Items;
using BattleSystem.Core.Stats;

namespace ConsoleBattleSystem.Extensions.ActionResults
{
    /// <summary>
    /// Extension methods for <see cref="BuffActionResult{TSource}"/>.
    /// </summary>
    public static class BuffActionResultExtensions
    {
        /// <summary>
        /// Returns a string describing this buff result.
        /// </summary>
        /// <param name="buff">The buff result.</param>
        public static string Describe<TSource>(this BuffActionResult<TSource> buff)
        {
            var target = buff.Target;
            if (target.IsDead)
            {
                return null;
            }

            var descriptions = new List<string>();

            foreach (var change in buff.StatMultiplierChanges)
            {
                var description = DescribeStatChange(buff, change);
                if (description is not null)
                {
                    descriptions.Add(description);
                }
            }

            return string.Join("\n", descriptions);
        }

        /// <summary>
        /// Returns a string describing the given stat change in the context of the given buff result.
        /// </summary>
        /// <param name="buff">The buff result.</param>
        /// <param name="statChange">The stat change.</param>
        private static string DescribeStatChange<TSource>(
            BuffActionResult<TSource> buff,
            KeyValuePair<StatCategory, double> statChange)
        {
            var percentage = (int) (statChange.Value * 100);
            if (percentage == 0)
            {
                return null;
            }

            var target = buff.Target;
            var stat = statChange.Key;

            if (buff.IsSelfInflicted)
            {
                if (percentage > 0)
                {
                    return buff.Source switch
                    {
                        Item item => $"{target.Name}'s {item.Name} increased their {stat} by {percentage}%!",
                        _ => $"{target.Name}'s {stat} rose by {percentage}%!",
                    };
                }

                return buff.Source switch
                {
                    Item item => $"{target.Name}'s {item.Name} decreased their {stat} by {-percentage}%!",
                    _ => $"{target.Name}'s {stat} fell by {-percentage}%!",
                };
            }

            var user = buff.User;

            if (percentage > 0)
            {
                return buff.Source switch
                {
                    Item item => $"{user.Name}'s {item.Name} increased {target.Name}'s {stat} by {percentage}%!",
                    _ => $"{target.Name}'s {stat} rose by {percentage}%!",
                };
            }

            return buff.Source switch
            {
                Item item => $"{user.Name}'s {item.Name} decreased {target.Name}'s {stat} by {-percentage}%!",
                _ => $"{target.Name}'s {stat} fell by {-percentage}%!",
            };
        }
    }
}
