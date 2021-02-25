using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode.Model
{
    public class Car
    {
        private int _itineraryPosition;

        public bool Finished { get; private set; }

        public int Id { get; }

        public List<Street> Itinerary { get; }

        public Street? CurrentStreet { get; set; }

        public int StreetPosition { get; set; }

        public Car(int n, int id)
        {
            _itineraryPosition = 0;
            Itinerary = new List<Street>(n - 1);
            Id = id;
        }

        public void Move()
        {
            //if (CurrentStreet is null)
            //{
            //    CurrentStreet = Itinerary[0];
            //    StreetPosition = CurrentStreet.Length;
            //}

            ++StreetPosition;

            if (StreetPosition >= CurrentStreet.Length)
            {
                if (CurrentStreet.Name == Itinerary.Last().Name)
                {
                    Finished = true;
                    return;
                }

                var trafficLight = CurrentStreet.Destination.TrafficLights.Single(s => s.StreetName == CurrentStreet.Name);

                if (trafficLight.Green &&
                    (trafficLight.CarsWaiting.Count == 0 || trafficLight.CarsWaiting.Peek() == this)) // Pase semaforo
                {
                    trafficLight.CarsWaiting.Dequeue();
                    StreetPosition = StreetPosition > CurrentStreet.Length
                        ? 1
                        : 0;

                    ++_itineraryPosition;
                    CurrentStreet = Itinerary[_itineraryPosition];
                }
                else // final de nuestra calle
                {
                    trafficLight.CarsWaiting.Enqueue(this);
                    StreetPosition = CurrentStreet.Length;
                }
            }
        }
    }
}
