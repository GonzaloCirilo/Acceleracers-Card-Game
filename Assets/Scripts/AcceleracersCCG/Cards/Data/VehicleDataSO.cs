using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewVehicle", menuName = "AcceleracersCCG/Vehicle")]
    public class VehicleDataSO : CardDataSO
    {
        [Header("Vehicle")]
        public Team team;
        public ModabilityIcon modabilityIcons;
        public bool isAdvancedVehicle;
        public string baseVehicleName;

        public override CardData ToCardData()
        {
            return new VehicleCardData(cardId, cardName, new SPP(speed, power, performance),
                team: team, modabilityIcons: modabilityIcons, terrainIcons: terrainIcons,
                isAdvancedVehicle: isAdvancedVehicle, baseVehicleName: baseVehicleName,
                effectId: effectId);
        }
    }
}
