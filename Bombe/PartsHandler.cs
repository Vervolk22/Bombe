using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;

namespace Bombe
{
    class PartsHandler
    {
        private MainWindow window;
        private int count = 0;
        private int squaresPerRow = 0;
        private SolidColorBrush lightGrayBrush = new SolidColorBrush(Colors.LightGray);
        private SolidColorBrush lightBlueBrush = new SolidColorBrush(Colors.LightBlue);
        private SolidColorBrush lightRedBrush = new SolidColorBrush(Colors.PaleVioletRed);
        private SolidColorBrush lightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        private SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);
        private SolidColorBrush blueBrush = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        internal PartsHandler(MainWindow window, int squaresPerRow)
        {
            this.window = window;
            this.squaresPerRow = squaresPerRow;
        }

        public void setAll(int count)
        {
            for (int i = 0; i < count; i++)
            {
                draw(i, 0);
            }
        }

        public void set(int number, int type)
        {
            draw(number, type);
        }

        protected void draw(int number, int type)
        {
            window.Dispatcher.Invoke((Action)(() =>
            {
                int row = number / squaresPerRow;
                int pos = number % squaresPerRow;
                int left = pos * (int)window.canvas.Width / (squaresPerRow);
                int top = row * 15;
                System.Windows.Shapes.Rectangle rect = getRect(type);
                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                window.canvas.Children.Add(rect);
            }));
        }

        protected System.Windows.Shapes.Rectangle getRect(int type)
        {
            System.Windows.Shapes.Rectangle rect;
            rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Fill = new SolidColorBrush(Colors.Black);
            rect.Width = 10;
            rect.Height = 10;
            switch (type)
            {
                case 0:
                    rect.Stroke = grayBrush;
                    rect.Fill = lightGrayBrush;
                    break;
                case 1:
                    rect.Stroke = blueBrush;
                    rect.Fill = lightBlueBrush;
                    break;
                case 2:
                    rect.Stroke = redBrush;
                    rect.Fill = lightRedBrush;
                    break;
                case 3:
                    rect.Stroke = greenBrush;
                    rect.Fill = lightGreenBrush;
                    break;
            }
            return rect;
        }
    }
}
