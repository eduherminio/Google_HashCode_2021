using FileParser;
using GoogleHashCode.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleHashCode
{
    public class Solver
    {
        private const int RoundRobin = 1;
        private readonly string _inputFile;
        private readonly string _outputFile;

        private int _simulationTime;
        private int _pointsPerCar;
        private readonly Dictionary<string, Street> _dict_streets = new();
        private readonly List<Street> _streets = new();
        private readonly List<Car> _cars = new();
        private readonly List<Intersection> _intersections = new();
        private readonly Dictionary<int, Intersection> _dict_intersections = new();

        public Solver(string inputFileName)
        {
            _inputFile = Path.Combine("Inputs", inputFileName);
            _outputFile = Path.Combine("Outputs", $"output_{inputFileName}");

            Directory.CreateDirectory("Outputs");
        }

        public void Solve()
        {
            ParseInput();

            foreach (var car in _cars)
            {
                car.CurrentStreet = car.Itinerary[0];
                car.StreetPosition = car.CurrentStreet.Length;
                car.CurrentStreet!.Destination.TrafficLights.Single(s => s.StreetName == car.CurrentStreet.Name).CarsWaiting.Enqueue(car);
            }

            foreach (var intersection in _intersections)
            {
                if (intersection.TrafficLights.Count == 1)
                {
                    intersection.Schedule.Add(new(intersection.TrafficLights[0], 6));
                }
                else
                {
                    foreach (var trafficLight in intersection.TrafficLights)
                    {
                        intersection.Schedule.Add(new(trafficLight, RoundRobin));
                    }
                }
            }

            PrintResult();
            //return;

            var magia = new Dictionary<TrafficLight, int>();

            int t = 0;
            while (t <= _simulationTime)
            {
                foreach (var intersection in _intersections)
                {
                    intersection.NextTurn();
                }

                foreach (var car in _cars.Where(c => !c.Finished))
                {
                    car.Move();
                }

                foreach (var intersection in _intersections)
                {
                    foreach (var sem in intersection.TrafficLights)
                    {
                        if (magia.TryGetValue(sem, out var val))
                        {
                            magia[sem] = val + sem.CarsWaiting.Count;
                        }
                        else
                        {
                            magia[sem] = sem.CarsWaiting.Count;
                        }
                    }
                }

                ++t;
            }

            var ordered = magia
                .Where(m => m.Value != 0)
                .OrderByDescending(m => m.Value);

            var semsByInter = ordered
                .GroupBy(pair => pair.Key.IntersectionId)
                .Where(pair => pair.Count() > 1)
                .ToList();

            foreach (var intersection in _intersections)
            {
                if (intersection.TrafficLights.Count == 1)
                {
                    intersection.Schedule.Add(new(intersection.TrafficLights[0], 6));
                }
                else
                {
                    var one = semsByInter.FirstOrDefault(g => g.Key == intersection.Id);
                    if (one is not null)
                    {
                        var order = one.OrderBy(s => s.Value);

                        var val = 1;
                        for (int i = 1; i < order.Count(); ++i)
                        {
                            var ex = intersection.Schedule.Find(pair => pair.TrafficLight == order.ElementAt(i).Key);
                            if (ex.TrafficLight is not null)
                            {
                                intersection.Schedule.Remove(ex);
                            }

                            if (order.ElementAt(i).Value > order.ElementAt(i - 1).Value)
                            {
                                intersection.Schedule.Add((order.ElementAt(i).Key, ++val));
                            }
                            else
                            {
                                intersection.Schedule.Add((order.ElementAt(i).Key, val));
                            }
                        }
                    }
                    else
                    {
                        foreach (var trafficLight in intersection.TrafficLights)
                        {
                            intersection.Schedule.Add(new(trafficLight, RoundRobin));
                        }
                    }
                }
            }

            PrintResult();
        }

        private void ParseInput()
        {
            var file = new ParsedFile(_inputFile);

            var line = file.NextLine();

            _simulationTime = line.NextElement<int>();
            var intersections = line.NextElement<int>();
            var streets = line.NextElement<int>();
            var cars = line.NextElement<int>();
            _pointsPerCar = line.NextElement<int>();

            for (int i = 0; i < intersections; ++i)
            {
                var inter = new Intersection(i);
                _intersections.Add(inter);
                _dict_intersections.Add(i, inter);
            }

            for (int i = 0; i < streets; ++i)
            {
                line = file.NextLine();
                var beginId = line.NextElement<int>();
                var endId = line.NextElement<int>();
                var streetName = line.NextElement<string>();

                var begin = _dict_intersections[beginId];
                var end = _dict_intersections[endId];

                end.TrafficLights.Add(new TrafficLight(streetName, end.Id));

                Street st = new Street(begin, end, streetName, line.NextElement<int>());
                _streets.Add(st);
                _dict_streets.Add(streetName, st);
            }

            for (int i = 0; i < cars; ++i)
            {
                line = file.NextLine();
                var totalStreets = line.NextElement<int>();
                var car = new Car(totalStreets, i + 1);

                for (int j = 0; j < totalStreets; ++j)
                {
                    var name = line.NextElement<string>();

                    var street = _dict_streets[name];

                    car.Itinerary.Add(street);
                }

                _cars.Add(car);
            }
        }

        private void PrintResult()
        {
            var lines = new List<string>() { _intersections.Count.ToString() };

            foreach (var inter in _intersections)
            {
                lines.Add(inter.Id.ToString());
                lines.Add(inter.TrafficLights.Count.ToString());

                foreach (var streetName in inter.TrafficLights.Select(tf => tf.StreetName))
                {
                    lines.Add($"{streetName} {inter.Schedule.First(pair => pair.TrafficLight.StreetName == streetName).timeOn}");
                }
            }
            File.WriteAllLines(_outputFile, lines);
        }
    }
}
