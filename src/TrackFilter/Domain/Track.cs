using System.Collections.Generic;
using System.Windows.Media;

namespace Domain
{
    public class Track
    {
        private List<Coordinate> _coordinates = new List<Coordinate>();

        public List<Coordinate> Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }
        public Color Color { get; set; }
    }
}
