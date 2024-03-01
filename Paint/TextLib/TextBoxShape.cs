using MyLib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextLib
{
    public class TextBoxShape : IShape
    {
        public override string Name => "TextBox";

        public override IShape Clone()
        {
            return new TextBoxShape();
        }

        public override UIElement Draw(Color color, double lineThickness, DoubleCollection strokeDash)
        {
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var textBox = new TextBox()
            {
                Width = width,
                Height = height,
                Foreground = new SolidColorBrush(color),
                FontSize = Math.Max(Math.Min(Math.Abs(width - 10), Math.Abs(height - 10)), 1),
                IsEnabled = true,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Colors.Transparent),
                TextWrapping = TextWrapping.WrapWithOverflow
            };


            Canvas.SetLeft(textBox, Math.Min(Points[0].X, Points[1].X));
            Canvas.SetTop(textBox, Math.Min(Points[0].Y, Points[1].Y));

            return textBox;
        }
    }
}
