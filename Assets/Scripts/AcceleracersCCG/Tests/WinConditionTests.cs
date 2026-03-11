using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class WinConditionTests
    {
        [Test]
        public void ThreeVehiclesFinished_Player0Wins()
        {
            var state = TestHelpers.CreateTestGameState();
            state.Players[0].VehiclesFinished = 3;

            new CheckWinConditionCommand().Execute(state);
            Assert.AreEqual(GameResult.Player0Wins, state.Result);
        }

        [Test]
        public void ThreeVehiclesFinished_Player1Wins()
        {
            var state = TestHelpers.CreateTestGameState();
            state.Players[1].VehiclesFinished = 3;

            new CheckWinConditionCommand().Execute(state);
            Assert.AreEqual(GameResult.Player1Wins, state.Result);
        }

        [Test]
        public void TwoVehiclesFinished_GameContinues()
        {
            var state = TestHelpers.CreateTestGameState();
            state.Players[0].VehiclesFinished = 2;

            new CheckWinConditionCommand().Execute(state);
            Assert.AreEqual(GameResult.InProgress, state.Result);
        }

        [Test]
        public void DeckEmpty_AtDrawPhase_OpponentWins()
        {
            // Directly test: if active player's deck is empty, opponent wins
            var state = TestHelpers.CreateTestGameState();
            state.ActivePlayerIndex = 0;

            // Give player 0 an empty deck
            state.Players[0].Deck = new Components.Deck(new List<CardInstance>());

            // Give player 1 a normal deck
            state.Players[1].Deck = new Components.Deck(new List<CardInstance>
            {
                new CardInstance(TestHelpers.MakeVehicle("v1", "V1"))
            });

            // DrawPhase should detect empty deck and set opponent as winner
            var drawPhase = new StateMachine.Phases.DrawPhase();
            var commands = drawPhase.GetAutoCommands(state);

            // Execute the commands — should include SetGameResult for player 1 winning
            var processor = new CommandProcessor();
            foreach (var cmd in commands)
            {
                processor.ExecuteUnchecked(cmd, state);
            }

            Assert.AreEqual(GameResult.Player1Wins, state.Result);
        }

        [Test]
        public void BothPlayersAtTwo_GameContinues()
        {
            var state = TestHelpers.CreateTestGameState();
            state.Players[0].VehiclesFinished = 2;
            state.Players[1].VehiclesFinished = 2;

            new CheckWinConditionCommand().Execute(state);
            Assert.AreEqual(GameResult.InProgress, state.Result);
        }
    }
}
