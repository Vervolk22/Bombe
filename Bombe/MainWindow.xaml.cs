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
        private GUIRotorPresentation[] rotorPresentations;
        private string[] enigmaSettings;

        protected SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        protected SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Bridge.setWindow(this);
            new ComputingScheduler();
            makePreparations();
            //scheduler.run();
        }

        /// <summary>
        /// Some preparations, before scheduler starts.
        /// </summary>
        protected void makePreparations()
        {
            iplabel.Content = Bridge.computingSide.getLocalIP();
            setRotorPresentations();
            setRotorContent();
        }

        /// <summary>
        /// Handler, to start/stop receive connections button.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
        protected void cmdListen_Click(object sender, System.EventArgs e)
        {
            Bridge.computingSide.changeServerStatus();
        }

        /// <summary>
        /// If breaking or test tabs are chosen, check current Enigma
        /// configuration.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
        protected void tabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab1.IsSelected || tab4.IsSelected)
            {
                // Check rotors layout.
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
                // Check reflector's layout, stop word and encrypted message.
                if (!FileWorker.checkLayout(rotorPresentations[10].layout.Text) || 
                    !FileWorker.checkStopWord(stopword.Text) || !FileWorker.checkMessage(message.Text))
                {
                    cmdCompute.IsEnabled = false;
                    return;
                }

                // If some unused rotors are incorrect, set them to default state.
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
                Bridge.computingSide.getEnigmaConfiguration();
                FileWorker.writeSettingsFile(rotorsAmount, rotorPresentations, 
                        stopword.Text, message.Text);
            }
        }

        /// <summary>
        /// Handler, to start breaking process.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
        protected void cmdCompute_Click(object sender, System.EventArgs e)
        {
            Bridge.computingSide.startBreaking();
        }

        /// <summary>
        /// Handler, to encrypte/decrypt message with current Enigma configuration.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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
                message.Text = encryptoutput.Text;
            }
        }

        /// <summary>
        /// Get rotors offsets from TextBoxes.
        /// </summary>
        /// <returns>Byte array with rotor offsets.</returns>
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

        /// <summary>
        /// Handler, to change amount of showing rotors, accordingly to 
        /// ComboBox chosen value.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Set the array of GUIRotorPresentations.
        /// </summary>
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

        /// <summary>
        /// Set content of rotor's textboxes with Enigma settings from 
        /// FileWorker class.
        /// </summary>
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

        /// <summary>
        /// Append some text to mainlist.
        /// </summary>
        /// <param name="s">String to append.</param>
        public void mainListAppendText(string s)
        {
            mainlist.AppendText(s);
        }

        /// <summary>
        /// Handler, to validate the rotor's layout.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Handler, to validate the reflector's layout.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
        protected void validateReflector(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!FileWorker.checkReflector(tb.Text))
            {
                tb.BorderBrush = redBrush;
            }
            else
            {
                tb.BorderBrush = greenBrush;
            }
        }

        /// <summary>
        /// Handler, to validate notch position.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Handler, to validate the stop word.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Handler, to validate encrypted message.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Handler, to validate the rotor's offset.
        /// </summary>
        /// <param name="sender">Button control object.</param>
        /// <param name="e">EventArgs object of event.</param>
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

        /// <summary>
        /// Check the encrypted message. It allows only lowercase and uppercase
        /// english letters.
        /// </summary>
        /// <param name="s">Encrypted message.</param>
        /// <returns>Result ofcheck.</returns>
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

        /// <summary>
        /// Get string array of rotors layouts.
        /// </summary>
        /// <returns>Array of rotors layouts.</returns>
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

        /// <summary>
        /// Get notch positions of rotos.
        /// </summary>
        /// <returns>Array of notch positions.</returns>
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

        /// <summary>
        /// Get stop word from the MainWindow.
        /// </summary>
        /// <returns></returns>
        public string getStopWord()
        {
            return stopword.Text;
        }

        /// <summary>
        /// Get the encrypted message.
        /// </summary>
        /// <returns></returns>
        public string getMessage()
        {
            return message.Text;
        }
    }
}
