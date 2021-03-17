using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class SpeedLimit
    {
        [DataMember]
        public int? Min { get; set; }

        [DataMember]
        public int? Max { get; set; }
    }
}
