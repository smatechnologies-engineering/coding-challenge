using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class Road
    {
        [DataMember]
        public SpeedLimit CurrentSpeedLimit { get; set; }

        [DataMember]
        public SpeedLimitAhead SpeedLimitAhead { get; set; }
    }
}
