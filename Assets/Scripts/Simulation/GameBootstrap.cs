using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Headless harness: drop on an empty GameObject and press Play. Loads the real
    /// card assets, starts a game, and (optionally) auto-plays it to completion with
    /// the naive AI, logging every step to the Console. No UI — see <see cref="GameView"/>
    /// for a visual, clickable version.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Resources")]
        [Tooltip("Folder under Assets/Resources that holds the CardDataSO assets.")]
        [SerializeField] private string cardResourceFolder = "Card";

        [Header("Game")]
        [Tooltip("Seed for the game RNG (shuffles, coin flip).")]
        [SerializeField] private int gameSeed = 42;
        [Tooltip("If true, auto-plays the whole game with the naive AI on Start.")]
        [SerializeField] private bool autoPlay = true;
        [Tooltip("Seed for the auto-play AI's choices.")]
        [SerializeField] private int aiSeed = 1;
        [Tooltip("Log every command the AI submits.")]
        [SerializeField] private bool verbose = false;

        public GameController Controller { get; private set; }

        private void Start()
        {
            Controller = GameFactory.CreateFromResources(cardResourceFolder, gameSeed, out var summary);
            if (Controller == null)
            {
                Debug.LogError("[GameBootstrap] " + summary);
                return;
            }
            Debug.Log("[GameBootstrap] " + summary);

            if (!autoPlay) return;

            var result = GameSimulator.Run(Controller, new GameSimulator.Options
            {
                Seed = aiSeed,
                Log = verbose ? (line => Debug.Log("[Sim] " + line)) : null,
            });

            Debug.Log($"[GameBootstrap] Result: {result.Outcome} after {result.Turns} turns " +
                      $"({result.Commands} commands){(result.HitTurnCap ? " [HIT TURN CAP]" : "")}.");
        }
    }
}
