using System.Collections.Generic;
using BattleSystem.Abstractions.Control;
using BattleSystem.Core.Characters;
using BattleSystem.Core.Moves;
using BattleSystem.Core.Stats;

namespace ConsoleBattleSystem.Characters
{
    /// <summary>
    /// Class representing a user-controller player.
    /// </summary>
    public class Player : Character
    {
        /// <summary>
        /// The user input.
        /// </summary>
        private readonly IUserInput _userInput;

        /// <summary>
        /// Creates a new <see cref="Player"/> instance.
        /// </summary>
        public Player(
            IUserInput userInput,
            string name,
            string team,
            int maxHealth,
            StatSet stats,
            MoveSet moves) : base(name, team, maxHealth, stats, moves)
        {
            _userInput = userInput;
        }

        /// <inheritdoc/>
        public override MoveUse ChooseMove(IEnumerable<Character> otherCharacters)
        {
            var move = _userInput.SelectMove(this, otherCharacters);

            return new MoveUse
            {
                Move = move,
                User = this,
                OtherCharacters = otherCharacters,
            };
        }
    }
}
