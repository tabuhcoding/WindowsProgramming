
using System.Windows;
using System.Windows.Media;

namespace MyLib
{
    public abstract class IShape
    {
        public abstract string Name { get; }
        public List<Point> Points { get; set; } = new List<Point>();

        public abstract UIElement Draw(Color color, double lineThickness, DoubleCollection strokeDash);
        public abstract IShape Clone();
    }
}
