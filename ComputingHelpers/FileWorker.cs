using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;

namespace ComputingHelpers
{
    /// <summary>
    /// Class to read/write last used Enigma configuration to save file.
    /// </summary>
    public class FileWorker
    {
        /// <summary>
        /// Gets last used Enigma settings. If save file is missing/incorrect,
        /// creates  file with default settings, and returns it.
        /// </summary>
        /// <returns></returns>
        public string[] getEnigmaSettings()
        {
            FileInfo info = new FileInfo("EnigmaSettings.txt");
            if (info.Exists)
            {
                return readSettingsFile();
            }
            else
            {
                return writeDefaultSettingsFile();
            }
        }

        /// <summary>
        /// Read Enigma settings file.
        /// </summary>
        /// <returns>Resulting Enigma settings.</returns>
        protected string[] readSettingsFile()
        {
            try
            {
                // Read settings from file.
                StreamReader reader = new StreamReader("EnigmaSettings.txt");
                string[] settings = new string[34];
                for (int i = 0; i < 34; i++)
                {
                    settings[i] = reader.ReadLine();
                }
                reader.Close();

                byte rotorsAmount;
                // If file's structure is incorrect, write default settings,
                // and return them:
                // If rotor's amount is incorrect.
                if (!Byte.TryParse(settings[0], out rotorsAmount))
                {
                    return writeDefaultSettingsFile();
                }
                if (rotorsAmount < 3 || rotorsAmount > 10)
                    return writeDefaultSettingsFile();
                // If rotor's layout, notch and offset are incorrect.
                for (int i = 0; i < 10; i++)
                {
                    if (!checkLayout(settings[i * 3 + 1]) || !checkNotch(settings[i * 3 + 2]) ||
                        !checkOffset(settings[i * 3 + 3]))
                        return writeDefaultSettingsFile();
                }
                // If reflector's layout, stop word and encrypted message 
                // are incorrect.
                if (!checkLayout(settings[31]) || !checkStopWord(settings[32]) ||
                        !checkMessage(settings[33]))
                    return writeDefaultSettingsFile();
                return settings;
            }
            catch (IOException e)
            {
                return writeDefaultSettingsFile();
            }
        }

        /// <summary>
        /// Write last used Enigma settings to the file.
        /// </summary>
        /// <param name="rotorsAmount">Amount of rotors used.</param>
        /// <param name="presentations">Array that contains references to controls
        /// with settings.</param>
        /// <param name="stopWord">Last used stop word/</param>
        /// <param name="message">Last used encrypted message.</param>
        public static void writeSettingsFile(int rotorsAmount, GUIRotorPresentation[] presentations,
                string stopWord, string message)
        {
            StreamWriter writer = new StreamWriter("EnigmaSettings.txt");
            writer.WriteLine(rotorsAmount);
            for (int i = 0; i < 10; i++)
            {
                writer.WriteLine(presentations[i].layout.Text);
                writer.WriteLine(presentations[i].notch.Text);
                writer.WriteLine(presentations[i].offset.Text);
            }
            writer.WriteLine(presentations[10].layout.Text);
            writer.WriteLine(stopWord);
            writer.WriteLine(message);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Write default Enigma settings to the file.
        /// </summary>
        /// <returns>Default Enigma settings.</returns>
        protected string[] writeDefaultSettingsFile()
        {
            // Structure of the file:
            // 1:              rotors amount;
            // i * 3 + 1:      layout of i's rotor;
            // i * 3 + 2:      notch position of i's rotor;
            // i * 3 + 3:      offset of i's rotor;
            // lastString - 2: reflector's layout;
            // lastString - 1: stop word;
            // lastString:     encrypted message.
            StreamWriter writer = new StreamWriter("EnigmaSettings.txt");
            string[] settings = new string[34];
            settings[0] = "5";
            writer.WriteLine(settings[0]);
            for (int i = 0; i < 10; i++)
            {
                settings[i * 3 + 1] = Settings.rotorsLayout[i];
                settings[i * 3 + 2] = Settings.notchPositions[i].ToString();
                settings[i * 3 + 3] = "0";
                writer.WriteLine(settings[i * 3 + 1]);
                writer.WriteLine(settings[i * 3 + 2]);
                writer.WriteLine(settings[i * 3 + 3]);
            }
            settings[31] = Settings.rotorsLayout[10];
            writer.WriteLine(settings[31]);
            settings[32] = Settings.stopWord;
            writer.WriteLine(settings[32]);
            settings[33] = Settings.message;
            writer.WriteLine(settings[33]);
            writer.Flush();
            writer.Close();
            return settings;
        }

        /// <summary>
        /// Checks, if the layout of rotor is correct.
        /// </summary>
        /// <param name="s">Rotor's layout.</param>
        /// <returns>Result of check.</returns>
        public static bool checkLayout(string s)
        {
            if (s.Length != Settings.ALPHABET_LENGTH) return false;
            byte[] array = new byte[Settings.ALPHABET_LENGTH];
            int counter = 0;
            try
            {
                // Check, if all letters are in the layout exactly only 
                // a single time;
                foreach (char ch in s)
                {
                    if (array[ch - 'A'] == 0)
                    {
                        counter++;
                        array[ch - 'A']++;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Check, if all the letters was encountered.
                if (counter == Settings.ALPHABET_LENGTH)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Check, if the reflector's layout is correct.
        /// </summary>
        /// <param name="s">Reflector's layout.</param>
        /// <returns>Result of check.</returns>
        public static bool checkReflector(string s)
        {
            if (!checkLayout(s))
                return false;

            // Check, if letters in the layout are symmetric, as
            // reflector require.
            for (int i = 0; i < Settings.ALPHABET_LENGTH; i++)
            {
                if (('A' + i) != s[s[i] - 'A'])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check, if the notch position is correct.
        /// </summary>
        /// <param name="s">Notch position.</param>
        /// <returns>Result of check.</returns>
        public static bool checkNotch(string s)
        {
            if (s.Length == 1 && (char)s[0] >= 'A' && (char)s[0] <= 'Z') return true;
            else return false;
        }

        /// <summary>
        /// Check, if the stop word is correct.
        /// </summary>
        /// <param name="s">Stop word.</param>
        /// <returns>Result of check.</returns>
        public static bool checkStopWord(string s)
        {
            if (s.Length < 5 || s.Length > 20)
            {
                return false;
            }
            foreach (char ch in s)
            {
                if ((ch < 'A' || ch > 'Z') && ch != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check, if the encrypted message is correct.
        /// </summary>
        /// <param name="s">Encrypted message.</param>
        /// <returns>Result of check.</returns>
        public static bool checkMessage(string s)
        {
            if (s.Length < 10 || s.Length > 400)
            {
                return false;
            }
            foreach (char ch in s)
            {
                if ((ch < 'A' || ch > 'Z') && ch != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check, if the rotor's offset is correct.
        /// </summary>
        /// <param name="s">Rotor's offset.</param>
        /// <returns>Result of check.</returns>
        public static bool checkOffset(string s)
        {
            byte offset;
            if (Byte.TryParse(s, out offset))
            {
                if (offset < 0 || offset > Settings.ALPHABET_LENGTH - 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
