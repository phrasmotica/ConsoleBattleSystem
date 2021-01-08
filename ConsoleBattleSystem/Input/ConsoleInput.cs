using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem.Abstractions.Control;
using BattleSystem.Core.Characters;
using BattleSystem.Core.Moves;

namespace ConsoleBattleSystem.Input
{
    /// <summary>
    /// Class for user input via the console.
    /// </summary>
    public class ConsoleInput : IUserInput
    {
        /// <summary>
        /// The game output.
        /// </summary>
        private readonly IGameOutput _gameOutput;

        /// <summary>
        /// Creates a new <see cref="ConsoleInput"/> instance.
        /// </summary>
        /// <param name="gameOutput">The game output.</param>
        public ConsoleInput(IGameOutput gameOutput)
        {
            _gameOutput = gameOutput;
        }

        /// <inheritdoc />
        public int SelectIndex()
        {
            var choiceIsValid = false;
            var chosenIndex = -1;

            while (!choiceIsValid)
            {
                var input = Console.ReadLine()?.Trim();
                choiceIsValid = int.TryParse(input, out chosenIndex);

                if (!choiceIsValid)
                {
                    Console.WriteLine("Please enter a valid integer!");
                }
            }

            return chosenIndex;
        }

        /// <inheritdoc />
        public string SelectChoice(string prompt = null, params string[] choices)
        {
            if (prompt is not null)
            {
                Console.WriteLine(prompt);
            }

            var choiceIsValid = false;
            var choice = string.Empty;
            choices = choices.Select(c => c.ToLower()).ToArray();

            while (!choiceIsValid)
            {
                choice = Console.ReadLine()?.Trim();
                choiceIsValid = choices.Contains(choice.ToLower());

                if (!choiceIsValid)
                {
                    Console.WriteLine("Please enter a valid choice!");
                }
            }

            return choice;
        }

        /// <inheritdoc />
        public void Confirm(string prompt = null)
        {
            if (prompt is not null)
            {
                Console.WriteLine(prompt);
            }

            Console.ReadKey();
        }

        /// <inheritdoc />
        public Move SelectMove(Character user, IEnumerable<Character> otherCharacters)
        {
            Move move = null;

            var validIndexes = user.Moves.GetIndexes();
            int chosenIndex = -1;

            var allCharacters = otherCharacters.Prepend(user).ToArray();

            while (!validIndexes.Contains(chosenIndex) || !(move?.CanUse() ?? false))
            {
                _gameOutput.ShowMessage();
                _gameOutput.ShowMessage($"What will {user.Name} do?");
                _gameOutput.ShowMoveSetSummary(user.Moves);
                _gameOutput.ShowMessage();

                var inspectChoices = allCharacters.Select((_, i) => $"inspect {i + 1}").ToArray();

                var input = Console.ReadLine();
                if (input == "view")
                {
                    ViewCharacters(user, otherCharacters);
                    continue;
                }

                var index = Array.IndexOf(inspectChoices, input);
                if (index > -1)
                {
                    InspectPlayer(user, allCharacters[index]);
                    continue;
                }

                var inputIsValid = int.TryParse(input, out chosenIndex);

                if (!validIndexes.Contains(chosenIndex))
                {
                    _gameOutput.ShowMessage($"Invalid choice! Please enter one of: {string.Join(", ", validIndexes)}");
                    continue;
                }

                // chosenIndex starts from 1, so subtract 1 to avoid off-by-one errors
                move = user.Moves.GetMove(chosenIndex - 1);

                if (!move.CanUse())
                {
                    _gameOutput.ShowMessage($"{move.Name} has no uses left! Choose another move");
                }
            }

            return move;
        }

        // <inheritdoc />
        public Character SelectTarget(IEnumerable<Character> targets)
        {
            // TODO: we use 'move' instead of 'action' under the assumption
            // that only one action in the move will require selecting a target.
            // This should be fixed to show a different message depending on the
            // type of the action
            _gameOutput.ShowMessage("Select a target for the move:");
            _gameOutput.ShowMessage(GetChoices(targets));

            Character character = null;

            var validIndexes = targets.Select((_, i) => i + 1);
            int chosenIndex = -1;

            while (!validIndexes.Contains(chosenIndex))
            {
                chosenIndex = SelectIndex();

                if (!validIndexes.Contains(chosenIndex))
                {
                    _gameOutput.ShowMessage($"Invalid choice! Please enter one of: {string.Join(", ", validIndexes)}");
                    continue;
                }

                // subtract 1 to avoid off-by-one errors
                character = targets.ToArray()[chosenIndex - 1];
            }

            return character;
        }

        /// <summary>
        /// Views a summary of all the characters.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="otherCharacters">The other characters.</param>
        private void ViewCharacters(Character user, IEnumerable<Character> otherCharacters)
        {
            var allCharacters = otherCharacters.Prepend(user).ToArray();
            var teams = allCharacters.GroupBy(c => c.Team).ToArray();

            foreach (var team in teams)
            {
                _gameOutput.ShowMessage();

                foreach (var c in team.Where(c => !c.IsDead))
                {
                    _gameOutput.ShowCharacterSummary(c);
                }
            }
        }

        /// <summary>
        /// Inspects the given character.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="character">The character to inspect.</param>
        private void InspectPlayer(Character user, Character character)
        {
            _gameOutput.ShowMessage();
            _gameOutput.ShowCharacterSummary(character);

            if (character.Team == user.Team)
            {
                _gameOutput.ShowMessage();
                _gameOutput.ShowMessage("Moves:");
                _gameOutput.ShowMoveSetSummary(character.Moves);

                if (character.HasItem)
                {
                    _gameOutput.ShowMessage();
                    _gameOutput.ShowMessage("Item:");
                    _gameOutput.ShowItemSummary(character.Item);
                }
            }
        }

        /// <summary>
        /// Returns a string describing the given targets.
        /// </summary>
        /// <param name="targets">The targets.</param>
        private static string GetChoices(IEnumerable<Character> targets)
        {
            var choices = targets.Select((c, i) => $"{i + 1}: {c.Name}");
            return string.Join("\n", choices);
        }
    }
}
