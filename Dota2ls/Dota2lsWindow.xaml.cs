using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StatisticLib;
using System.Diagnostics;

namespace Dota2ls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //
    public interface IMainWindow
    {
        void SetPlayers(Player[] p, int indexOfMe);
        event EventHandler onWindowLoad;
        ListView StatListView { get; set; }
        event EventHandler ListViewSelectionChanged;
        string matchIdLabel { get; set; }
        string matchTimeLabel { get; set; }
        string radiantWinLabel { get; set; }
        string lobbyLabel { set; }
        string topPlayersLabel { set; }
        void Barracks(Statistic st);
        string positiveVotes { set; }
        string negativeVotes { set; }
        string leavers { set; }
        string mainLobbyLabel { set; }
        void SetRadiantWinLabelForeColor(byte R, byte G, byte B);
        string accountTextBox { get; set; }
        event EventHandler saveAccountIdButton;
        Visibility saveButtonVisible { set; }
        void SetPentagoneBounds(Player p);
        event EventHandler refreshButtonClick;
        int progress {set; }
        bool progressVisible { set; }
    }
    public partial class MainWindow : Window, IMainWindow
    {

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();//экземпляр класса для трея

        public MainWindow()
        {
            this.InitializeComponent();

            StatList.SelectedIndex = 0;
            this.Window.Loaded += MainWindow_onWindowLoad;
            this.StatList.SelectionChanged += StatList_SelectionChanged;
            this.SaveAccountIdButton.Click += SaveAccountIdButton_Click;
            this.AccountTextBox.TextChanged += AccountTextBox_TextChanged;
            this.RefreshButton.Click += RefreshButton_Click;
            this.ExitButton.Click += ExitButton_Click;
            this.HideButton.Click += HideButton_Click;
            this.Closing += MainWindow_Closing;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
          
            //трей
            ni.Icon = new System.Drawing.Icon("Image/+.ico");
            ni.Text = "Dota2ls";

            AccountTextBox.ToolTip = "Please enter your Dota2 identification";






        }
        /// <summary>
        /// перетаскивание формы за любой участок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!MainPresenter.isProcessing)
                    this.DragMove();
            }
            catch (Exception) { }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try {
               // Program.asp_Proc.Kill();

                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (process.ProcessName == "Dota2ls_asp")
                    {
                        process.Kill();
                    }
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            //if (HideButtonClick != null) HideButtonClick(this, EventArgs.Empty);
            ni.Visible = true;
            ni.Click += (sndr, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
            this.Hide();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                // Program.asp_Proc.Kill();
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
                this.Close();
            } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка закрытия приложения",MessageBoxButton.OK); }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (refreshButtonClick != null) refreshButtonClick(this, EventArgs.Empty);
        }

        private void AccountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveAccountIdButton.Visibility = Visibility.Visible;
        }

        private void SaveAccountIdButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveAccountIdButton != null) saveAccountIdButton(this, EventArgs.Empty);
        }

        private void StatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewSelectionChanged != null) ListViewSelectionChanged(this, EventArgs.Empty);
        }

        private void MainWindow_onWindowLoad(object sender, EventArgs e)
        {
            if (onWindowLoad != null) onWindowLoad(this, EventArgs.Empty);
        }

        public event EventHandler onWindowLoad;
        public event EventHandler ListViewSelectionChanged;
        public event EventHandler saveAccountIdButton;
        public event EventHandler refreshButtonClick;

        private void RefreshColorPlayerBar()
        {
            p1Rect.Opacity = 0.26; SetForeColor(Player1, Color.FromArgb(255,50, 235, 251)); 
            p2Rect.Opacity = 0.26; SetForeColor(Player2, Color.FromArgb(255, 50, 235, 251)); 
            p3Rect.Opacity = 0.26; SetForeColor(Player3, Color.FromArgb(255, 50, 235, 251)); 
            p4Rect.Opacity = 0.26; SetForeColor(Player4, Color.FromArgb(255, 50, 235, 251)); 
            p5Rect.Opacity = 0.26; SetForeColor(Player5, Color.FromArgb(255, 50, 235, 251)); 
            p6Rect.Opacity = 0.26; SetForeColor(Player6, Color.FromArgb(255, 50, 235, 251)); 
            p7Rect.Opacity = 0.26; SetForeColor(Player7, Color.FromArgb(255, 50, 235, 251));
            p8Rect.Opacity = 0.26; SetForeColor(Player8, Color.FromArgb(255, 50, 235, 251));
            p9Rect.Opacity = 0.26; SetForeColor(Player9, Color.FromArgb(255, 50, 235, 251)); 
            p10Rect.Opacity = 0.26; SetForeColor(Player10, Color.FromArgb(255, 50, 235, 251)); 
        }//устанавливает цвет задней панели и цвет букв на PlayerBar'е в положение default
        private void SetColorPlayerBar(int index)
        {
            RefreshColorPlayerBar();
            if (index == 0) { p1Rect.Opacity = 0.7; Player1.IDLabel.Content = "You"; SetForeColor(Player1, Colors.Red); }
            if (index == 1) { p2Rect.Opacity = 0.7; Player2.IDLabel.Content = "You"; SetForeColor(Player2, Colors.Red); }
            if (index == 2) { p3Rect.Opacity = 0.7; Player3.IDLabel.Content = "You"; SetForeColor(Player3, Colors.Red); }
            if (index == 3) { p4Rect.Opacity = 0.7; Player4.IDLabel.Content = "You"; SetForeColor(Player4, Colors.Red); }
            if (index == 4) { p5Rect.Opacity = 0.7; Player5.IDLabel.Content = "You"; SetForeColor(Player5, Colors.Red); }
            if (index == 5) { p6Rect.Opacity = 0.7; Player6.IDLabel.Content = "You"; SetForeColor(Player6, Colors.Red); }
            if (index == 6) { p7Rect.Opacity = 0.7; Player7.IDLabel.Content = "You"; SetForeColor(Player7, Colors.Red); }
            if (index == 7) { p8Rect.Opacity = 0.7; Player8.IDLabel.Content = "You"; SetForeColor(Player8, Colors.Red); }
            if (index == 8) { p9Rect.Opacity = 0.7; Player9.IDLabel.Content = "You"; SetForeColor(Player9, Colors.Red); }
            if (index == 9) { p10Rect.Opacity = 0.7; Player10.IDLabel.Content = "You"; SetForeColor(Player10, Colors.Red); }

        }//устанавливает цвет задней панели и цвет букв на PlayerBar'е в положение select
        public void SetPlayers(Player[] p , int indexOfMe) 
        {
            SetValue(Player1, p[0]);
            SetValue(Player2, p[1]);
            SetValue(Player3, p[2]);
            SetValue(Player4, p[3]);
            SetValue(Player5, p[4]);

            SetValue(Player6, p[5]);
            SetValue(Player7, p[6]);
            SetValue(Player8, p[7]);
            SetValue(Player9, p[8]);
            SetValue(Player10, p[9]);

            SetColorPlayerBar(indexOfMe);
        }
        /// <summary>
        /// Заполнения данными таблицы для плеера
        /// </summary>
        /// <param name="c">Ссылка на контрол плеера</param>
        /// <param name="p">Ссылка на статистику плеера</param>
        void SetValue(UserControl1 c, Player p)
        {

            try {
                
                c.IDLabel.Content = p.id;
                c.PicHero.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../Image/Heroes/" + p.heroId + ".png");
                //c.PicHero.Source = new Uri("pack://siteoforigin:,,,/../Image/Heroes/" + p.heroId + ".png");
                //c.NameHeroLabel.Content = Statistic.TranslateHeroName(p.heroId);
                c.NameHeroLabel.Content = Helper.getHeroName(p.heroId);
                c.PicItem1.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID0));
                c.PicItem2.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID1));
                c.PicItem3.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID2));
                c.PicItem4.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID3));
                c.PicItem5.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID4));
                c.PicItem6.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(p.itemsID5));
                c.KLabel.Content = p.heroKills;
                c.DLabel.Content = p.heroDeath;
                c.ALebel.Content = p.heroAssist;
                c.GoldLabel.Content = p.heroGolds;
                c.LHLabel.Content = p.heroLH;
                c.DNLabel.Content = p.heroDN;
                c.XPMLabel.Content = p.heroXPM;
                c.GPMLabel.Content = p.heroGPM;
                c.TGLabel.Content = p.heroTotalGold;
                c.DMGLabel.Content = p.heroDMG;
                c.TDLabel.Content = p.heroTDMG;
                c.hero_id = p.heroId;
                c.Hero_Pic.Source = new Uri("pack://siteoforigin:,,,/../Videos/Heroes/"+ p.heroId + "/" + p.heroId + ".mp4");
                c.Hero_features.Source = new Uri("pack://siteoforigin:,,,/../Videos/Heroes/" + p.heroId + "/" + p.heroId + "_features.mp4");
                c.Hero_Id.Content = p.heroId;

                //c.PicItem1.ToolTip = MainPresenter.st.GetToolTip(p.itemsID0);
                //c.PicItem2.ToolTip = MainPresenter.st.GetToolTip(p.itemsID1);
                //c.PicItem3.ToolTip = MainPresenter.st.GetToolTip(p.itemsID2);
                //c.PicItem4.ToolTip = MainPresenter.st.GetToolTip(p.itemsID3);
                //c.PicItem5.ToolTip = MainPresenter.st.GetToolTip(p.itemsID4);
                //c.PicItem6.ToolTip = MainPresenter.st.GetToolTip(p.itemsID5);
                c.setToolTipId(p.itemsID0, p.itemsID1, p.itemsID2, p.itemsID3, p.itemsID4, p.itemsID5);
              



            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        void SetForeColor(UserControl1 c,Color col)//Выделение текущего игрока
        {
            SolidColorBrush b = new SolidColorBrush(col);
            c.IDLabel.Foreground = b;
            c.NameHeroLabel.Foreground = b;
            c.KLabel.Foreground = b;
            c.DLabel.Foreground = b;
            c.ALebel.Foreground = b;
            c.GoldLabel.Foreground = b;
            c.LHLabel.Foreground = b;
            c.DNLabel.Foreground = b;
            c.XPMLabel.Foreground = b;
            c.GPMLabel.Foreground = b;
            c.TGLabel.Foreground = b;
            c.DMGLabel.Foreground = b;
            c.TDLabel.Foreground = b;
        }
        public void SetRadiantWinLabelForeColor(byte R,byte G, byte B)
        {
            RadiantWinLabel.Foreground = new SolidColorBrush(Color.FromRgb(R,G,B));
        }
        public void SetPentagoneBounds(Player p)
        {
            int kills = Convert.ToInt32(p.heroKills);
            int death = Convert.ToInt32(p.heroDeath);
            int assists = Convert.ToInt32(p.heroAssist);
            int lh = Convert.ToInt32(p.heroLH);
            int dn = Convert.ToInt32(p.heroDN);
            int xpm = Convert.ToInt32(p.heroXPM);
            int gpm = Convert.ToInt32(p.heroGPM);
            int gold = Convert.ToInt32(p.heroTotalGold);
            int damage = Convert.ToInt32(p.heroDMG);
            int towerDamage = Convert.ToInt32(p.heroTDMG);

            var path = new System.Windows.Shapes.Path();
            
        }
        BitmapSource ConvertBitmapTo96DPI(string path)//конвертер для изображений
        {
            var uri = new Uri(path);
            var bitmapImage = new BitmapImage(uri);

            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            int stride = width * 4; // 4 байта на пиксель
            var pixelData = new byte[stride * height];
            bitmapImage.CopyPixels(pixelData, stride, 0);

            return BitmapSource.Create(width, height, 96, 96,
                                        PixelFormats.Bgra32,
                                        bitmapImage.Palette,
                                        pixelData, stride);
        }

        public ListView StatListView
        {
        get { return StatList; }
        set { StatList = value; }
        }
        public string matchIdLabel
        {
            get { return MatchIdLabel.Content.ToString() ; }
            set { MatchIdLabel.Content = value; }
        }
        public string matchTimeLabel
        {
            get { return TimeMatchLabel.Content.ToString(); }
            set { TimeMatchLabel.Content = value; }
        }
        public string radiantWinLabel
        {
            get { return RadiantWinLabel.Content.ToString(); }
            set { RadiantWinLabel.Content = value; }
        }
        public string lobbyLabel
        {
            set { LobbyLabel.Content = value; }
        }
        public string topPlayersLabel
        {
            set { TopPlayerLabel.Content = value; }
        }
        public int progress
        {
            set { LoadProgressBar.Value = value; }
        }
        public string positiveVotes
        { set { { PositiveVotes.Content = value; } } }
        public string negativeVotes
        { set { { NegativeVotes.Content = value; } } }
        public string leavers
        { set { { LeaversLabel.Content = value; } } }
        public string mainLobbyLabel
        { set { MainLobbyLabel.Content = value; } }
        public string accountTextBox
        {
            get { return AccountTextBox.Text; }
            set { AccountTextBox.Text = value; }
        }
        public Visibility saveButtonVisible
        {
            set { SaveAccountIdButton.Visibility = value; }
        }
        public bool progressVisible
        {
            set {
                if (value)
                {
                    LoadProgressBar.Visibility = Visibility.Visible;
                }
                else LoadProgressBar.Visibility = Visibility.Hidden;
            }
        }

        #region Baracks
        /// <summary>
        /// Визуализация состояния бараков
        /// </summary>
        /// <param name="st">статистика матча</param>
        public void Barracks(Statistic st)
        {
            ClearMap();
            Visibility visible = Visibility.Visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Top, Convert.ToInt32(st.towerStatusRadiant))) T1RT.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Top, Convert.ToInt32(st.towerStatusRadiant))) T2RT.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Top, Convert.ToInt32(st.towerStatusRadiant))) T3RT.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedTop, Convert.ToInt32(st.barrackStatusRadiant))) RBTR.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeTop, Convert.ToInt32(st.barrackStatusRadiant))) MBTR.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Middle, Convert.ToInt32(st.towerStatusRadiant))) T1RM.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Middle, Convert.ToInt32(st.towerStatusRadiant))) T2RM.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Middle, Convert.ToInt32(st.towerStatusRadiant))) T3RM.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedMiddle, Convert.ToInt32(st.barrackStatusRadiant))) RBMR.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeMiddle, Convert.ToInt32(st.barrackStatusRadiant))) MBMR.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Bottom, Convert.ToInt32(st.towerStatusRadiant))) T1RB.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Bottom, Convert.ToInt32(st.towerStatusRadiant))) T2RB.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Bottom, Convert.ToInt32(st.towerStatusRadiant))) T3RB.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedBottom, Convert.ToInt32(st.barrackStatusRadiant))) RBBR.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeBottom, Convert.ToInt32(st.barrackStatusRadiant))) MBBR.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.AncientTop, Convert.ToInt32(st.towerStatusRadiant))) T4R1.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.AncientBottom, Convert.ToInt32(st.towerStatusRadiant))) T4R2.Visibility = visible;
            if (st.heroResult == "true") { AncientRadiant.Visibility = visible; } else AncientDire.Visibility = visible;


            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Top, Convert.ToInt32(st.towerStatusDire))) T1DT.Visibility = visible; 
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Top, Convert.ToInt32(st.towerStatusDire))) T2DT.Visibility = visible; 
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Top, Convert.ToInt32(st.towerStatusDire))) T3DT.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedTop, Convert.ToInt32(st.barrackStatusDire))) RTBD.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeTop, Convert.ToInt32(st.barrackStatusDire))) MTBD.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Middle, Convert.ToInt32(st.towerStatusDire))) T1DM.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Middle, Convert.ToInt32(st.towerStatusDire))) T2DM.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Middle, Convert.ToInt32(st.towerStatusDire))) T3DM.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedMiddle, Convert.ToInt32(st.barrackStatusDire))) RMBD.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeMiddle, Convert.ToInt32(st.barrackStatusDire))) MBMD.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier1Bottom, Convert.ToInt32(st.towerStatusDire))) T1DB.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier2Bottom, Convert.ToInt32(st.towerStatusDire))) T2DB.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.Tier3Bottom, Convert.ToInt32(st.towerStatusDire))) T3DB.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.RangedBottom, Convert.ToInt32(st.barrackStatusDire))) RBBD.Visibility = visible;
            if (st.GetMapBarrackStatus(Statistic.DotABarracksStatus.MeleeBottom, Convert.ToInt32(st.barrackStatusDire))) MBBD.Visibility = visible;

            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.AncientTop, Convert.ToInt32(st.towerStatusDire))) T4D1.Visibility = visible;
            if (st.GetMapTowerStatus(Statistic.DotATowerStatus.AncientBottom, Convert.ToInt32(st.towerStatusDire))) T4D2.Visibility = visible;

        }
        public void ClearMap()
        {
            Visibility visible = Visibility.Hidden;
            T1RT.Visibility = visible;
            T2RT.Visibility = visible;
            T3RT.Visibility = visible;
            RBTR.Visibility = visible;
            MBTR.Visibility = visible;

            T1RM.Visibility = visible;
            T2RM.Visibility = visible;
            T3RM.Visibility = visible;
            RBMR.Visibility = visible;
            MBMR.Visibility = visible;

            T1RB.Visibility = visible;
            T2RB.Visibility = visible;
            T3RB.Visibility = visible;
            RBBR.Visibility = visible;
            MBBR.Visibility = visible;
            //////////////////////////////////
            T1DT.Visibility = visible;
            T2DT.Visibility = visible;
            T3DT.Visibility = visible;
            RTBD.Visibility = visible;
            MTBD.Visibility = visible;

            T1DM.Visibility = visible;
            T2DM.Visibility = visible;
            T3DM.Visibility = visible;
            RMBD.Visibility = visible;
            MBMD.Visibility = visible;

            T1DB.Visibility = visible;
            T2DB.Visibility = visible;
            T3DB.Visibility = visible;
            RBBD.Visibility = visible;
            MBBD.Visibility = visible;

            T4R1.Visibility = visible;
            T4R2.Visibility = visible;
            T4D1.Visibility = visible;
            T4D2.Visibility = visible;

            AncientDire.Visibility = visible;
            AncientRadiant.Visibility = visible;
        }
        #endregion

        private void Player2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ASP_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ASP_CheckBox.IsChecked == true)
            {
                Program.asp_Proc.StartInfo.FileName = "Dota2ls_asp.exe";
                Program.asp_Proc.Start();
            }
            else
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (process.ProcessName == "Dota2ls_asp")
                    {
                        process.Kill();
                    }
                }
            }
        }
    }
}