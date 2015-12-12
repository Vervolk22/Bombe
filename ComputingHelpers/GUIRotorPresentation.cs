using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingHelpers
{
    /// <summary>
    /// Class to keep references to all controls of a rotor at MainWindow.
    /// </summary>
    public class GUIRotorPresentation
    {
        public Label pos { get; private set; }
        public TextBox layout { get; private set; }
        public TextBox notch { get; private set; }
        public Label rot { get; private set; }
        public TextBox offset { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pos">Label - number of current rotor in the common list.</param>
        /// <param name="layout">TextBox - layout of current rotor.</param>
        /// <param name="notch">TextBox - notch position of current rotor.</param>
        /// <param name="rot">Label - number of current rotor in offsets list.</param>
        /// <param name="offset">TextBox - offset of current rotor.</param>
        public GUIRotorPresentation(Label pos, TextBox layout, TextBox notch, Label rot, TextBox offset)
        {
            this.pos = pos;
            this.layout = layout;
            this.notch = notch;
            this.rot = rot;
            this.offset = offset;
        }
    }
}
