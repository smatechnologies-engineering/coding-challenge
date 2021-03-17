using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class SpeedLimitAhead : SpeedLimit
    {
        [DataMember]
        public double? RemainingDistanceToEnforcement { get; set; }
    }
}
