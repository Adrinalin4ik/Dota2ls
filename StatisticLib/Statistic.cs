using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace StatisticLib
{
    public struct Player
    {
        //Игрок
        public string id;
        public string slot;
        public string heroId;
        public string heroLevel;
        public string heroKills;
        public string heroDeath;
        public string heroAssist;
        public string heroGolds;
        public string heroLH;
        public string heroDN;
        public string heroXPM;
        public string heroGPM;
        public string heroDMG;
        public string heroTDMG;
        public string heroTotalGold;
        public string itemsID0;
        public string itemsID1;
        public string itemsID2;
        public string itemsID3;
        public string itemsID4;
        public string itemsID5;
        public string leaveStatus;

        //Матч

    }

    public class Statistic
    {
        public static string AccountId = null;
        public static List<string> last10MatchId;
        public static List<string> last25MatchId;
        public string heroResult; //результат true=radiant win 
        public string matchTime;
        public string gameMod;
        public string positiveVotes;
        public string negativeVotes;
        public string lobbyType;
        public string towerStatusRadiant; //состояния бараков и башен
        public string towerStatusDire;
        public string barrackStatusRadiant;
        public string barrackStatusDire;

        public Player[] p;
        private string playerID;
        /// <summary>
        /// Запрос на получение id последнего матча
        /// </summary>
        private string lastMatchRequest = "https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001/?matches_requested=1&key=833F0F7612795A2950FB524392CD07E4&format=xml&account_id=";
        /// <summary>
        /// Запрос на получение полной информации о матче
        /// </summary>
        private string lastMatchFullStatisticUrl = "http://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/V1/?language=en_us&format=xml&key=833F0F7612795A2950FB524392CD07E4&match_id=";// + LastMatchID;
        /// <summary>
        /// Запрос на получение id последних 10 матчей
        /// </summary>
        private string last10MatchURL =   "https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001/?matches_requested=10&key=833F0F7612795A2950FB524392CD07E4&format=xml&account_id=";
        /// <summary>
        /// Запрос на получение id последних 25 матчей
        /// </summary>
        private string last25MatchURL = "https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001/?matches_requested=25&key=833F0F7612795A2950FB524392CD07E4&format=xml&account_id=";
        private string SpecialMatchUrl = "https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/V001/?key=833F0F7612795A2950FB524392CD07E4&format=xml&match_id=";
        /// <summary>
        /// Id последнего матча
        /// </summary>
        public static string lastMatchId;
        /// <summary>
        /// Стэк для хранения последних матчей
        /// </summary>
        public static Stack<string> lastMatchStatistics;

        public bool isSelectedPlayer;
        ////////////////////////////////////////////////////////
     
        public Statistic(string _playerID)
        {
            playerID = _playerID;
            p = new Player[10];
            last10MatchId = new List<string>();
            last25MatchId = new List<string>();
            lastMatchStatistics = new Stack<string>();
        }
        public Player[] Players
        {
            get { return p; }
        }
        

        private Player GetMe()//ссылка на структуру текущего игрока
        {
            foreach (Player player in p)
            {
                if (player.id == playerID) return player;
            }
            return p[0];
        }
        /// <summary>
        /// Получение номера слота 
        /// </summary>
        /// <returns></returns>
        public int GetMeIndex()//индекс плеера текущего игрока 0-9
        {
            int i = 0;
            foreach (Player player in p)
            {  
                if (player.id == playerID) return i;
                i++;
            }
            return 0;
        }
        /// <summary>
        /// Получение id последних 10 матчей с последующим их в список last10MatchId
        /// </summary>
        public void Get10StatisticId()
        {
            try
            {
                string FileURL = last10MatchURL + playerID;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FileURL);// Веб запрос к нашему серверу
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера
                XmlDocument xd = new XmlDocument();
                xd.Load(response.GetResponseStream());
                XmlNodeList list = xd.GetElementsByTagName("match");
                if (list.Count > 0)
                {
                    Parallel.For(0, list.Count, i =>
                    {
                        XmlElement node = (XmlElement)xd.GetElementsByTagName("match_id")[i];
                        FileURL = node.InnerText;  
                        last10MatchId.Add(FileURL);
                    });

                }
                lastMatchId = last10MatchId[0];
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        /// <summary>
        /// Получение последних 25 матчей
        /// </summary>
        public void save_last_25_matches()
        {
            try
            {
                string FileURL = last25MatchURL + AccountId;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FileURL);// Веб запрос к нашему серверу
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера
                XmlDocument xd = new XmlDocument();
                xd.Load(response.GetResponseStream());
                XmlNodeList list = xd.GetElementsByTagName("match");
                if (list.Count > 0)
                {
                    //Parallel.For(0, list.Count, i =>
                    for(int i = 0; i<list.Count; i++)
                    {
                        XmlElement node = (XmlElement)xd.GetElementsByTagName("match_id")[i];

                    //last25MatchId.Add(node.InnerText);
                      reload_special:;
                        try {

                            string matchId = node.InnerText;
                            string fileURL = SpecialMatchUrl + matchId;
                            HttpWebRequest request_match = (HttpWebRequest)WebRequest.Create(fileURL);// Веб запрос к нашему серверу
                            HttpWebResponse response_match = (HttpWebResponse)request_match.GetResponse(); // Ответ сервера
                            XmlDocument xd1 = new XmlDocument();
                            xd1.Load(response_match.GetResponseStream());
                            //File.Create("matches/" + matchId + ".xml");
                            StreamWriter sw = new StreamWriter("matches/" + matchId + ".xml");
                            xd1.Save(sw);
                            sw.Close();
                            XmlNodeList list1 = xd1.GetElementsByTagName("player");
                            if (list1.Count > 0)
                            {
                                Parallel.For(0, list1.Count, i1 =>//Инициализация 
                                                                  //for (int i1 = 0; i1 < list1.Count; i1++)
                                {
                                    string threadSt;//инициализация нового экземпляра строковой переменной для каждого потока
                                    XmlElement node1 = (XmlElement)xd1.GetElementsByTagName("hero_id")[i1];
                                    threadSt = node1.InnerText; // Считывае ID  
                                    p[i1].heroId = threadSt; // Добавляем его в массив 

                                    node1 = (XmlElement)xd1.GetElementsByTagName("account_id")[i1];
                                    threadSt = node1.InnerText;
                                    if (threadSt != "4294967295")
                                    {
                                        p[i1].id = threadSt;
                                    }
                                    else p[i1].id = "Anonymous";

                                    node1 = (XmlElement)xd1.GetElementsByTagName("player_slot")[i1];
                                    threadSt = node1.InnerText;
                                    p[i1].slot = threadSt;
                                });
                                XmlElement node3 = (XmlElement)xd1.GetElementsByTagName("radiant_win")[0];
                                heroResult = node3.InnerText;
                            }
                            
                            Player player = GetMe();
                            if (!File.Exists("matches/matches_" + AccountId + ".stat"))
                            {
                                File.Create("matches/matches_" + AccountId + ".stat").Close();
                            }
                            
                            if (Convert.ToInt32(player.slot) <= 5 && heroResult == "true" || Convert.ToInt32(player.slot) > 5 && heroResult == "false")
                            {
                                // sw1.WriteLine("Win," + TranslateHeroName(player.heroId) + "," + matchId);
                                last25MatchId.Add("Win," + Helper.getHeroName(player.heroId) + "," + matchId);
                            }
                            else //sw1.WriteLine("Loose," + TranslateHeroName(player.heroId) + "," + matchId);
                                last25MatchId.Add("Loose," + Helper.getHeroName(player.heroId) + "," + matchId);
                           
                        }
                        catch (Exception) { goto reload_special; }

                    }
                    last25MatchId.Reverse();
                    StreamWriter sw1 = File.CreateText("matches/matches_" + AccountId + ".stat");
                    foreach (var stat in last25MatchId)
                        sw1.WriteLine(stat);
                    sw1.Close();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        /// <summary>
        /// Получение id последнего матча
        /// </summary>
        /// <returns>match Id or null</returns>
        public string GetLastMatchId()
        {
            try
            {
                string s;
                string FileURL = lastMatchRequest + playerID;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FileURL);// Веб запрос к нашему серверу
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера
                XmlDocument xd = new XmlDocument();
                xd.Load(response.GetResponseStream());
                XmlNodeList list = xd.GetElementsByTagName("match");
                if (list.Count > 0)
                {
                        XmlElement node = (XmlElement)xd.GetElementsByTagName("match_id")[0];
                        s = node.InnerText;
                    return lastMatchId = s;
                }
                return null;
            }
            catch (Exception ex) {
                //return GetLastMatchId();
                return lastMatchId;
                 //throw new Exception(ex.Message);
            }
        }

        public bool Connection()
        {
            string FileURL = lastMatchRequest + playerID;
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FileURL);// Веб запрос к нашему серверу
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Close();
                return true;
            }
            }
            catch (Exception) { return false; }
            
            return false;
        }

        /// <summary>
        ///  Получение статистики
        /// </summary>
        /// <param name="matchId">id матча</param>
        /// <param name="isHistory">Флаг истории</param>
        public void GetStatistic(string matchId,bool isHistory)
        {

                string st;
                XmlDocument xd = new XmlDocument();

                string fileURL = lastMatchFullStatisticUrl + matchId;

                try
                {
                    if (!File.Exists("matches/" + matchId + ".xml"))
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileURL);// Веб запрос к нашему серверу
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера
                        xd.Load(response.GetResponseStream());
                        if (!isSelectedPlayer)
                        {
                            StreamWriter sw = new StreamWriter("matches/" + matchId + ".xml");
                            xd.Save(sw);
                            sw.Close();
                        }
                        response.Close();
                    }
                    else
                    {
                        xd.Load("matches/" + matchId + ".xml");
                        isHistory = true;
                    }

                    if (xd.GetElementsByTagName("error").Count > 0)
                    {
                        Get10StatisticId();
                        lastMatchId = last10MatchId[2];
                        GetStatistic(matchId, false);
                    }
                    XmlNodeList list = xd.GetElementsByTagName("player");
                    if (list.Count > 0)
                    {
                        /////////////////////////////////////
                        if (Parallel.For(0, list.Count, i =>//Инициализация 
                                                            //for (int i = 0; i < list.Count; i++)
                        {
                            string threadSt;//инициализация нового экземпляра строковой переменной для каждого потока
                        XmlElement node = (XmlElement)xd.GetElementsByTagName("hero_id")[i];
                            threadSt = node.InnerText; // Считывае ID  
                            p[i].heroId = threadSt; // Добавляем его в массив  

                            node = (XmlElement)xd.SelectNodes("result/players/player/level")[i];
                            threadSt = node.InnerText;
                            p[i].heroLevel = threadSt;


                            node = (XmlElement)xd.GetElementsByTagName("kills")[i];
                            threadSt = node.InnerText;
                            p[i].heroKills = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("deaths")[i];
                            threadSt = node.InnerText;
                            p[i].heroDeath = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("assists")[i];
                            threadSt = node.InnerText;
                            p[i].heroAssist = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("gold")[i];
                            threadSt = node.InnerText;
                            p[i].heroGolds = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("last_hits")[i];
                            threadSt = node.InnerText;
                            p[i].heroLH = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("denies")[i];
                            threadSt = node.InnerText;
                            p[i].heroDN = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("xp_per_min")[i];
                            threadSt = node.InnerText;
                            p[i].heroXPM = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("gold_per_min")[i];
                            threadSt = node.InnerText;
                            p[i].heroGPM = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("gold_spent")[i];
                            threadSt = node.InnerText;
                            p[i].heroTotalGold = threadSt;


                            node = (XmlElement)xd.GetElementsByTagName("tower_damage")[i];
                            threadSt = node.InnerText;
                            p[i].heroTDMG = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("hero_damage")[i];
                            threadSt = node.InnerText;
                            p[i].heroDMG = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("leaver_status")[i];
                            threadSt = node.InnerText;
                            p[i].leaveStatus = threadSt;

                            node = (XmlElement)xd.GetElementsByTagName("player_slot")[i];
                            threadSt = node.InnerText;
                            p[i].slot = threadSt;



                        ////////items
                        node = (XmlElement)xd.SelectNodes("result/players/player/item_0")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID0 = threadSt;
                            node = (XmlElement)xd.SelectNodes("result/players/player/item_1")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID1 = threadSt;
                            node = (XmlElement)xd.SelectNodes("result/players/player/item_2")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID2 = threadSt;
                            node = (XmlElement)xd.SelectNodes("result/players/player/item_3")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID3 = threadSt;
                            node = (XmlElement)xd.SelectNodes("result/players/player/item_4")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID4 = threadSt;
                            node = (XmlElement)xd.SelectNodes("result/players/player/item_5")[i];
                            threadSt = node.InnerText;
                            p[i].itemsID5 = threadSt;


                            node = (XmlElement)xd.GetElementsByTagName("account_id")[i];
                            threadSt = node.InnerText;
                            if (threadSt != "4294967295")
                            {
                                p[i].id = threadSt;
                            }
                            else p[i].id = "Anonymous";
                        }).IsCompleted)
                        {



                            Thread th1 = new Thread(() =>
                            {//Инициализация нового потока
                            XmlElement node = (XmlElement)xd.GetElementsByTagName("radiant_win")[0];
                                st = node.InnerText;
                                heroResult = st;

                                node = (XmlElement)xd.GetElementsByTagName("duration")[0];
                                st = node.InnerText;
                                matchTime = GetTime(st);

                                node = (XmlElement)xd.GetElementsByTagName("game_mode")[0];
                                st = node.InnerText;
                                if (st == "0") gameMod = "Unknown";
                                if (st == "1") gameMod = "All Pick";
                                if (st == "2") gameMod = "Captains Mode";
                                if (st == "3") gameMod = "Random Draft";
                                if (st == "4") gameMod = "Single Draft";
                                if (st == "5") gameMod = "All Random";
                                if (st == "6") gameMod = "?? INTRO/DEATH ??";
                                if (st == "7") gameMod = "The Diretide";
                                if (st == "8") gameMod = "Reverse Captains Mode";
                                if (st == "9") gameMod = "Greeviling";
                                if (st == "10") gameMod = "Tutorial";
                                if (st == "11") gameMod = "Mid Only";
                                if (st == "12") gameMod = "Least Played";
                                if (st == "13") gameMod = "New Player Pool";
                                if (st == "14") gameMod = "Compendium Matchmaking";
                                if (st == "15") gameMod = "Custom";
                                if (st == "16") gameMod = "Captains Draft";
                                if (st == "17") gameMod = "Balanced Draft";
                                if (st == "18") gameMod = "Ability Draft";
                                if (st == "19") gameMod = "Event";
                                if (st == "20") gameMod = "Death Match";
                                if (st == "21") gameMod = "1vs1 Solo Mid";

                                node = (XmlElement)xd.GetElementsByTagName("positive_votes")[0];
                                st = node.InnerText;
                                positiveVotes = st;
                            });
                            th1.Start();
                            Thread th2 = new Thread(() =>//Инициализация нового потока
                            {
                                XmlElement node = (XmlElement)xd.GetElementsByTagName("negative_votes")[0];
                                st = node.InnerText;
                                negativeVotes = st;

                                node = (XmlElement)xd.GetElementsByTagName("lobby_type")[0];
                                st = node.InnerText;
                                if (st == "0") lobbyType = "Public matchmaking";
                                if (st == "1") lobbyType = "Practice";
                                if (st == "2") lobbyType = "Tournament";
                                if (st == "3") lobbyType = "Tutorial";
                                if (st == "4") lobbyType = "Co-op with bots";
                                if (st == "5") lobbyType = "Team match";
                                if (st == "6") lobbyType = "Solo Queue";
                                if (st == "7") lobbyType = "Ranked";
                                if (st == "8") lobbyType = "Solo Mid 1vs1";

                                node = (XmlElement)xd.GetElementsByTagName("tower_status_radiant")[0];
                                st = node.InnerText; // Считывае ID  
                            towerStatusRadiant = st; // Добавляем его в массив  

                            node = (XmlElement)xd.GetElementsByTagName("tower_status_dire")[0];
                                st = node.InnerText; // Считывае ID  
                            towerStatusDire = st; // Добавляем его в массив  

                            node = (XmlElement)xd.GetElementsByTagName("barracks_status_radiant")[0];
                                st = node.InnerText; // Считывае ID  
                            barrackStatusRadiant = st; // Добавляем его в массив  

                            node = (XmlElement)xd.GetElementsByTagName("barracks_status_dire")[0];
                                st = node.InnerText; // Считывае ID  
                            barrackStatusDire = st; // Добавляем его в массив  


                        }); th2.Start();

                            while (th1.IsAlive || th2.IsAlive) Thread.Sleep(50);
                        }

                        if (matchId == lastMatchId && !isHistory && !isSelectedPlayer)
                            SaveShortStat();
                        //return "0"; // Возвращаем результат  
                    }
                }
                catch (Exception ex) {
                GetStatistic(matchId, isHistory);
                //MessageBox.Show(ex.Message);
                }
           // if (saved_lastMatchId != matchId && !isHistory) {saved_lastMatchId = matchId;}


        }
        #region Barracks
        public enum DotATowerStatus
        {
            Tier1Top = 1,
            Tier2Top = 2,
            Tier3Top = 4,
            Tier1Middle = 8,
            Tier2Middle = 16,
            Tier3Middle = 32,
            Tier1Bottom = 64,
            Tier2Bottom = 128,
            Tier3Bottom = 256,

            AncientBottom = 512,
            AncientTop = 1024
        }
        public enum DotABarracksStatus
        {
            MeleeTop = 1,
            RangedTop = 2,
            MeleeMiddle = 4,
            RangedMiddle = 8,
            MeleeBottom = 16,
            RangedBottom = 32
        }

        public bool GetMapTowerStatus(DotATowerStatus barrack, int num)
        {
            DotATowerStatus CurrentTowerStatus = (DotATowerStatus)num;
            if ((CurrentTowerStatus & barrack) == barrack)
            {
                return true;
            }
            return false;
        }
        public bool GetMapBarrackStatus(DotABarracksStatus barrack, int num)
        {
            DotABarracksStatus CurrentTowerStatus = (DotABarracksStatus)num;
            if ((CurrentTowerStatus & barrack) == barrack)
            {
                return true;
            }
            return false;
        }
        #endregion

        public static string ItemToPic(string id)
        {
            if (id == "0") { return "Image/Items/empty_slot.png"; }
            if (File.Exists("Image/Items/" + id + ".png"))
            {
                string Item = "Image/Items/" + id + ".png";
                return Item;
            }
            else throw new Exception("Предмет ID = " + id + " не найден");

            //return "Image/Items/empty_slot.png";
        }
        public static string TranslateHeroName(string id)
        {
            if (!File.Exists("Image/Heroes/" + id + ".png"))
            {
                MessageBox.Show("Герой " + id + " не найден");
            }

            if (id == "0") { return "not picked"; }
            if (id == "1") { return "Anti-Mage"; }
            if (id == "2") { return "Axe"; }
            if (id == "3") { return "Bane"; }
            if (id == "4") { return "Bloodseeker"; }
            if (id == "5") { return "Crystal Maiden"; }
            if (id == "6") { return "Drow Ranger"; }
            if (id == "7") { return "Earthshaker"; }
            if (id == "8") { return "Juggernaut"; }
            if (id == "9") { return "Mirana"; }
            if (id == "10") { return "Morphling"; }
            if (id == "11") { return "Shadow Fiend"; }
            if (id == "12") { return "Phantom Lancer"; }
            if (id == "13") { return "Puck"; }
            if (id == "14") { return "Pudge"; }
            if (id == "15") { return "Razor"; }
            if (id == "16") { return "Sand King"; }
            if (id == "17") { return "Storm Spirit"; }
            if (id == "18") { return "Sven"; }
            if (id == "19") { return "Tiny"; }
            if (id == "20") { return "Vengeful Spirit"; }
            if (id == "21") { return "Windranger"; }
            if (id == "22") { return "Zeus"; }
            if (id == "23") { return "Kunkka"; }
            if (id == "24") { return ""; }
            if (id == "25") { return "Lina"; }
            if (id == "26") { return "Lion"; }
            if (id == "27") { return "Shadow Shaman"; }
            if (id == "28") { return "Slardar"; }
            if (id == "29") { return "Tidehunter"; }
            if (id == "30") { return "Witch Doctor"; }
            if (id == "31") { return "Lich"; }
            if (id == "32") { return "Riki"; }
            if (id == "33") { return "Enigma"; }
            if (id == "34") { return "Tinker"; }
            if (id == "35") { return "Sniper"; }
            if (id == "36") { return "Necrophos"; }
            if (id == "37") { return "Warlock"; }
            if (id == "38") { return "Beastmaster"; }
            if (id == "39") { return "Queen of Pain"; }
            if (id == "40") { return "Venomancer"; }
            if (id == "41") { return "Faceless Void"; }
            if (id == "42") { return "Wraith King"; }
            if (id == "43") { return "Death Prophet"; }
            if (id == "44") { return "Phantom Assassin"; }
            if (id == "45") { return "Pugna"; }
            if (id == "46") { return "Templar Assassin"; }
            if (id == "47") { return "Viper"; }
            if (id == "48") { return "Luna"; }
            if (id == "49") { return "Dragon Knight"; }
            if (id == "50") { return "Dazzle"; }
            if (id == "51") { return "Clockwerk"; }
            if (id == "52") { return "Leshrac"; }
            if (id == "53") { return "Nature's Prophet"; }
            if (id == "54") { return "Lifestealer"; }
            if (id == "55") { return "Dark Seer"; }
            if (id == "56") { return "Clinkz"; }
            if (id == "57") { return "Omniknight"; }
            if (id == "58") { return "Enchantress"; }
            if (id == "59") { return "Huskar"; }
            if (id == "60") { return "Night Stalker"; }
            if (id == "61") { return "Broodmother"; }
            if (id == "62") { return "Bounty Hunter"; }
            if (id == "63") { return "Weaver"; }
            if (id == "64") { return "Jakiro"; }
            if (id == "65") { return "Batrider"; }
            if (id == "66") { return "Chen"; }
            if (id == "67") { return "Spectre"; }
            if (id == "68") { return "Ancient Apparition"; }
            if (id == "69") { return "Doom"; }
            if (id == "70") { return "Ursa"; }
            if (id == "71") { return "Spirit Breaker"; }
            if (id == "72") { return "Gyrocopter"; }
            if (id == "73") { return "Alchemist"; }
            if (id == "74") { return "Invoker"; }
            if (id == "75") { return "Silencer"; }
            if (id == "76") { return "Outworld Devourer"; }
            if (id == "77") { return "Lycan"; }
            if (id == "78") { return "Brewmaster"; }
            if (id == "79") { return "Shadow Demon"; }
            if (id == "80") { return "Lone Druid"; }
            if (id == "81") { return "Chaos Knight"; }
            if (id == "82") { return "Meepo"; }
            if (id == "83") { return "Treant Protector"; }
            if (id == "84") { return "Ogre Magi"; }
            if (id == "85") { return "Undying"; }
            if (id == "86") { return "Rubick"; }
            if (id == "87") { return "Disruptor"; }
            if (id == "88") { return "Nyx Assassin"; }
            if (id == "89") { return "Naga Siren"; }
            if (id == "90") { return "Keeper of the Light"; }
            if (id == "91") { return "IO"; }
            if (id == "92") { return "Visage"; }
            if (id == "93") { return "Slark"; }
            if (id == "94") { return "Medusa"; }
            if (id == "95") { return "Troll Warlord"; }
            if (id == "96") { return "Centaur Warrunner"; }
            if (id == "97") { return "Magnus"; }
            if (id == "98") { return "Timbersaw"; }
            if (id == "99") { return "Bristleback"; }
            if (id == "100") { return "Tusk"; }
            if (id == "101") { return "Skywrath Mage"; }
            if (id == "102") { return "Abaddon"; }
            if (id == "103") { return "Elder Titan"; }
            if (id == "104") { return "Legion Commander"; }
            if (id == "105") { return "Techies"; }
            if (id == "106") { return "Ember Spirit"; }
            if (id == "107") { return "Earth Spirit"; }
            if (id == "108") { return ""; }
            if (id == "109") { return "Terrorblade"; }
            if (id == "110") { return "Phoenix"; }
            if (id == "111") { return "Oracle"; }
            if (id == "112") { return "Winter Wyvern"; }

            return id;
        }
      

        #region Save/Read Matches
        void SaveShortStat()
        {
            
            Player player = GetMe();
            if (!File.Exists("matches/matches_" + AccountId + ".stat"))
            {
                File.Create("matches/matches_" + AccountId + ".stat").Close();
            }
                StreamWriter sw = File.AppendText("matches/matches_" + AccountId + ".stat");
                if (Convert.ToInt32(player.slot) <= 5 && heroResult == "true" || Convert.ToInt32(player.slot) > 5 && heroResult == "false")
                {
                    sw.WriteLine("Win," + Helper.getHeroName(player.heroId) + "," + lastMatchId);
                }
                else sw.WriteLine("Loose," + Helper.getHeroName(player.heroId) + "," + lastMatchId);

                sw.Close();
            
        }
        public static void LastMatchesFromFile()
        {
            if (!File.Exists("matches/matches_" + AccountId + ".stat"))
            {
                File.Create("matches/matches_" + AccountId + ".stat").Close();
            }
            lastMatchStatistics.Clear();
            string[] s = File.ReadAllLines("matches/matches_" + AccountId + ".stat");
            foreach (string str in s) 
            {
               lastMatchStatistics.Push(str);
            }
            
        }
        #endregion

        #region Addition information
        public string GetTop()
        {
            int maxKills =       0,
                maxDeaths =      0, 
                maxAssists =     0, 
                maxDN =          0, 
                maxLH =          0, 
                maxDamage =      0, 
                maxTowerDamage = 0, 
                maxXpm =         0, 
                maxGpm =         0, 
                maxGold =        0,
                maxLowDeath =    100;

            double maxKDA = 0;

            string Kills       = null, 
                   Deaths      = null, 
                   Assists     = null, 
                   DN          = null, 
                   LH          = null, 
                   Damage      = null, 
                   TowerDamage = null, 
                   XPM         = null, 
                   GPM         = null, 
                   Gold        = null,
                   KDA         = null,
                   LowDeath    = null,
                   averageKDA = null;

            int averageK = 0,
                averageD = 0,
                averageA = 0;
                  
            for(int i=0;i<Players.Length;i++)
            //Parallel.For(0, 10, i =>
                {
                    averageK += Convert.ToInt32(Players[i].heroKills);
                    averageD += Convert.ToInt32(Players[i].heroDeath);
                    averageA += Convert.ToInt32(Players[i].heroAssist);
                    averageKDA = averageK / 10 + " | " + averageD / 10 + " | " + averageA / 10;

                    if (Convert.ToInt32(Players[i].heroKills) > maxKills)
                    {
                        maxKills = Convert.ToInt32(Players[i].heroKills);
                        Kills = "Kills : " + maxKills + " by " + TranslateHeroName(Players[i].heroId);
                        
                    }
                    if (Convert.ToInt32(Players[i].heroDeath) > maxDeaths)
                    {
                        maxDeaths = Convert.ToInt32(Players[i].heroDeath);
                        Deaths = "Deaths : " + maxDeaths + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroAssist) > maxAssists)
                    {
                        maxAssists = Convert.ToInt32(Players[i].heroAssist);
                        Assists = "Assists : " + maxAssists + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroDN) > maxDN)
                    {
                        maxDN = Convert.ToInt32(Players[i].heroDN);
                        DN = "Denied : " + maxDN + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroLH) > maxLH)
                    {
                        maxLH = Convert.ToInt32(Players[i].heroLH);
                        LH = "Last hists : " + maxLH + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroDMG) > maxDamage)
                    {
                        maxDamage = Convert.ToInt32(Players[i].heroDMG);
                        Damage = "Damage : " + maxDamage + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroTDMG) > maxTowerDamage)
                    {
                        maxTowerDamage = Convert.ToInt32(Players[i].heroTDMG);
                        TowerDamage = "Tower damage : " + maxTowerDamage + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroXPM) > maxXpm)
                    {
                        maxXpm = Convert.ToInt32(Players[i].heroXPM);
                        XPM = "XPM : " + maxXpm + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroGPM) > maxGpm)
                    {
                        maxGpm = Convert.ToInt32(Players[i].heroGPM);
                        GPM = "GPM : " + maxGpm + " by " + TranslateHeroName(Players[i].heroId);
                    }
                    if (Convert.ToInt32(Players[i].heroTotalGold) > maxGold)
                    {
                        maxGold = Convert.ToInt32(Players[i].heroTotalGold);
                        Gold = "Gold : " + maxGold + " by " + TranslateHeroName(Players[i].heroId);
                    }

                    if (Players[i].leaveStatus != "1")
                    if (Convert.ToInt32(Players[i].heroDeath) < maxLowDeath)
                    {
                        maxLowDeath = Convert.ToInt32(Players[i].heroDeath);
                        LowDeath = "Low deaths : " + maxLowDeath + " by " + TranslateHeroName(Players[i].heroId);
                    }

                    if (Players[i].leaveStatus != "1")
                    if (Math.Round(
                        (
                        (((Convert.ToDouble(Players[i].heroKills) +  Convert.ToDouble(Players[i].heroAssist)
                        )!=0) ? (Convert.ToDouble(Players[i].heroKills) + Convert.ToDouble(Players[i].heroAssist)):1) / 
                    (
                      (Convert.ToDouble(Players[i].heroDeath)!=0) ? Convert.ToDouble(Players[i].heroDeath) : 1)
                    )
                    ,2) > maxKDA)
                    {
                         maxKDA = Math.Round(((Convert.ToDouble(Players[i].heroKills) + Convert.ToDouble(Players[i].heroAssist)) /
                          (
                            (Convert.ToDouble(Players[i].heroDeath) != 0) ? Convert.ToDouble(Players[i].heroDeath):1)
                          )
                          ,2);

                        KDA = "KDA ratio : " + maxKDA + " by " + TranslateHeroName(Players[i].heroId);
                    }

                }//);

            return Kills + "\n" +
                Deaths + "\n" +
                LowDeath + "\n" +
                Assists + "\n" +
                KDA + "\n" +
                Damage + "\n" +
                XPM + "\n" +
                GPM + "\n" +
                Gold + "\n" +
                DN + "\n" +
                LH + "\n" +
                TowerDamage;
        }


        public string GetLeavers()
        {
            string leavers = null;
            foreach (var p in Players)
            {
                if (p.leaveStatus == "1") leavers += TranslateHeroName(p.heroId) + " | " ;
            }
            return leavers;
        }
        public string GetToolTip(string Num)
        {
            if (!File.Exists("ToolTips/ToolTip" + Num + ".tdb")) { return "Empty"; }
            else
            {
                StreamReader sr = new StreamReader("ToolTips/ToolTip" + Num + ".tdb", Encoding.GetEncoding(1251));
                string str1;
                str1 = sr.ReadToEnd();
                sr.Close();

                return str1;
            }
        }
        string GetTime(string time)
        {
            time = Convert.ToString(Convert.ToInt32(time) / 60) + " min";
            return time;
        }
        #endregion
    }
}
