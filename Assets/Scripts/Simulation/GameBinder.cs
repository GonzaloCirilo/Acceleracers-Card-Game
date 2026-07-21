using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Drives your hand-authored board. Drop this on a manager GameObject and drag your
    /// zone rectangles into the slots below. It builds the game from the card assets and
    /// renders each zone by instantiating <see cref="CardView"/> prefabs into your
    /// containers, then refreshes after every move.
    ///
    /// Interaction (hot-seat): click a hand card to play it. If it needs a target
    /// (Mod/Shift/AcceleCharger/Hazard) the valid vehicles highlight — click one to
    /// complete the play. Use the End Phase button to advance.
    ///
    /// Every reference is optional: wire only the zones you've built and the rest is
    /// skipped, so you can grow the board incrementally.
    /// </summary>
    public class GameBinder : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private string cardResourceFolder = "Card";
        [SerializeField] private int gameSeed = 42;

        [Header("Prefab")]
        [Tooltip("Your card rectangle prefab (must have a CardView component).")]
        [SerializeField] private CardView cardPrefab;

        [Header("Zone containers (drag your rectangles here)")]
        [Tooltip("Holds the 4 realm tiles.")]
        [SerializeField] private Transform realmContainer;
        [Tooltip("Holds the active player's hand cards.")]
        [SerializeField] private Transform handContainer;
        [Tooltip("Holds Player 0's vehicles in play.")]
        [SerializeField] private Transform player0VehicleContainer;
        [Tooltip("Holds Player 1's vehicles in play.")]
        [SerializeField] private Transform player1VehicleContainer;

        [Header("HUD")]
        [SerializeField] private Text statusLabel;
        [SerializeField] private Button endPhaseButton;

        private GameController _controller;
        private int _selectedCardId = -1; // hand card awaiting a target vehicle

        private void Start()
        {
            _controller = GameFactory.CreateFromResources(cardResourceFolder, gameSeed, out var summary);
            if (_controller == null)
            {
                Debug.LogError("[GameBinder] " + summary);
                return;
            }
            Debug.Log("[GameBinder] " + summary);

            if (cardPrefab == null)
                Debug.LogError("[GameBinder] No cardPrefab assigned — nothing will render.");

            if (endPhaseButton != null)
                endPhaseButton.onClick.AddListener(OnEndPhase);

            Refresh();
        }

        // -------------------------------------------------------------- interaction

        private void OnHandCardClicked(int cardUniqueId)
        {
            var legal = _controller.GetLegalCommands();
            var forCard = legal.Where(c => SourceCardId(c) == cardUniqueId).ToList();
            if (forCard.Count == 0) return;

            // No-target plays (play vehicle, discard) resolve immediately.
            var immediate = forCard.FirstOrDefault(c => c is PlayVehicleCommand || c is DiscardCardCommand);
            if (immediate != null)
            {
                Submit(immediate);
                return;
            }

            // Otherwise this card needs a target vehicle — enter targeting mode.
            _selectedCardId = cardUniqueId;
            Refresh();
        }

        private void OnVehicleClicked(int vehicleUniqueId)
        {
            if (_selectedCardId < 0) return;

            var match = _controller.GetLegalCommands().FirstOrDefault(c =>
                SourceCardId(c) == _selectedCardId && TargetVehicleId(c) == vehicleUniqueId);

            if (match != null) Submit(match);
        }

        private void OnEndPhase()
        {
            if (_controller.IsGameOver) return;
            Submit(new EndPhaseCommand(_controller.State.ActivePlayerIndex));
        }

        private void Submit(ICommand cmd)
        {
            var error = _controller.SubmitCommand(cmd);
            if (error != null) Debug.LogWarning("[GameBinder] Rejected: " + error);
            _selectedCardId = -1;
            Refresh();
        }

        // -------------------------------------------------------------- rendering

        private void Refresh()
        {
            var s = _controller.State;
            var legal = _controller.IsGameOver ? new List<ICommand>() : _controller.GetLegalCommands();

            RenderRealms(s);
            RenderVehicles(s, 0, player0VehicleContainer, legal);
            RenderVehicles(s, 1, player1VehicleContainer, legal);
            RenderHand(s, legal);
            RenderHud(s, legal);
        }

        private void RenderRealms(GameState s)
        {
            if (realmContainer == null || cardPrefab == null) return;
            Clear(realmContainer);
            for (int i = 0; i < Constants.RealmsPerRace; i++)
            {
                var realm = s.RealmTrack.GetRealm(i);
                var revealed = s.RealmTrack.IsRevealed(i);
                var view = Instantiate(cardPrefab, realmContainer);
                if (realm == null)
                {
                    view.Bind("[empty]", "", false, null, CardView.State.Dim);
                }
                else if (!revealed)
                {
                    view.Bind("Realm ?", "hidden", false, null, CardView.State.Dim);
                }
                else
                {
                    var rd = realm.Data as RacingRealmCardData;
                    var stat = rd != null ? $"esc {rd.EscapeValue} {Cat(rd.EscapeCategory)}" : "";
                    view.Bind(realm.Data.Name, stat, false, null);
                }
            }
        }

        private void RenderVehicles(GameState s, int playerIndex, Transform container, List<ICommand> legal)
        {
            if (container == null || cardPrefab == null) return;
            Clear(container);

            foreach (var stack in s.Players[playerIndex].VehiclesInPlay)
            {
                int vid = stack.Vehicle.UniqueId;

                // A vehicle is clickable when we're targeting and it's a legal target for the selected card.
                bool isTarget = _selectedCardId >= 0 && legal.Any(c =>
                    SourceCardId(c) == _selectedCardId && TargetVehicleId(c) == vid);

                var view = Instantiate(cardPrefab, container);
                view.Bind(stack.Vehicle.Data.Name, VehicleStat(stack, s),
                    interactable: isTarget,
                    onClick: () => OnVehicleClicked(vid),
                    state: isTarget ? CardView.State.Highlight : CardView.State.Normal);
            }
        }

        private void RenderHand(GameState s, List<ICommand> legal)
        {
            if (handContainer == null || cardPrefab == null) return;
            Clear(handContainer);

            var active = s.ActivePlayer;
            foreach (var card in active.Hand.Cards)
            {
                int cid = card.UniqueId;
                bool playable = legal.Any(c => SourceCardId(c) == cid);
                var state = cid == _selectedCardId ? CardView.State.Selected
                          : playable ? CardView.State.Normal : CardView.State.Dim;

                var view = Instantiate(cardPrefab, handContainer);
                view.Bind(card.Data.Name, CardStat(card.Data),
                    interactable: playable,
                    onClick: () => OnHandCardClicked(cid),
                    state: state);
            }
        }

        private void RenderHud(GameState s, List<ICommand> legal)
        {
            if (endPhaseButton != null)
                endPhaseButton.interactable = !_controller.IsGameOver && legal.Any(c => c is EndPhaseCommand);

            if (statusLabel == null) return;

            if (_controller.IsGameOver)
            {
                statusLabel.text = $"GAME OVER — {s.Result}";
                return;
            }

            var p0 = s.Players[0];
            var p1 = s.Players[1];
            var hint = _selectedCardId >= 0 ? "  ▶ pick a highlighted vehicle" : "";
            statusLabel.text =
                $"Turn {s.TurnNumber}   {s.CurrentPhase}   Active: P{s.ActivePlayerIndex}   AP:{s.ActivePlayer.AP}{hint}\n" +
                $"P0  hand {p0.Hand.Count}  deck {p0.Deck.Count}  finished {p0.VehiclesFinished}/{Constants.VehiclesToWin}    " +
                $"P1  hand {p1.Hand.Count}  deck {p1.Deck.Count}  finished {p1.VehiclesFinished}/{Constants.VehiclesToWin}";
        }

        // -------------------------------------------------------------- helpers

        private static void Clear(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                Destroy(t.GetChild(i).gameObject);
        }

        private static string VehicleStat(VehicleStack stack, GameState s)
        {
            if (stack.HasFinished) return "FINISHED";
            var eff = SPPCalculator.Calculate(stack, s.RealmTrack);
            return $"{eff.Speed}/{eff.Power}/{eff.Performance}  R{stack.RealmIndex}";
        }

        private static string CardStat(CardData d)
        {
            if (d is ISPPCard spp) return $"{spp.SPP.Speed}/{spp.SPP.Power}/{spp.SPP.Performance}";
            if (d is HazardCardData h) return $"dmg {h.SPPDamage.Speed}/{h.SPPDamage.Power}/{h.SPPDamage.Performance}";
            if (d is RacingRealmCardData r) return $"esc {r.EscapeValue}";
            return "";
        }

        private static string Cat(SPPCategory c)
        {
            switch (c)
            {
                case SPPCategory.Speed: return "SPD";
                case SPPCategory.Power: return "PWR";
                default: return "PRF";
            }
        }

        // Which hand card (unique id) a command originates from, or -1.
        private static int SourceCardId(ICommand c)
        {
            switch (c)
            {
                case PlayVehicleCommand pv: return pv.CardUniqueId;
                case EquipModCommand em: return em.CardUniqueId;
                case EquipShiftCommand es: return es.CardUniqueId;
                case EquipAcceleChargerCommand ea: return ea.CardUniqueId;
                case PlayHazardCommand ph: return ph.HazardCardUniqueId;
                case DiscardCardCommand dc: return dc.CardUniqueId;
                default: return -1;
            }
        }

        // Which vehicle a targeted command points at, or -1.
        private static int TargetVehicleId(ICommand c)
        {
            switch (c)
            {
                case EquipModCommand em: return em.TargetVehicleUniqueId;
                case EquipShiftCommand es: return es.TargetVehicleUniqueId;
                case EquipAcceleChargerCommand ea: return ea.TargetVehicleUniqueId;
                case PlayHazardCommand ph: return ph.TargetVehicleUniqueId;
                default: return -1;
            }
        }
    }
}
