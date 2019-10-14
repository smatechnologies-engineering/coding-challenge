using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class Engine
    {
        public Engine()
        {
            State = "Off";
        }

        [DataMember]
        public string State { get; set; }
    }
}
