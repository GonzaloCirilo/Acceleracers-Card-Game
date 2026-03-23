using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class VehicleCardData : CardData, ISPPCard
    {
        public override CardType CardType => CardType.Vehicle;
        public SPP SPP { get; }
        public Team Team { get; }
        public ModabilityIcon ModabilityIcons { get; }
        public bool IsAdvancedVehicle { get; }
        public string BaseVehicleName { get; }

        public VehicleCardData(string id, string name, SPP spp,
            Team team = Team.None,
            ModabilityIcon modabilityIcons = ModabilityIcon.None,
            TerrainIcon terrainIcons = TerrainIcon.None,
            bool isAdvancedVehicle = false,
            string baseVehicleName = null,
            IEnumerable<string> effectIds = null)
            : base(id, name, effectIds: effectIds, terrainIcons: terrainIcons)
        {
            SPP = spp;
            Team = team;
            ModabilityIcons = modabilityIcons;
            IsAdvancedVehicle = isAdvancedVehicle;
            BaseVehicleName = baseVehicleName;
        }
    }
}
