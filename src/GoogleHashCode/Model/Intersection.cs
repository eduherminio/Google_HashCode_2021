using System.Collections.Generic;

namespace GoogleHashCode.Model
{
    public class Intersection
    {
        public List<TrafficLight> TrafficLights { get; set; }
        public int Id { get; }

        public List<(TrafficLight TrafficLight, int timeOn)> Schedule { get; set; }

        public int Turn { get; private set; }
        public (TrafficLight TrafficLight, int TimeOn)? On { get; private set; }
        public int TimeOn { get; private set; }

        public Intersection(int id)
        {
            Id = id;
            TrafficLights = new List<TrafficLight>();
            Schedule = new List<(TrafficLight TrafficLight, int timeOn)>();
        }

        public void NextTurn()
        {
            if (On is null)
            {
                On = Schedule[0];
                TimeOn = 0;
            }

            if (++TimeOn >= On.Value.TimeOn)
            {
                Turn = (++Turn % TrafficLights.Count);
            }
        }

    }
}
