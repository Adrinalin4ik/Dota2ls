using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace StatisticLib
{
    public class Helper
    {

            public static string ItemsDataString;
            public static string HeroesDataString;

            public static void LoadItems()
            {
                string getItemsList = "http://www.dota2.com/jsfeed/itemdata?l=ru";
                DownloadFile(getItemsList, "data/itemdata.json");
                StreamReader r = new StreamReader("data/itemdata.json");
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getItemsList);// Веб запрос к нашему серверу
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера
                //StreamReader r = new StreamReader(response.GetResponseStream());
                ItemsDataString = r.ReadToEnd();
                r = new StreamReader("Image/Heroes/heroes.json");
                HeroesDataString = r.ReadToEnd();
                r.Close();
                UpdateDataBase();


            }
            public static string GetItemAttribute(string item_id, string attr)
            {
                JObject itemsData = JObject.Parse(ItemsDataString);
                //foreach (JToken j in itemsData["itemdata"].Children().Children())
                //JToken j = null;
                string value = "Empty";
                Parallel.ForEach(itemsData["itemdata"].Children().Children(), j =>
                {
                    string t = j["id"].ToString();
                    if (j["id"].ToString().Equals(item_id))
                    {
                        value = j[attr].ToString();
                    }
                });
                return value;

            }

            public static string getHeroName(string id)
            {
                string heroName = "Герой " + id + " не найден";
                JObject HeroesData = JObject.Parse(HeroesDataString);
               
               while (!Parallel.ForEach(HeroesData["result"]["heroes"], j => {
                   //foreach(var j in HeroesData.Children().Children().Children()) { 
                   Console.WriteLine(j);
                    if (j["id"].ToString().Equals(id))
                    {
                        Console.WriteLine(j["id"].ToString());
                        try
                        {
                            heroName = j["localized_name"].ToString();
                           // break;
                        }
                        catch (Exception) { }
                    }
               }).IsCompleted)
                {
                    Thread.Sleep(100);
                }
                if (heroName == "Герой " + id + " не найден")
                {
                    UpdateHeroesJSON();
                    getHeroName(id);
                }
                return heroName;
            }
        public static string translateItemStats(string attributes)
        {
            string pattern = "\">(.*)";
            int k = 0;
            string result = null;
            string[] attr = attributes.Split('<');
            for (int i = 0; i < attr.Length; i++)
            {
                var match = Regex.Match(attr[i], pattern);
                if (match.Success && match.Value != " ")
                {
                    k++;
                    //Console.WriteLine("0 "+match.Groups[0].Value);
                    //Console.Write(match.Groups[1].Value);
                    result += match.Groups[1].Value.Replace('\n',' ');
                    //Console.WriteLine("3 " + match.Groups[3].Value);
                    //System.Console.WriteLine("2 " + match.Groups[2].Value);
                    //Console.Write(match.Groups[2].Value.Split('>').Last().Split('<').First());
                    if (k % 2 == 0 && i >= 2)
                        result += "\n";
                }
            }
            return result;
        }
        public static void UpdateHeroesImageBase()
            {
                string Heroes_img_path = "http://cdn.dota2.com/apps/dota2/images/heroes/";
                JObject HeroesData = JObject.Parse(HeroesDataString);
            
                Parallel.ForEach(HeroesData["result"]["heroes"], j => {
                    Console.WriteLine(j);
                    if (!File.Exists("Image/Heroes/" + j["id"].ToString() + ".png"))
                    {
                        Console.WriteLine(j["id"].ToString());
                        try
                        {
                            DownloadFile(Heroes_img_path + j["localized_name"].ToString().Replace(' ','_').ToLower() + "_lg.png", "Image/Heroes/" + j["id"].ToString() + ".png");
                        }
                        catch (Exception) { }
                    }
                });
            }

        public static void UpdateHeroesJSON()
        {
            string Heroes_json_path = "http://api.steampowered.com/IEconDOTA2_570/GetHeroes/v1/?key=833F0F7612795A2950FB524392CD07E4&language=ru";
            DownloadFile(Heroes_json_path, "Image/Heroes/heroes.json");
            UpdateHeroesImageBase();
        }
        public static void UpdateDataBase()
            {
                string Items_img_path = "http://media.steampowered.com/apps/dota2/images/items/";

                JObject itemsData = JObject.Parse(ItemsDataString);

            Parallel.ForEach(itemsData["itemdata"].Children().Children(), j =>
            //foreach (var j in itemsData["itemdata"].Children().Children())
            {
                    if (!File.Exists("Image/Items/" + j["id"].ToString() + ".png"))
                    {
                        DownloadFile(Items_img_path + j["img"].ToString(), "Image/Items/" + j["id"].ToString() + ".png");
                    }

                    if (!File.Exists("ToolTips/ToolTip" + j["id"].ToString() + ".tdb"))
                    {
                        StreamWriter sw = File.CreateText("ToolTips/ToolTip" + j["id"].ToString() + ".tdb");
                        sw.WriteLine(j["dname"].ToString());
                        sw.WriteLine("Coast " + j["cost"].ToString() + "\n");

                        //sw.WriteLine(j["desc"].ToString() + "\n");
                        //sw.WriteLine(j["components"].ToString() == "" ? "" : "Components:\n" + j["components"].ToString());
                        sw.Write((j["cd"].ToString() == "False" ? "" : ("Cooldown " + j["cd"].ToString()) + " sec"));

                        sw.Close();
                        //DownloadFile(img_path + j["img"].ToString(), "Image/Heroes/" + j["id"].ToString() + ".png");

                    }
                });

                UpdateHeroesImageBase();
            }

            public static JToken getItemTooltip(string itemId)
            {
            var ItemsDataString = File.ReadAllText("data/itemdata.json");
            JToken jObj = null;
            JObject itemsData = JObject.Parse(ItemsDataString);

            Parallel.ForEach(itemsData["itemdata"].Children().Children(), j =>
            {
                if (j["id"].ToString().Equals(itemId))
                {
                    jObj = j;
                }
            });

            return jObj;
            }

        public static void DownloadFile(String Url, String ResultFileName)
        {
            try
            {
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(Url);
                HttpWebResponse ws = (HttpWebResponse)wr.GetResponse();
                Stream str = ws.GetResponseStream();

                byte[] inBuf = new byte[100000];
                int bytesReadTotal = 0;

                FileStream fstr = new FileStream(ResultFileName, FileMode.Create, FileAccess.Write);

                while (true)
                {
                    int n = str.Read(inBuf, 0, 100000);
                    if ((n == 0) || (n == -1))
                    {
                        break;
                    }

                    fstr.Write(inBuf, 0, n);

                    bytesReadTotal += n;
                }

                str.Close();
                fstr.Close();
            }
            catch (Exception) { }

        }
    }
}
