using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class VehicleCardData : CardData
    {
        public override CardType CardType => CardType.Vehicle;
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
            string effectId = null)
            : base(id, name, apCost: 0, effectId: effectId, spp: spp, terrainIcons: terrainIcons)
        {
            Team = team;
            ModabilityIcons = modabilityIcons;
            IsAdvancedVehicle = isAdvancedVehicle;
            BaseVehicleName = baseVehicleName;
        }
    }
}
