﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingHelpers
{
    public class GUIRotorPresentation
    {
        public Label pos { get; private set; }
        public TextBox layout { get; private set; }
        public TextBox notch { get; private set; }

        public GUIRotorPresentation(Label pos, TextBox layout, TextBox notch)
        {
            this.pos = pos;
            this.layout = layout;
            this.notch = notch;
        }
    }
}