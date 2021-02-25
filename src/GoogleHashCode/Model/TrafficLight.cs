using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode.Model
{
    public class TrafficLight
    {
        public string Id => $"{IntersectionId}_{StreetName}";
        public string StreetName { get; }

        public bool Green { get; set; }

        public int IntersectionId { get; }

        public Queue<Car> CarsWaiting { get; }

        public TrafficLight(string streetName, int intersectionId)
        {
            StreetName = streetName;
            Green = false;
            IntersectionId = intersectionId;
            CarsWaiting = new Queue<Car>();
        }

    }
}
