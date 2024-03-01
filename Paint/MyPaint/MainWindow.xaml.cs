using MyLib;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ShapeFactory _factory;
        bool isDrawing = false;
        Point _start;
        Point _end;
        string _choice; // Line
        List<IShape> _shapes = new List<IShape>();
        ControlTemplate btnControlTemplate;
        double _lineThickness = 1;
        Color _lineColor = Colors.Blue;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var abilities = new List<IShape>();

            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            abilities.Add(shape!);
                        }
                    }
                }
            }

            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(
                    ability.Name, ability
                );

            };

            if (abilities.Count > 0)
            {
                _choice = abilities[0].Name;
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            _start = e.GetPosition(drawingCanvas);

            try
            {
                _lineThickness = double.Parse(LineThickness.Text);

                if (_lineThickness < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                LineThickness.Text = _lineThickness.ToString();
                MessageBox.Show("Invalid line thickness!", "Error", MessageBoxButton.OK);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                try
                {
                    _lineThickness = double.Parse(LineThickness.Text);

                    if (_lineThickness < 0)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    LineThickness.Text = _lineThickness.ToString();
                    MessageBox.Show("Invalid line thickness!", "Error", MessageBoxButton.OK);
                }

                _end = e.GetPosition(drawingCanvas);

                IShape preview = _factory.Create(_choice);
                preview.Points.Add(_start);
                preview.Points.Add(_end);

                drawingCanvas.Children.Clear();

                foreach (var shape in _shapes)
                {
                    var drawnObj = shape.Draw(_lineColor, _lineThickness, new DoubleCollection() { 1 });

                    if (drawnObj.GetType() == typeof(TextBox))
                    {
                        textBoxCanvas.Children.Add(drawnObj);

                        var textBox = (TextBox)drawnObj;
                        textBox.BorderThickness = new Thickness(0);

                        continue;
                    }    
                    drawingCanvas.Children.Add(drawnObj);    
                }

                drawingCanvas.Children.Add(preview.Draw(_lineColor, _lineThickness, new DoubleCollection() { 1 }));
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _end = e.GetPosition(drawingCanvas);

            isDrawing = false;
            if (_start.X == _end.X && _start.Y == _end.Y)
            {
                return;
            }

            IShape shape = _factory.Create(_choice);
            shape.Points.Add(_start);
            shape.Points.Add(_end);

            _shapes.Add(shape);
        }

        private void LineToolButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = "Line";
        }

        private void RectangleToolButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = "Rectangle";
        }

        private void EllipseToolButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = "Ellipse";
        }

        private void TextToolButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = "TextBox";
        }
    }
}