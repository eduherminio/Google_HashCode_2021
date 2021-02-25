namespace GoogleHashCode.Model
{
    public class Street
    {
        public string Name { get; }

        public int Length { get; }

        public Intersection Origin { get; }

        public Intersection Destination { get; }

        public Street(Intersection origin, Intersection destination, string name, int length)
        {
            Origin = origin;
            Destination = destination;
            Name = name;
            Length = length;
        }
    }
}
