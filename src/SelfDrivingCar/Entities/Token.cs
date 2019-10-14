using System;
using System.Runtime.Serialization;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class Token
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime ExpiryDate { get; set; }
    }
}
