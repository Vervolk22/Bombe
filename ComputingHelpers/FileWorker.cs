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
                settings[i * 2] = LibrarySettings.rotorsLayout[i];
                settings[i * 2 + 1] = LibrarySettings.notchPositions[i].ToString();
                writer.WriteLine(settings[i * 2]);
                writer.WriteLine(settings[i * 2 + 1]);
            }
            settings[20] = LibrarySettings.rotorsLayout[10];
            writer.WriteLine(settings[20]);
            writer.Flush();
            writer.Close();
            return settings;
        }

        protected string getAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }
    }
}
