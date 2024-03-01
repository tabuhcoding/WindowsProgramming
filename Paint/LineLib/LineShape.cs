
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using MyLib;

namespace LineLib
{
    public class LineShape : IShape
    {
        public override UIElement Draw(Color color, double lineThickness, DoubleCollection strokeDash)
        {
            return new Line()
            {
                X1 = Points[0].X,
                Y1 = Points[0].Y,
                X2 = Points[1].X,
                Y2 = Points[1].Y,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = lineThickness,
            };
        }

        public override IShape Clone()
        {
            return new LineShape();
        }

        public override string Name => "Line";

    }

}
