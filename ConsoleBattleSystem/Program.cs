using System.Collections.Generic;
using BattleSystem.Abstractions.Control;
using BattleSystem.Battles.TurnBased;
using BattleSystem.Battles.TurnBased.Extensions;
using BattleSystem.Core.Actions.Buff;
using BattleSystem.Core.Actions.Damage;
using BattleSystem.Core.Actions.Heal;
using BattleSystem.Core.Actions.Protect;
using BattleSystem.Core.Actions.ProtectLimitChange;
using BattleSystem.Core.Characters;
using BattleSystem.Core.Items;
using BattleSystem.Core.Moves;
using BattleSystem.Core.Moves.Success;
using BattleSystem.Core.Random;
using BattleSystem.Core.Stats;
using ConsoleBattleSystem.Characters;
using ConsoleBattleSystem.Input;
using ConsoleBattleSystem.Output;

namespace ConsoleBattleSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var gameOutput = new ConsoleOutput();
            var playerInput = new ConsoleInput(gameOutput);
            var random = new Random();
            var actionHistory = new ActionHistory();

            gameOutput.ShowMessage("Welcome to the Console Battle System!");

            var playAgain = true;
            while (playAgain)
            {
                new TurnBasedBattle(
                    new MoveProcessor(),
                    actionHistory,
                    gameOutput,
                    CreateCharacters(random, actionHistory, playerInput)
                ).Start();

                var playAgainChoice = playerInput.SelectChoice("Play again? [y/n]", "y", "n");
                playAgain = playAgainChoice == "y";
            }

            playerInput.Confirm("Thanks for playing! Press any key to exit.");
        }

        /// <summary>
        /// Creates characters for the game.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="actionHistory">The action history.</param>
        /// <param name="userInput">The user input.</param>
        private static IEnumerable<Character> CreateCharacters(
            IRandom random,
            ActionHistory actionHistory,
            IUserInput userInput)
        {
            var playerStats = new StatSet
            {
                Attack = new Stat(5),
                Defence = new Stat(4),
                Speed = new Stat(4),
            };

            var playerMoves =
                new MoveSetBuilder()
                    .WithMove(
                        new MoveBuilder()
                            .Name("Sword Strike")
                            .Describe("The user swings their sword to inflict damage. This move has increased priority.")
                            .WithMaxUses(15)
                            .WithPriority(1)
                            .SuccessDecreasesLinearlyWithUses(100, 25, 10, random, MoveUseResult.Failure, actionHistory)
                            .WithAction(
                                new DamageActionBuilder()
                                    .WithBasePower(20, random)
                                    .UserSelectsSingleEnemy(userInput)
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Insistent Jab")
                            .Describe("This attack's base power increases with each consecutive successful use.")
                            .WithMaxUses(15)
                            .WithAccuracy(100, random)
                            .WithAction(
                                new DamageActionBuilder()
                                    .BasePowerIncreasesLinearlyWithUses(20, 5, random, actionHistory)
                                    .UserSelectsSingleEnemy(userInput)
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Retaliate")
                            .Describe("The user deals 1.5x the damage of the last attack it received.")
                            .WithMaxUses(5)
                            .AlwaysSucceeds()
                            .WithAction(
                                new DamageActionBuilder()
                                    .PercentageOfLastReceivedMoveDamage(150, actionHistory)
                                    .Retaliates(actionHistory)
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Restore")
                            .Describe("The user drinks a potion to restore 20 health, while also increasing their protect limit by one.")
                            .WithMaxUses(10)
                            .AlwaysSucceeds()
                            .WithAction(
                                new HealActionBuilder()
                                    .WithAmount(20)
                                    .AbsoluteHealing()
                                    .TargetsUser()
                                    .Build()
                            )
                            .WithAction(
                                new ProtectLimitChangeActionBuilder()
                                    .WithAmount(1)
                                    .TargetsUser()
                                    .Build()
                            )
                            .Build()
                    )
                    .Build();

            var player = new Player(
                userInput,
                "Warrior",
                "a",
                100,
                playerStats,
                playerMoves);

            player.EquipItem(
                new ItemBuilder()
                    .Name("Might Relic")
                    .Describe("Increases the holder's Attack by 5% at the end of each turn.")
                    .WithEndTurnAction(
                        new BuffActionBuilder()
                            .TargetsUser()
                            .WithRaiseAttack(0.05)
                            .Build()
                    )
                    .Build()
            );

            var bardStats = new StatSet
            {
                Attack = new Stat(4),
                Defence = new Stat(3),
                Speed = new Stat(4),
            };

            var bardMoves =
                new MoveSetBuilder()
                    .WithMove(
                        new MoveBuilder()
                            .Name("Play Music")
                            .Describe("The user shreds on their guitar to inflict 5 damage on all enemies.")
                            .WithMaxUses(25)
                            .WithAccuracy(100, random)
                            .WithAction(
                                new DamageActionBuilder()
                                    .AbsoluteDamage(5)
                                    .TargetsEnemies()
                                    .Build()
                            )
                            .Build()
                    )
                    .Build();

            var bard = new BasicCharacter("Bard", "a", 100, bardStats, bardMoves, random);

            bard.EquipItem(
                new ItemBuilder()
                    .Name("Capo")
                    .Describe("Makes the holder's music better, increasing the power of their attacks by 1.")
                    .WithDamagePowerTransform(p => p + 1)
                    .Build()
            );

            var mageStats = new StatSet
            {
                Attack = new Stat(6),
                Defence = new Stat(3),
                Speed = new Stat(5),
            };

            var mageMoves =
                new MoveSetBuilder()
                    .WithMove(
                        new MoveBuilder()
                            .Name("Magic Missile")
                            .Describe("The user fires a spectral missile to inflict 20 damage.")
                            .WithMaxUses(15)
                            .WithAccuracy(100, random)
                            .WithAction(
                                new DamageActionBuilder()
                                    .AbsoluteDamage(20)
                                    .TargetsFirstEnemy()
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Lightning Bolt")
                            .Describe("The user summons a lightning strike to deal damage equal to 30% of the target's health.")
                            .WithMaxUses(5)
                            .WithAccuracy(70, random)
                            .WithAction(
                                new DamageActionBuilder()
                                    .PercentageDamage(30)
                                    .TargetsFirstEnemy()
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Meditate")
                            .Describe("The user finds inner calm to raise the Defence stat of all characters on their team.")
                            .WithMaxUses(10)
                            .AlwaysSucceeds()
                            .WithAction(
                                new BuffActionBuilder()
                                    .TargetsTeam()
                                    .WithRaiseDefence(0.1)
                                    .Build()
                            )
                            .Build()
                    )
                    .WithMove(
                        new MoveBuilder()
                            .Name("Refresh")
                            .Describe("The user regenerates 30% of their max health.")
                            .WithMaxUses(10)
                            .AlwaysSucceeds()
                            .WithAction(
                                new HealActionBuilder()
                                    .WithAmount(30)
                                    .PercentageHealing()
                                    .TargetsUser()
                                    .Build()
                            )
                            .Build()
                    )
                    .Build();

            var mage = new BasicCharacter("Mage", "b", 100, mageStats, mageMoves, random);

            mage.EquipItem(
                new ItemBuilder()
                    .Name("Rolling Wave")
                    .Describe("Deals 6 damage to another random character at the start of the holder's turn.")
                    .WithStartTurnAction(
                        new DamageActionBuilder()
                            .AbsoluteDamage(6)
                            .TargetsRandomOther(random)
                            .Build()
                    )
                    .Build()
                    
            );

            var rogueStats = new StatSet
            {
                Attack = new Stat(3),
                Defence = new Stat(2),
                Speed = new Stat(3),
            };

            var rogueMoves =
                new MoveSetBuilder()
                    .WithMove(
                        new MoveBuilder()
                            .Name("Protect")
                            .Describe("The user protects themself from the next move.")
                            .WithMaxUses(5)
                            .WithPriority(2)
                            .AlwaysSucceeds()
                            .WithAction(
                                new ProtectActionBuilder()
                                    .TargetsFirstAlly()
                                    .Build()
                            )
                            .Build()
                    )
                    .Build();

            var rogue = new BasicCharacter("Rogue", "b", 80, rogueStats, rogueMoves, random);

            return new Character[] { player, bard, mage, rogue };
        }
    }
}
