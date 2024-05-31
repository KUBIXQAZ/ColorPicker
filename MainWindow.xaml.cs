using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap colorBitmap;
        private bool isMouseDown = false;
        private Color color;

        public MainWindow()
        {
            InitializeComponent();

            color = Color.FromRgb(0, 0, 0);
            selectedColorRectangle.Fill = new SolidColorBrush(color);

            CreateColorGradient();
        }

        private void ColorGradientRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            PickColor(e.GetPosition(colorGradientRectangle));
        }

        private void colorGradientRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if(isMouseDown) PickColor(e.GetPosition(colorGradientRectangle));
        }

        private void colorGradientRectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        public void PickColor(Point position)
        {
            if (position.X >= 0 && position.X < 300 && position.Y >= 0 && position.Y < 300)
            {
                int x = (int)position.X;
                int y = (int)position.Y;

                int stride = colorBitmap.PixelWidth * 4;
                byte[] pixels = new byte[stride * colorBitmap.PixelHeight];
                colorBitmap.CopyPixels(pixels, stride, 0);

                int index = (y * colorBitmap.PixelWidth + x) * 4;
                byte b = pixels[index];
                byte g = pixels[index + 1];
                byte r = pixels[index + 2];

                color = Color.FromRgb(r, g, b);
                selectedColorRectangle.Fill = new SolidColorBrush(color);

                RValue.Value = r;
                GValue.Value = g;
                BValue.Value = b;

                RLabel.Content = r;
                GLabel.Content = g;
                BLabel.Content = b;
            }
        }

        private void CreateColorGradient()
        {
            int width = 300;
            int height = 300;
            colorBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double hue = (double)x / width;
                    double saturation = (double)y / height;
                    Color color = HsvToRgb(hue, saturation, 1.0);
                    int index = (y * width + x) * 4;
                    pixels[index] = color.B;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.R;
                    pixels[index + 3] = 255;
                }
            }

            colorBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            colorGradientRectangle.Fill = new ImageBrush(colorBitmap);
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            int i = (int)(h * 6);
            double f = h * 6 - i;
            double p = v * (1 - s);
            double q = v * (1 - f * s);
            double t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private void BValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            color.B = (byte)BValue.Value;
            selectedColorRectangle.Fill = new SolidColorBrush(color);

            BLabel.Content = BValue.Value;
        }

        private void GValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            color.G = (byte)GValue.Value;
            selectedColorRectangle.Fill = new SolidColorBrush(color);

            GLabel.Content = GValue.Value;
        }

        private void RValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            color.R = (byte)RValue.Value;
            selectedColorRectangle.Fill = new SolidColorBrush(color);

            RLabel.Content = RValue.Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"{color.R},{color.G},{color.B}");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"));
        }
    }
}