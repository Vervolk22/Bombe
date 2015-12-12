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
        protected int completedCount = 0;
        protected SolidColorBrush lightGrayBrush = new SolidColorBrush(Colors.LightGray);
        protected SolidColorBrush lightBlueBrush = new SolidColorBrush(Colors.LightBlue);
        protected SolidColorBrush lightRedBrush = new SolidColorBrush(Colors.PaleVioletRed);
        protected SolidColorBrush lightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        protected SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);
        protected SolidColorBrush blueBrush = new SolidColorBrush(Colors.Blue);
        protected SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        protected SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);
        protected SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);

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

        public void setAll(byte[][] array, byte[] groups, int startArray)
        {
            if (completedCount == groupsCount)
            {
                return;
            }
            if (groupsCount == 1)
            {
                basicIndication(array, groups, startArray);
                return;
            }
            clearSpace();
            completedCount++;
            for (int i = 0; i < groupsToShow; i++)
            {
                drawLine(i, array[(i + startArray) % groupsToShow], groups);
            }
        }

        protected void basicIndication(byte[][] array, byte[] groups, int startArray)
        {
            clearSpace();
            completedCount++;
            drawLine(0, array[0], groups);
        }

        protected void drawLine(int lineNumber, byte[] array, byte[] groups)
        {
            int topOffset = getTopOffset(lineNumber);
            drawText(lineNumber, groups, topOffset - 17);
            for (int i = 0; i < alphabetLength; i++)
            {
                drawSquare(i, array[i], topOffset);
            }
        }

        public void set(int number, int groupNumber, int type)
        {
            if (completedCount + groupNumber <= groupsCount)
                drawSquare(number, type, getTopOffset(groupNumber % groupsToShow));
        }

        protected void drawSquare(int number, int type, int topOffset)
        {
            int row = number / squaresPerRow;
            if (completedCount + row > groupsCount + 1) return;
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

        protected void drawText(int number, byte[] values, int topOffset)
        {
            try
            {
                window.Dispatcher.Invoke((Action)(() =>
                {
                    TextBlock textBlock = getGroupsTextBlock(values, number);
                    Canvas.SetLeft(textBlock, 20);
                    Canvas.SetTop(textBlock, topOffset);
                    window.canvas.Children.Add(textBlock);
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
            return 45 * number + 20;
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

        protected TextBlock getGroupsTextBlock(byte[] values, int num)
        {
            if (values.Length == 0)
            {
                return getBasicGroupsTextBlock();
            }
            StringBuilder str = new StringBuilder(64);
            byte[] array = new byte[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i];
            }
            incrementLastChecked(array, 0, (byte)num);

            str.Append("... :");
            for (int i = values.Length - 1; i >= 0; i--)
            {
                str.Append(array[i] + ":");
            }
            str.Append("X");

            str.Append("   " + (completedCount + num) + "/" + groupsCount + "   (" +
                    (int)((completedCount + num) * 100 / groupsCount) + "%)");
            TextBlock textBlock = new TextBlock();
            string st = str.ToString();
            textBlock.Text = str.ToString();
            textBlock.Foreground = blackBrush;
            return textBlock;
        }

        protected TextBlock getBasicGroupsTextBlock()
        {
            StringBuilder str = new StringBuilder(64);

            str.Append("X");
            str.Append("   " + (completedCount) + "/" + groupsCount + "   (" +
                    (int)((completedCount) * 100 / groupsCount) + "%)");
            TextBlock textBlock = new TextBlock();
            string st = str.ToString();
            textBlock.Text = str.ToString();
            textBlock.Foreground = blackBrush;
            return textBlock;
        }

        protected void incrementLastChecked(byte[] array, int position, byte num)
        {
            array[position] += num;
            if (array[position] == alphabetLength)
            {
                array[position] = (byte)(array[position] % alphabetLength);
                incrementLastChecked(array, position + 1, num);
            }
        }
    }
}
