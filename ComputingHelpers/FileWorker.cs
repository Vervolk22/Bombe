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
                return writeSettingsFile();
            }
        }

        protected string[] readSettingsFile()
        {
            try
            {
                StreamReader reader = new StreamReader("EnigmaSettings.txt");
                string[] settings = new string[21];
                for (int i = 0; i < 21; i++)
                {
                    settings[i] = reader.ReadLine();
                }
                reader.Close();

                for (int i = 0; i < 10; i++)
                {
                    if (!checkLayout(settings[i * 2]) || !checkNotch(settings[i * 2 + 1]))
                        return writeSettingsFile();
                }
                if (!checkLayout(settings[20]))
                    return writeSettingsFile();
                return settings;
            }
            catch (IOException e)
            {
                return writeSettingsFile();
            }
        }

        protected string[] writeSettingsFile()
        {
            StreamWriter writer = new StreamWriter("EnigmaSettings.txt");
            string[] settings = new string[21];
            for (int i = 0; i < 10; i++)
            {
                settings[i * 2] = Settings.rotorsLayout[i];
                settings[i * 2 + 1] = Settings.notchPositions[i].ToString();
                writer.WriteLine(settings[i * 2]);
                writer.WriteLine(settings[i * 2 + 1]);
            }
            settings[20] = Settings.rotorsLayout[10];
            writer.WriteLine(settings[20]);
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
