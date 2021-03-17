using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class TokenRequest
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int CourseLayout { get; set; } = 1;
    }
}
