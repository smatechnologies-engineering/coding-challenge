using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SelfDrivingCar.Entities
{
    [DataContract]
    public class Car
    {
        [DataMember]
        public string Ignition { get; set; }

        [DataMember]
        public Engine Engine { get; set; }

        [DataMember]
        public double? CurrentVelocity { get; set; }

        [DataMember]
        public double? TotalDistanceTravelled { get; set; }

        [DataMember]
        public double? TotalTimeTravelled { get; set; }

        public Car()
        {
            Ignition = "Off";
            Engine = new Engine();
        }
    }
}
