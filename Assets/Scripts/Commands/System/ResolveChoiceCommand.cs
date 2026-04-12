using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Resolves a pending player choice with a selected card unique ID.
    /// Clears PendingChoice and executes the appropriate action.
    /// For multi-step choices, sets the next PendingChoice.
    /// </summary>
    public class ResolveChoiceCommand : ICommand
    {
        public int SelectedCardUniqueId { get; }

        public ResolveChoiceCommand(int selectedCardUniqueId)
        {
            SelectedCardUniqueId = selectedCardUniqueId;
        }

        public string Validate(GameState state)
        {
            if (state.PendingChoice == null)
                return "No pending choice to resolve.";
            if (!state.PendingChoice.Options.Contains(SelectedCardUniqueId))
                return "Selected card is not a valid option.";
            return null;
        }

        public void Execute(GameState state)
        {
            var choice = state.PendingChoice;
            state.PendingChoice = null;

            switch (choice.Type)
            {
                case ChoiceType.RecoverCardFromJunk:
                    new RecoverCardFromJunkCommand(choice.PlayerIndex, SelectedCardUniqueId).Execute(state);
                    break;

                case ChoiceType.TransferModSelectMod:
                    ResolveTransferModSelectMod(state, choice);
                    break;

                case ChoiceType.TransferModSelectTarget:
                    new TransferModCommand(choice.PlayerIndex, choice.AuxCardUniqueId.Value, SelectedCardUniqueId, choice.IgnoreModability).Execute(state);
                    break;

                case ChoiceType.RecoverModsForAP:
                    ResolveRecoverModsForAP(state, choice);
                    break;
            }
        }

        private void ResolveTransferModSelectMod(GameState state, PendingChoice choice)
        {
            var player = state.GetPlayer(choice.PlayerIndex);
            var sourceStack = player.VehiclesInPlay.FirstOrDefault(s => s.Vehicle.UniqueId == choice.SourceCardUniqueId);
            if (sourceStack == null) return;

            var mod = sourceStack.EquippedMods.FirstOrDefault(m => m.UniqueId == SelectedCardUniqueId);
            if (mod == null) return;

            // Collect valid targets for the selected mod
            var targets = new List<int>();
            foreach (var target in player.VehiclesInPlay)
            {
                if (target == sourceStack) continue;
                if (((VehicleCardData)target.Vehicle.Data).Team != Team.MetalManiacs) continue;
                if (target.RealmIndex != sourceStack.RealmIndex) continue;
                if (!ModabilityRules.CanEquipMod(mod.Data, target.Vehicle.Data)) continue;
                targets.Add(target.Vehicle.UniqueId);
            }

            // No valid targets — effect is lost
            if (targets.Count == 0) return;

            state.PendingChoice = new PendingChoice(
                ChoiceType.TransferModSelectTarget,
                choice.PlayerIndex,
                sourceCardUniqueId: choice.SourceCardUniqueId,
                options: targets,
                auxCardUniqueId: SelectedCardUniqueId,
                ignoreModability: choice.IgnoreModability
            );
        }

        private void ResolveRecoverModsForAP(GameState state, PendingChoice choice)
        {
            // Player chose to stop
            if (SelectedCardUniqueId == PendingChoice.PassOptionId) return;

            var player = state.GetPlayer(choice.PlayerIndex);
            var mod = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == SelectedCardUniqueId);
            if (mod == null) return;

            player.JunkPile.Remove(SelectedCardUniqueId);
            player.Hand.Add(mod);
            player.AP -= 1;

            // Offer another recovery if AP and mods remain
            if (player.AP < 1) return;

            var remaining = player.JunkPile.Cards
                .Where(c => c.Data.CardType == Core.CardType.Mod)
                .Select(c => c.UniqueId)
                .ToList();

            if (remaining.Count == 0) return;

            var options = new List<int>(remaining) { PendingChoice.PassOptionId };
            state.PendingChoice = new PendingChoice(
                ChoiceType.RecoverModsForAP,
                choice.PlayerIndex,
                choice.SourceCardUniqueId,
                options);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
