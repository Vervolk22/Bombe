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

        protected void tabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab1.IsSelected || tab4.IsSelected)
            {
                int rotorsAmount = Int32.Parse((string)((ComboBoxItem)rotorsamount.SelectedValue).Content);
                for (int i = 0; i < rotorsAmount; i++)
                {
                    if (!FileWorker.checkLayout(rotorPresentations[i].layout.Text) ||
                        !FileWorker.checkNotch(rotorPresentations[i].notch.Text) ||
                        !FileWorker.checkOffset(rotorPresentations[i].offset.Text))
                    {
                        cmdCompute.IsEnabled = false;
                        return;
                    }
                }
                if (!FileWorker.checkLayout(rotorPresentations[10].layout.Text) || 
                    !FileWorker.checkStopWord(stopword.Text) || !FileWorker.checkMessage(message.Text))
                {
                    cmdCompute.IsEnabled = false;
                    return;
                }

                for (int i = rotorsAmount; i < 10; i++)
                {
                    if (!FileWorker.checkLayout(rotorPresentations[i].layout.Text))
                        rotorPresentations[i].layout.Text = Settings.rotorsLayout[i];
                    if (!FileWorker.checkNotch(rotorPresentations[i].notch.Text))
                        rotorPresentations[i].notch.Text = Settings.notchPositions[i].ToString();
                    if (!FileWorker.checkNotch(rotorPresentations[i].notch.Text))
                        rotorPresentations[i].offset.Text = "0";
                }
                cmdCompute.IsEnabled = true;
                scheduler.getEnigmaConfiguration();
                FileWorker.writeSettingsFile(rotorsAmount, rotorPresentations, 
                        stopword.Text, message.Text);
            }
        }

        protected void cmdCompute_Click(object sender, System.EventArgs e)
        {
            scheduler.startBreaking();
        }

        protected void cmdEncrypt_Click(object sender, System.EventArgs e)
        {
            if (!checkInputMessage(encryptinput.Text))
            {
                encryptoutput.Text = "Input message is incorrect";
            }
            else
            {
                int rotorsAmount = Byte.Parse(rotorsamount.Text);
                EnigmaCryptography.Enigma enigma = 
                    new EnigmaCryptography.Enigma(rotorsAmount, getRotorOffsets());
                enigma.changeEnigmaStructure(getRotorsLayout(), getNotchPositions());
                enigma.initialize();
                encryptoutput.Text = enigma.encrypt(encryptinput.Text);
            }
        }

        protected byte[] getRotorOffsets()
        {
            int rotorsAmount = Byte.Parse(rotorsamount.Text);
            byte[] array = new byte[rotorsAmount];
            for (int i = 0; i < rotorsAmount; i++)
            {
                array[i] = Byte.Parse(rotorPresentations[i].offset.Text);
            }
            return array;
        }

        protected void rotorsAmountChanged(object sender, System.EventArgs e)
        {
            int rotorsAmount = Int32.Parse((string)((ComboBoxItem)rotorsamount.SelectedValue).Content);
            for (int i = 0; i < rotorsAmount; i++)
            {
                rotorPresentations[i].pos.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].layout.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].notch.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].rot.Visibility = System.Windows.Visibility.Visible;
                rotorPresentations[i].offset.Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = rotorsAmount; i < 10; i++)
            {
                rotorPresentations[i].pos.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].layout.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].notch.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].rot.Visibility = System.Windows.Visibility.Collapsed;
                rotorPresentations[i].offset.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        protected void setRotorPresentations()
        {
            rotorPresentations = new GUIRotorPresentation[11];
            rotorPresentations[0] = new GUIRotorPresentation(pos1, layout1, notch1, rot1, offset1);
            rotorPresentations[1] = new GUIRotorPresentation(pos2, layout2, notch2, rot2, offset2);
            rotorPresentations[2] = new GUIRotorPresentation(pos3, layout3, notch3, rot3, offset3);
            rotorPresentations[3] = new GUIRotorPresentation(pos4, layout4, notch4, rot4, offset4);
            rotorPresentations[4] = new GUIRotorPresentation(pos5, layout5, notch5, rot5, offset5);
            rotorPresentations[5] = new GUIRotorPresentation(pos6, layout6, notch6, rot6, offset6);
            rotorPresentations[6] = new GUIRotorPresentation(pos7, layout7, notch7, rot7, offset7);
            rotorPresentations[7] = new GUIRotorPresentation(pos8, layout8, notch8, rot8, offset8);
            rotorPresentations[8] = new GUIRotorPresentation(pos9, layout9, notch9, rot9, offset9);
            rotorPresentations[9] = new GUIRotorPresentation(pos10, layout10, notch10, rot10, offset10);
            rotorPresentations[10] = new GUIRotorPresentation(pos11, layout11, null, null, null);
            rotorsamount.SelectedItem = rotorsamount.Items[2];
        }

        protected void setRotorContent()
        {
            FileWorker fworker = new FileWorker();
            enigmaSettings = fworker.getEnigmaSettings();
            rotorsamount.Text = enigmaSettings[0];
            for (int i = 0; i < 10; i++)
            {
                rotorPresentations[i].layout.Text = enigmaSettings[i * 3 + 1];
                rotorPresentations[i].notch.Text = enigmaSettings[i * 3 + 2];
                rotorPresentations[i].offset.Text = enigmaSettings[i * 3 + 3];
            }
            rotorPresentations[10].layout.Text = enigmaSettings[31];
            stopword.Text = enigmaSettings[32];
            message.Text = enigmaSettings[33];
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

        protected void validateStopWord(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkStopWord(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        protected void validateMessage(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkMessage(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        protected void validateOffset(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkOffset(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        protected bool checkInputMessage(string s)
        {
            foreach (char ch in s)
            {
                if ((ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && (ch != ' '))
                {
                    return false;
                }
            }
            return true;
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

        public string getStopWord()
        {
            return stopword.Text;
        }

        public string getMessage()
        {
            return message.Text;
        }
    }
}
