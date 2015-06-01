using System.Collections.Generic;
using System.Windows.Media;

namespace Domain
{
    public class Track
    {
        private List<Coordinate> _coordinates = new List<Coordinate>();
        private Color _color = Colors.Black;
        private string _name = "Source";

        public List<Coordinate> Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
