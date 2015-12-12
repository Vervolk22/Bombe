using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;

namespace Bombe
{
    /// <summary>
    /// Class to handle breaking process indicating at MainWindow with the
    /// squares. Gray - part doesn't checked yet, Blue - checking now,
    /// Red - Solution at that part was not found, Green - solution was found.
    /// </summary>
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

        /// <summary>
        /// COnstructor.
        /// </summary>
        /// <param name="window">Main window to paint where.</param>
        /// <param name="squaresPerRow">How many squares to paint per row.</param>
        /// <param name="groupsExp">Index, that shows how many parts in the all task
        /// will be.</param>
        /// <param name="alphabetLength">Length of alphabet in the task.</param>
        /// <param name="groupsToShow">How many parts of task to show at a
        /// single time.</param>
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

        /// <summary>
        /// Set all squares to given statuses.
        /// </summary>
        /// <param name="array">Array with statuses of parts.</param>
        /// <param name="groups">CheckingGroups array.</param>
        /// <param name="startArray">Index that shows, which array of statuses
        /// array is at the first position.</param>
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

        /// <summary>
        /// Indication, when amount of parts of a task is small.
        /// </summary>
        /// <param name="array">Array with statuses of parts.</param>
        /// <param name="groups">CheckingGroups array.</param>
        /// <param name="startArray">Index that shows, which array of statuses
        /// array is at the first position.</param>
        protected void basicIndication(byte[][] array, byte[] groups, int startArray)
        {
            clearSpace();
            completedCount++;
            drawLine(0, array[0], groups);
        }

        /// <summary>
        /// Draw a single line of squares.
        /// </summary>
        /// <param name="lineNumber">Line number to draw.</param>
        /// <param name="array">Array with part's statuses.</param>
        /// <param name="groups">CheckingGroups array.</param>
        protected void drawLine(int lineNumber, byte[] array, byte[] groups)
        {
            int topOffset = getTopOffset(lineNumber);
            drawText(lineNumber, groups, topOffset - 17);
            for (int i = 0; i < alphabetLength; i++)
            {
                drawSquare(i, array[i], topOffset);
            }
        }

        /// <summary>
        /// Set a single square.
        /// </summary>
        /// <param name="number">Position of square in a group.</param>
        /// <param name="groupNumber">Number of a group.</param>
        /// <param name="type">Type of square to set.</param>
        public void set(int number, int groupNumber, int type)
        {
            if (completedCount + groupNumber <= groupsCount)
                drawSquare(number, type, getTopOffset(groupNumber % groupsToShow));
        }

        /// <summary>
        /// Draw a single square.
        /// </summary>
        /// <param name="number">Position of square in a group.</param>
        /// <param name="type">Type of square to set.</param>
        /// <param name="topOffset">Offset from the top of canvas.</param>
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

        /// <summary>
        /// Draw text for the group.
        /// </summary>
        /// <param name="number">Number of group in the canvas.</param>
        /// <param name="values">CheckingGroups array.</param>
        /// <param name="topOffset">Offset from the top of canvas.</param>
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

        /// <summary>
        /// Clear the canvas.
        /// </summary>
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

        /// <summary>
        /// Get offset from the top of canvas.
        /// </summary>
        /// <param name="number">Number of checkingGroup in the canvas.</param>
        /// <returns>Offset from the top of canvas.</returns>
        protected int getTopOffset(int number)
        {
            return 45 * number + 20;
        }

        /// <summary>
        /// Get total amount of checking groups in the task.
        /// </summary>
        /// <param name="groupsExp">Index, that shows how many parts in the all task
        /// will be.</param>
        /// <param name="countInGroup">How many parts are in a single group.</param>
        /// <returns>Total amount of checking groups.</returns>
        protected int getGroupsCount(int groupExp, int countInGroup)
        {
            int result = 1;
            for (int i = 0; i < groupExp; i++)
            {
                result *= countInGroup;
            }
            return result;
        }

        /// <summary>
        /// Get rectangle to draw.
        /// </summary>
        /// <param name="type">Type of the rectangle to draw.</param>
        /// <returns>Rectangle to draw.</returns>
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

        /// <summary>
        /// Get TextBlock to draw.
        /// </summary>
        /// <param name="values">CheckingGroups array of the task.</param>
        /// <param name="num">Number of currect GroupsTextBlock to draw.</param>
        /// <returns>TextBlock to draw.</returns>
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

            str.Append("X");
            for (int i = 0; i < values.Length; i++)
            {
                str.Append(":" + array[i]);
            }
            str.Append(": ...");

            str.Append("   " + (completedCount + num) + "/" + groupsCount + "   (" +
                    (int)((completedCount + num) * 100 / groupsCount) + "%)");
            TextBlock textBlock = new TextBlock();
            string st = str.ToString();
            textBlock.Text = str.ToString();
            textBlock.Foreground = blackBrush;
            return textBlock;
        }

        /// <summary>
        /// Get TextBlock to draw, when number of checkingGroups is small.
        /// </summary>
        /// <returns>TextBlock to draw.</returns>
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

        /// <summary>
        /// Increment temporary array with checkingGroups, to find
        /// next TextBlock to draw.
        /// </summary>
        /// <param name="array">CheckingGroups array.</param>
        /// <param name="position">Position to add value.</param>
        /// <param name="num">Value to add (offset of next group
        /// from current).</param>
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
