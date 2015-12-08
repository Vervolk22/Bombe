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
                string[] settings = new string[22];
                for (int i = 0; i < 22; i++)
                {
                    settings[i] = reader.ReadLine();
                }
                reader.Close();

                int rotorsAmount = Byte.Parse(settings[0]);
                if (rotorsAmount < 3 || rotorsAmount > 10)
                    return writeDefaultSettingsFile();
                for (int i = 0; i < 10; i++)
                {
                    if (!checkLayout(settings[i * 2 + 1]) || !checkNotch(settings[i * 2 + 2]))
                        return writeDefaultSettingsFile();
                }
                if (!checkLayout(settings[21]))
                    return writeDefaultSettingsFile();
                return settings;
            }
            catch (IOException e)
            {
                return writeDefaultSettingsFile();
            }
        }

        public static void writeSettingsFile(int rotorsAmount, GUIRotorPresentation[] presentations)
        {
            StreamWriter writer = new StreamWriter("EnigmaSettings.txt");
            writer.WriteLine(rotorsAmount);
            for (int i = 0; i < 10; i++)
            {
                writer.WriteLine(presentations[i].layout.Text);
                writer.WriteLine(presentations[i].notch.Text);
            }
            writer.WriteLine(presentations[10].layout.Text);
            writer.Flush();
            writer.Close();
        }

        protected string[] writeDefaultSettingsFile()
        {
            StreamWriter writer = new StreamWriter("EnigmaSettings.txt");
            string[] settings = new string[22];
            settings[0] = "5";
            writer.WriteLine("5");
            for (int i = 0; i < 10; i++)
            {
                settings[i * 2 + 1] = Settings.rotorsLayout[i];
                settings[i * 2 + 2] = Settings.notchPositions[i].ToString();
                writer.WriteLine(settings[i * 2]);
                writer.WriteLine(settings[i * 2 + 1]);
            }
            settings[21] = Settings.rotorsLayout[10];
            writer.WriteLine(settings[21]);
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

        public static bool checkNotch(string s)
        {
            if (s.Length == 1 && (char)s[0] >= 'A' && (char)s[0] <= 'Z') return true;
            else return false;
        }
    }
}
