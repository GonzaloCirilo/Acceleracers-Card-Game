using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// "Destroy after N turns" effect. Sets a countdown token on equip,
    /// and junks the card when the token reaches 0 during TuneUp.
    /// </summary>
    public class TimedDestructionEffect : ICardEffect
    {
        public const string TokenKey = "timed_destruction";
        public int Turns { get; set; } = 2; // Default 2 turns

        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            var commands = new List<ICommand>();

            if (context.Trigger == EffectTrigger.OnEquip)
            {
                // Set countdown token via command
                commands.Add(new SetTokenCommand(
                    context.OwnerPlayerIndex,
                    context.SourceStack.Vehicle.UniqueId,
                    $"{TokenKey}_{context.SourceCard.UniqueId}",
                    Turns));
            }
            else if (context.Trigger == EffectTrigger.OnTuneUp)
            {
                var tokenKey = $"{TokenKey}_{context.SourceCard.UniqueId}";
                var remaining = context.SourceStack.Tokens.Get(tokenKey);
                if (remaining <= 1)
                {
                    // Time's up — junk the card (junk handles cleanup)
                    commands.Add(new JunkCardCommand(
                        context.OwnerPlayerIndex,
                        context.SourceStack.Vehicle.UniqueId,
                        context.SourceCard.UniqueId));
                }
                else
                {
                    commands.Add(new TickTokenCommand(
                        context.OwnerPlayerIndex,
                        context.SourceStack.Vehicle.UniqueId,
                        tokenKey));
                }
            }

            return commands;
        }
    }
}
