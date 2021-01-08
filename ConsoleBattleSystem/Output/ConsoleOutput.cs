using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem.Abstractions.Control;
using BattleSystem.Core.Actions;
using BattleSystem.Core.Actions.Buff;
using BattleSystem.Core.Actions.Damage;
using BattleSystem.Core.Actions.Heal;
using BattleSystem.Core.Actions.Protect;
using BattleSystem.Core.Actions.ProtectLimitChange;
using BattleSystem.Core.Characters;
using BattleSystem.Core.Items;
using BattleSystem.Core.Moves;
using BattleSystem.Core.Moves.Success;
using ConsoleBattleSystem.Extensions;
using ConsoleBattleSystem.Extensions.ActionResults;

namespace ConsoleBattleSystem.Output
{
    /// <summary>
    /// Class for game output via the console.
    /// </summary>
    public class ConsoleOutput : IGameOutput
    {
        /// <inheritdoc />
        public void ShowStartTurn(int turnCounter)
        {
            ShowMessage();
            ShowMessage($"Turn {turnCounter}");
        }

        /// <inheritdoc />
        public void ShowTeamSummary(IEnumerable<Character> characters)
        {
            ShowMessage();

            foreach (var c in characters.Where(c => !c.IsDead))
            {
                ShowCharacterSummary(c);
            }
        }

        /// <inheritdoc />
        public void ShowCharacterSummary(Character character)
        {
            ShowMessage(character.Summarise());
        }

        /// <inheritdoc />
        public void ShowMoveSetSummary(MoveSet moveSet)
        {
            ShowMessage(moveSet.Summarise(true));
        }

        /// <inheritdoc />
        public void ShowItemSummary(Item item)
        {
            ShowMessage(item.Summarise());
        }

        /// <inheritdoc />
        public void ShowMessage()
        {
            Console.WriteLine();
        }

        /// <inheritdoc />
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc />
        public void ShowMoveUse(MoveUse moveUse)
        {
            if (moveUse.HasResult && !moveUse.TargetsAllDead)
            {
                switch (moveUse.Result)
                {
                    case MoveUseResult.Success:
                        ShowMessage($"{moveUse.User.Name} used {moveUse.Move.Name}!");
                        break;

                    case MoveUseResult.Miss:
                        ShowMessage($"{moveUse.User.Name} used {moveUse.Move.Name} but missed!");
                        break;

                    case MoveUseResult.Failure:
                        ShowMessage($"{moveUse.User.Name} used {moveUse.Move.Name} but it failed!");
                        break;
                }

                foreach (var actionResult in moveUse.ActionsResults)
                {
                    if (actionResult.Success)
                    {
                        foreach (var result in actionResult.Results)
                        {
                            ShowResult(result);
                        }
                    }
                    else
                    {
                        ShowMessage("But it failed!");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void ShowResult<TSource>(IActionResult<TSource> result)
        {
            if (result.TargetProtected)
            {
                ShowMessage(result.DescribeProtected());
            }
            else switch (result)
            {
                case BuffActionResult<TSource> br:
                    var buffDescription = br.Describe();
                    if (buffDescription is not null)
                    {
                        ShowMessage(buffDescription);
                    }
                    break;
                case DamageActionResult<TSource> dr:
                    var damageDescription = dr.Describe();
                    if (damageDescription is not null)
                    {
                        ShowMessage(damageDescription);
                    }
                    break;
                case HealActionResult<TSource> hr:
                    var healDescription = hr.Describe();
                    if (healDescription is not null)
                    {
                        ShowMessage(healDescription);
                    }
                    break;
                case ProtectLimitChangeActionResult<TSource> plcr:
                    var plcrDescription = plcr.Describe();
                    if (plcrDescription is not null)
                    {
                        ShowMessage(plcrDescription);
                    }
                    break;
                case ProtectActionResult<TSource> pr:
                    var protectDescription = pr.Describe();
                    if (protectDescription is not null)
                    {
                        ShowMessage(protectDescription);
                    }
                    break;
            }
        }

        /// <inheritdoc />
        public void ShowBattleEnd(string winningTeam)
        {
            ShowMessage($"Team {winningTeam} wins!");
        }
    }
}
