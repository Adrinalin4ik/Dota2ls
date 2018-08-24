using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dota2ls
{
    static class Settings
    {
        public static void SaveAccount(string str)
        {
            StreamWriter sw = new StreamWriter("Account.player");
            sw.WriteLine(str);
            sw.Close();
        }
        public static string GetAccountId()
        {
            StreamReader sr = new StreamReader("Account.player");
            string AccountId = sr.ReadLine();
            sr.Close();
            return AccountId;
        }
        public static void debugSavedGames()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"/matches");
            foreach (var file in dir.GetFiles())
                {
                if (file.Length < 10 && file.Extension!=".stat") file.Delete();
                }
        }
        public static bool checkoutFiles()
        {
            return true;
        }
    }
}
