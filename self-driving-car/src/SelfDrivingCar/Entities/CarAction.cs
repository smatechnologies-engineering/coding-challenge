using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class CarAction
    {
        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public int? Force { get; set; }
    }
}
