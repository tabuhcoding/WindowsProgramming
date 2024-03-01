
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using MyLib;

namespace EllipseLib
{
    public class EllipseShape : IShape
    {
        public override UIElement Draw(Color color, double lineThickness, DoubleCollection strokeDash)
        {
            // TODO: can dam bao Diem 0 < Diem 1
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = lineThickness
            };

            Canvas.SetLeft(element, Math.Min(Points[0].X, Points[1].X));
            Canvas.SetTop(element, Math.Min(Points[0].Y, Points[1].Y));

            return element;
        }

        public override IShape Clone()
        {
            return new EllipseShape();
        }

        public override string Name => "Ellipse";
    }
}
