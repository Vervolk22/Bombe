using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComputingHelpers;

namespace Bombe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComputingScheduler scheduler;
        private GUIRotorPresentation[] rotorPresentations;
        private string[] enigmaSettings;

        protected SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        protected SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        public MainWindow()
        {
            InitializeComponent();
            scheduler = new ComputingScheduler(this);
            makePreparations();
            //scheduler.run();
        }

        protected void makePreparations()
        {
            iplabel.Content = scheduler.getLocalIP();
            setRotorPresentations();
            setRotorContent();
            bindHandlers();
        }

        protected void bindHandlers()
        {
            //this.cmdReceiveConnections.MouseUp += new RoutedEventHandler(this.cmdListen_Click);
            //this.cmdReceiveConnections.MouseUp += new MouseButtonEventHandler(this.cmdListen_Click);
            //this.cmdCompute.MouseUp += new
        }

        protected void cmdListen_Click(object sender, System.EventArgs e)
        {
            scheduler.changeServerStatus();
        }

        protected void cmdCompute_Click(object sender, System.EventArgs e)
        {
            scheduler.startBreaking();
        }

        protected void rotorsAmountChanged(object sender, System.EventArgs e)
        {
            int rotorsAmount = Int32.Parse((string)((ComboBoxItem)rotorsamount.SelectedValue).Content);
            for (int i = 0; i < rotorsAmount; i++)
            {
                rotorPresentations[i].pos.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].layout.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].notch.Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = rotorsAmount; i < 10; i++)
            {
                rotorPresentations[i].pos.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].layout.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].notch.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        protected void setRotorPresentations()
        {
            rotorPresentations = new GUIRotorPresentation[11];
            rotorPresentations[0] = new GUIRotorPresentation(pos1, layout1, notch1);
            rotorPresentations[1] = new GUIRotorPresentation(pos2, layout2, notch2);
            rotorPresentations[2] = new GUIRotorPresentation(pos3, layout3, notch3);
            rotorPresentations[3] = new GUIRotorPresentation(pos4, layout4, notch4);
            rotorPresentations[4] = new GUIRotorPresentation(pos5, layout5, notch5);
            rotorPresentations[5] = new GUIRotorPresentation(pos6, layout6, notch6);
            rotorPresentations[6] = new GUIRotorPresentation(pos7, layout7, notch7);
            rotorPresentations[7] = new GUIRotorPresentation(pos8, layout8, notch8);
            rotorPresentations[8] = new GUIRotorPresentation(pos9, layout9, notch9);
            rotorPresentations[9] = new GUIRotorPresentation(pos10, layout10, notch10);
            rotorPresentations[10] = new GUIRotorPresentation(pos11, layout11, null);
            rotorsamount.SelectedItem = rotorsamount.Items[2];
        }

        protected void setRotorContent()
        {
            FileWorker fworker = new FileWorker();
            enigmaSettings = fworker.getEnigmaSettings();
            for (int i = 0; i < 10; i++)
            {
                rotorPresentations[i].layout.Text = enigmaSettings[i * 2];
                rotorPresentations[i].notch.Text = enigmaSettings[i * 2 + 1];
            }
            rotorPresentations[10].layout.Text = enigmaSettings[20];
        }

        public void mainListAppendText(string s)
        {
            mainlist.AppendText(s);
        }

        protected void validateLayout(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkLayout(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        protected void validateNotch(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkNotch(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        public string[] getRotorsLayout()
        {
            int rotorsAmount = Byte.Parse(rotorsamount.Text);
            string[] layouts = new string[rotorsAmount + 1];
            for (int i = 0; i < rotorsAmount; i++)
            {
                layouts[i] = rotorPresentations[i].layout.Text;
            }
            layouts[rotorsAmount] = rotorPresentations[10].layout.Text;
            return layouts;
        }

        public char[] getNotchPositions()
        {
            int rotorsAmount = Byte.Parse(rotorsamount.Text);
            char[] notch = new char[rotorsAmount];
            for (int i = 0; i < rotorsAmount; i++)
            {
                notch[i] = rotorPresentations[i].notch.Text[0];
            }
            return notch;
        }
    }
}
