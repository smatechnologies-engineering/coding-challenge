using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class ErrorMessage
    {
        [DataMember]
        public string Error { get; set; }
    }
}
