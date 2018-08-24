using System;
using System.Linq;
using StatisticLib;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Threading;

namespace Dota2ls
{

    public class MainPresenter
    {
        IMainWindow mw;
        public static Statistic st;
        IMessageService ms;

        public string saved_lastMatchId = null;
        public static bool isProcessing = false;
        

        public MainPresenter(IMainWindow _mainWindow)
        {
            Settings.debugSavedGames();
            ms = new MessageService();
            mw = _mainWindow;
            mw.onWindowLoad += MainWindow_onWindowLoad;
            mw.ListViewSelectionChanged += Mw_ListViewSelectionChanged;
            mw.saveAccountIdButton += Mw_saveAccountIdButton;
            mw.refreshButtonClick += Mw_refreshButtonClick;
            Statistic.AccountId = Settings.GetAccountId();
            st = new Statistic(Settings.GetAccountId());

            

            DispatcherTimer reloadTimer = new DispatcherTimer();
            reloadTimer.Tick += new EventHandler(Mw_refreshButtonClick);
            reloadTimer.Interval = new TimeSpan(0, 0, 0, 30);
            reloadTimer.Start();
        }

        private void Mw_refreshButtonClick(object sender, EventArgs e)//обработчик клика RefreshButton
        {
            st.isSelectedPlayer = false;
            st = new Statistic(Settings.GetAccountId());
            LoadStatistic();
        }

        private void Mw_saveAccountIdButton(object sender, EventArgs e)//Обработчик нажатия на кнопку сохранения ID игрока
        {
            Settings.SaveAccount(mw.accountTextBox);
            Statistic.AccountId = mw.accountTextBox;
            ms.ShowExclamation("Application will be sleep a little bit time");
            st = new Statistic(mw.accountTextBox);
            st.save_last_25_matches();
            isProcessing = false;
            LoadStatistic();
            mw.saveButtonVisible = System.Windows.Visibility.Hidden;
            //ms.ShowExclamation("Please , restart application");
            
            //Application.Restart();
            //Program.asp_Proc.Kill();
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void Mw_ListViewSelectionChanged(object sender, EventArgs e)//обработчик нажатие на элемент ListBox'a
        {
            if (!isProcessing)
            {
                int index;
                string selectedMatchId;
                isProcessing = true;
                if (!st.isSelectedPlayer)
                {
                    index = mw.StatListView.SelectedIndex;
                    selectedMatchId = mw.matchIdLabel = Statistic.lastMatchStatistics.ToList()[index].Split(',').Last();
                    st.GetStatistic(selectedMatchId, true);
                }
                else
                {
                    index = mw.StatListView.SelectedIndex;
                    selectedMatchId = mw.matchIdLabel = Statistic.last10MatchId[index];
                    st.GetStatistic(selectedMatchId,false);
                }
               
                mw.SetPlayers(st.p, st.GetMeIndex());
                ReloadAdditionInformation();
                isProcessing = false;
            }

        }

        private void MainWindow_onWindowLoad(object sender, EventArgs e)
        {

            mw.accountTextBox = Settings.GetAccountId();
            LoadStatistic();
            mw.saveButtonVisible = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// Загрузка статистики и ее визуализация
        /// </summary>
        void LoadStatistic()
        {

            if (!isProcessing)
            {
                isProcessing = true;
                Statistic.LastMatchesFromFile();//загрузка статистики сохраненных матчей
                RefreshListStats();

                if (st.Connection())
                {
                    string id = st.GetLastMatchId();
                    if (saved_lastMatchId != id)
                    {
                        st.GetStatistic(id, false);
                        mw.SetPlayers(st.p, st.GetMeIndex());
                        ReloadAdditionInformation();
                        ReloadAdditionInformation();
                        saved_lastMatchId = id;
                    }
                    isProcessing = false;
                }
                else
                {
                    st.GetStatistic(Statistic.lastMatchStatistics.ToList()[0].Split(',').Last(), true);
                    mw.SetPlayers(st.p, st.GetMeIndex());
                    ReloadAdditionInformation();
                    isProcessing = false;
                }
            }
        }
        void ReloadAdditionInformation()
        {
            
            mw.Barracks(st);
            mw.matchTimeLabel = st.matchTime;
            if (st.heroResult == "true")
            {
                mw.radiantWinLabel = "Radiant Win";
                mw.SetRadiantWinLabelForeColor(0, 255, 0);
            }
            else
            {
                mw.radiantWinLabel = "Dire Win";
                mw.SetRadiantWinLabelForeColor(255, 0, 0);
            }
            mw.topPlayersLabel = st.GetTop();
            mw.lobbyLabel = st.gameMod +"   |   "+ st.lobbyType;
            mw.positiveVotes = "Positive votes: " + st.positiveVotes;
            mw.negativeVotes = "Negative votes: " + st.negativeVotes;
            mw.leavers = st.GetLeavers();
            mw.mainLobbyLabel = st.gameMod;
        }

        void RefreshListStats()//обновляет ListView
        {
                mw.StatListView.Items.Clear();
                foreach (string s in Statistic.lastMatchStatistics)
                {
                    mw.StatListView.Items.Add(new StatList() { Result = s.Split(',').First(),Hero=s.Split(',').ToList()[1], ID = s.Split(',').Last() });
                }             
        }
        /// <summary>
        /// Загрузка статистики для выбранного игрока
        /// </summary>
        public void LoadStatistic_SelectedPlayer()
        {
                if (!isProcessing)
                {

                    isProcessing = true;
                    st.Get10StatisticId();
                    RefreshListStat_SelectedPlayer();

                    if (st.Connection())
                    {
                        st.GetStatistic(st.GetLastMatchId(), false);
                    }
                    else ms.ShowError("internet connection lost");
                    mw.SetPlayers(st.p, st.GetMeIndex());
                    ReloadAdditionInformation();
                    isProcessing = false;
                }

        }
        private void setProgress(int val)
        {
            if (val<100)
            {
                mw.progressVisible = true;
                mw.progress = val;
            }
            else
            {
                mw.progressVisible = false;
            }
        }

        /// <summary>
        /// Обновление ListView для выбранного игрока
        /// </summary>
        void RefreshListStat_SelectedPlayer()
        {
            mw.StatListView.Items.Clear();
            foreach (string s in Statistic.last10MatchId)
            {
                mw.StatListView.Items.Add(new StatList() {Result = "" , Hero = s, ID = "" });
            }
        }

    }
    /// <summary>
    /// Структкура для данных ListView
    /// </summary>
    class StatList
    {
        public string Result { get; set; }
        public string Hero { get; set; }
        public string ID { get; set; }
    }
}
