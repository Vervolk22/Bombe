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
        protected MainWindow window;
        protected int squaresPerRow;
        protected int groupsCount;
        protected int alphabetLength;
        protected int groupsToShow;
        protected int canvasWidth;
        protected SolidColorBrush lightGrayBrush = new SolidColorBrush(Colors.LightGray);
        protected SolidColorBrush lightBlueBrush = new SolidColorBrush(Colors.LightBlue);
        protected SolidColorBrush lightRedBrush = new SolidColorBrush(Colors.PaleVioletRed);
        protected SolidColorBrush lightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        protected SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);
        protected SolidColorBrush blueBrush = new SolidColorBrush(Colors.Blue);
        protected SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        protected SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        internal PartsHandler(MainWindow window, int squaresPerRow, int groupsExp,
                int alphabetLength, int groupsToShow)
        {
            this.window = window;
            this.squaresPerRow = squaresPerRow;
            this.alphabetLength = alphabetLength;
            this.groupsCount = getGroupsCount(groupsExp, alphabetLength);
            this.groupsToShow = groupsToShow;

            window.Dispatcher.Invoke((Action)(() =>
            {
                canvasWidth = (int)window.canvas.Width;
            }));
        }

        public void setAll(byte[][] array, int startArray)
        {
            clearSpace();
            for (int i = 0; i < groupsToShow; i++)
            {
                drawLine(i, array[(i + startArray) % groupsToShow]);
            }
        }

        protected void drawLine(int lineNumber, byte[] array)
        {
            for (int i = 0; i < alphabetLength; i++)
            {
                draw(i, array[i], getTopOffset(lineNumber));
            }
        }

        public void set(int number, int groupNumber, int type)
        {
            draw(number, type, getTopOffset(groupNumber));
        }

        protected void draw(int number, int type, int topOffset)
        {

            int row = number / squaresPerRow;
            int pos = number % squaresPerRow;
            int left = pos * canvasWidth / (squaresPerRow);
            int top = row * 15 + topOffset;
            try
            {
                window.Dispatcher.Invoke((Action)(() =>
                {
                    System.Windows.Shapes.Rectangle rect = getRect(type);
                    Canvas.SetLeft(rect, left);
                    Canvas.SetTop(rect, top);
                    window.canvas.Children.Add(rect);
                }));
            }
            catch (Exception e)
            {

            }
        }

        protected void clearSpace()
        {
            try
            {
                window.Dispatcher.Invoke((Action)(() =>
                {
                    window.canvas.Children.Clear();
                }));
            }
            catch (Exception e)
            {

            }
        }

        protected int getTopOffset(int number)
        {
            return 45 * number;
        }

        protected int getGroupsCount(int groupExp, int countInGroup)
        {
            int result = 1;
            for (int i = 0; i < groupExp; i++)
            {
                result *= countInGroup;
            }
            return result;
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
