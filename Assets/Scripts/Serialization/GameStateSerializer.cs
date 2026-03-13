using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Serialization
{
    /// <summary>
    /// Serializes/deserializes game state to/from a simple JSON-like format.
    /// Uses plain C# (no Unity JsonUtility dependency for portability).
    /// </summary>
    public static class GameStateSerializer
    {
        /// <summary>
        /// Serialize minimal game state info for snapshot comparison.
        /// </summary>
        public static string Serialize(GameState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Turn: {state.TurnNumber}");
            sb.AppendLine($"Phase: {state.CurrentPhase}");
            sb.AppendLine($"ActivePlayer: {state.ActivePlayerIndex}");
            sb.AppendLine($"Result: {state.Result}");

            for (int i = 0; i < 2; i++)
            {
                var p = state.Players[i];
                sb.AppendLine($"--- Player {i} ---");
                sb.AppendLine($"  Deck: {p.Deck.Count}");
                sb.AppendLine($"  Hand: {p.Hand.Count} [{string.Join(", ", p.Hand.Cards.Select(c => c.Data.Name))}]");
                sb.AppendLine($"  Junk: {p.JunkPile.Count}");
                sb.AppendLine($"  AP: {p.AP}");
                sb.AppendLine($"  VehiclesFinished: {p.VehiclesFinished}");
                sb.AppendLine($"  Vehicles in play: {p.VehiclesInPlay.Count}");
                foreach (var v in p.VehiclesInPlay)
                {
                    sb.AppendLine($"    {v.Vehicle.Data.Name} @ Realm {v.RealmIndex}");
                    sb.AppendLine($"      Mods: {v.EquippedMods.Count}, Shifts: {v.EquippedShifts.Count}");
                    sb.AppendLine($"      AcceleCharger: {v.AcceleCharger?.Data.Name ?? "none"}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compute a simple hash of the game state for comparison.
        /// </summary>
        public static int ComputeHash(GameState state)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + state.TurnNumber;
                hash = hash * 31 + (int)state.CurrentPhase;
                hash = hash * 31 + state.ActivePlayerIndex;
                hash = hash * 31 + (int)state.Result;

                for (int i = 0; i < 2; i++)
                {
                    var p = state.Players[i];
                    hash = hash * 31 + p.Deck.Count;
                    hash = hash * 31 + p.Hand.Count;
                    hash = hash * 31 + p.JunkPile.Count;
                    hash = hash * 31 + p.AP;
                    hash = hash * 31 + p.VehiclesFinished;
                    hash = hash * 31 + p.VehiclesInPlay.Count;

                    foreach (var v in p.VehiclesInPlay)
                    {
                        hash = hash * 31 + v.Vehicle.UniqueId;
                        hash = hash * 31 + v.RealmIndex;
                        hash = hash * 31 + v.EquippedMods.Count;
                        hash = hash * 31 + v.EquippedShifts.Count;
                        hash = hash * 31 + (v.AcceleCharger?.UniqueId ?? 0);
                    }
                }

                return hash;
            }
        }
    }
}
