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
    public class FileWorker
    {
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

        protected string[] readSettingsFile()
        {
            try
            {
                StreamReader reader = new StreamReader("EnigmaSettings.txt");
                string[] settings = new string[34];
                for (int i = 0; i < 34; i++)
                {
                    settings[i] = reader.ReadLine();
                }
                reader.Close();

                byte rotorsAmount;
                if (!Byte.TryParse(settings[0], out rotorsAmount))
                {
                    return writeDefaultSettingsFile();
                }
                if (rotorsAmount < 3 || rotorsAmount > 10)
                    return writeDefaultSettingsFile();
                for (int i = 0; i < 10; i++)
                {
                    if (!checkLayout(settings[i * 3 + 1]) || !checkNotch(settings[i * 3 + 2]) ||
                        !checkOffset(settings[i * 3 + 3]))
                        return writeDefaultSettingsFile();
                }
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

        protected string[] writeDefaultSettingsFile()
        {
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

        public static bool checkLayout(string s)
        {
            if (s.Length != Settings.ALPHABET_LENGTH) return false;
            byte[] array = new byte[Settings.ALPHABET_LENGTH];
            int counter = 0;
            try
            {
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

        public static bool checkReflector(string s)
        {
            if (!checkLayout(s))
                return false;

            for (int i = 0; i < Settings.ALPHABET_LENGTH; i++)
            {
                if (('A' + i) != s[s[i] - 'A'])
                    return false;
            }

            return true;
        }

        public static bool checkNotch(string s)
        {
            if (s.Length == 1 && (char)s[0] >= 'A' && (char)s[0] <= 'Z') return true;
            else return false;
        }

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
