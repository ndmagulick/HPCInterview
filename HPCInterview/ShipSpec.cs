namespace HPCInterview
{
    public class ShipSpec
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }

        public ShipSpec(string name, string code, int length, int width)
        {
            Name = name;
            Code = code;
            Length = length;
            Width = width;
        }

        public override string ToString()
        {
            return Code + ", " + Name;
        }
    }
}
