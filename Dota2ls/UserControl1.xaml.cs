using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StatisticLib;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Dota2ls
{
    public interface IPlayersControl
    {
        string GetPlayerId();
    }

	public partial class UserControl1 : UserControl,IPlayersControl
	{
        ToolTip tip;
        JToken t_data_1, t_data_2, t_data_3, t_data_4, t_data_5, t_data_6;//шмотки
        public string hero_id = "undefined id";
        public UserControl1()
		{
            tip = new ToolTip();
			this.InitializeComponent();
            this.MouseLeftButtonDown += UserControl1_MouseLeftButtonDown;
        }

        private void UserControl1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadSelectedPlayerStatistic();
        }

        public string GetPlayerId()
        {
            return IDLabel.Content.ToString();
        }

        void LoadSelectedPlayerStatistic()
        {
            string id = GetPlayerId();
            if (id != "Anonymous")
            {
                
                Statistic.last10MatchId.Clear();
                MainPresenter.st = new Statistic(id);
                MainPresenter.st.isSelectedPlayer = true;
                Program.mp.LoadStatistic_SelectedPlayer();
            }
            else MessageBox.Show("Пользователь запретил просматривать статистику игр");
            
            
        }

        public void setToolTipId(string id_1, string id_2, string id_3, string id_4, string id_5, string id_6)
        {
            if (id_1 != "0") t_data_1 = Helper.getItemTooltip(id_1);
            if (id_2 != "0") t_data_2 = Helper.getItemTooltip(id_2);
            if (id_3 != "0") t_data_3 = Helper.getItemTooltip(id_3);
            if (id_4 != "0") t_data_4 = Helper.getItemTooltip(id_4);
            if (id_5 != "0") t_data_5 = Helper.getItemTooltip(id_5);
            if (id_6 != "0") t_data_6 = Helper.getItemTooltip(id_6);
        }

        void setTooltip(JToken jObj)
        {
            clearTooltip();
            //Tip_1_Pic
            if (jObj != null)
            {
                setToolTipElementVisible();
                
                Tip_1_Pic.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic(jObj["id"].ToString()));
                Tip_1_Name.Content = jObj["dname"].ToString();
                Tip_1_Name.Foreground = new SolidColorBrush(itemColorConverter(jObj["qual"].ToString()));
                Tip_1_Gold.Content = jObj["cost"].ToString();
                Tip_1_Lore.Text = jObj["lore"].ToString();
                Tip_1_Id.Content = jObj["id"].ToString(); ;
                if (jObj["cd"].ToString() == "False")
                {
                    Tip_1_Cooldown.Visibility = Visibility.Collapsed;
                    cd_ico.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Tip_1_Cooldown.Content = jObj["cd"].ToString() + " sec";
                }
              

                if (jObj["desc"].ToString() == "")
                {
                    Tip_1_Description.Visibility = Visibility.Collapsed;
                }
                else {
                    Tip_1_Description.Text = jObj["desc"].ToString();
                }
                if (jObj["attrib"].ToString() == "")
                {
                    Tip_1_Attrib.Visibility = Visibility.Collapsed;
                }
                else {
                    Tip_1_Attrib.Text = jObj["attrib"].ToString();
                }
                if (jObj["notes"].ToString() == "")
                {
                    Tip_1_Notes.Visibility = Visibility.Collapsed;
                }
                else {
                    Tip_1_Notes.Text = jObj["notes"].ToString();  }
                



            }
        }
        Color itemColorConverter(string quality)
        {
            switch (quality)
            {
                case "artifact":
                     return Color.FromRgb(255, 150, 50);
                case "epic":
                    return Color.FromRgb(144, 0, 255);
                case "rare":
                    return Color.FromRgb(0, 73, 184);
                case "component":
                    return Color.FromRgb(255, 255, 255);
                default: return Color.FromRgb(255, 255, 255);
            }
        }
        void clearTooltip()
        {
            Tip_1_Name.Content = "Empty Slot";
            Tip_1_Name.Foreground = new SolidColorBrush(itemColorConverter("default_color"));
            Tip_1_Pic.Source = ConvertBitmapTo96DPI("pack://siteoforigin:,,,/../" + Statistic.ItemToPic("0"));
            Tip_1_Gold.Content = "";
            Tip_1_Description.Text = "";
            Tip_1_Lore.Text = "";
            Tip_1_Attrib.Text = "";
            Tip_1_Notes.Text = "";
            Tip_1_Cooldown.Content = "";
            Tip_1_Id.Content = "0";
            setToolTipElementInvisible();
        }
        void setToolTipElementInvisible()
        {
            //Tip_1_Name.Visibility = Visibility.Collapsed;
            Tip_1_Pic.Visibility = Visibility.Collapsed;
            Tip_1_Gold.Visibility = Visibility.Collapsed;
            gold_ico.Visibility = Visibility.Collapsed;
            Tip_1_Description.Visibility = Visibility.Collapsed;
            Tip_1_Lore.Visibility = Visibility.Collapsed;
            Tip_1_Attrib.Visibility = Visibility.Collapsed;
            Tip_1_Notes.Visibility = Visibility.Collapsed;
            Tip_1_Cooldown.Visibility = Visibility.Collapsed;
            cd_ico.Visibility = Visibility.Collapsed;
            Tip_1_Id.Visibility = Visibility.Collapsed;
        }
        void setToolTipElementVisible()
        {
            //Tip_1_Name.Visibility = Visibility.Visible;
            Tip_1_Pic.Visibility = Visibility.Visible;
            Tip_1_Gold.Visibility = Visibility.Visible;
            gold_ico.Visibility = Visibility.Visible;
            Tip_1_Description.Visibility = Visibility.Visible;
            Tip_1_Lore.Visibility = Visibility.Visible;
            Tip_1_Attrib.Visibility = Visibility.Visible;
            Tip_1_Notes.Visibility = Visibility.Visible;
            Tip_1_Cooldown.Visibility = Visibility.Visible;
            cd_ico.Visibility = Visibility.Visible;
            Tip_1_Id.Visibility = Visibility.Visible;
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
        private void PicItem1_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_1);
                Tooltip_item_1.IsOpen = true;
        }

        private void PicItem1_MouseLeave(object sender, MouseEventArgs e)
        {
            
            Tooltip_item_1.IsOpen = false;
        }

        private void PicItem2_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_2);
                Tooltip_item_1.IsOpen = true;
        }

        private void PicItem2_MouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip_item_1.IsOpen = false;
        }

        private void PicItem3_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_3);
                Tooltip_item_1.IsOpen = true;
        }

        private void PicItem3_MouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip_item_1.IsOpen = false;
        }

        private void PicItem4_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_4);
                Tooltip_item_1.IsOpen = true;
        }

        private void PicItem4_MouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip_item_1.IsOpen = false;
        }

        private void PicItem5_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_5);
                Tooltip_item_1.IsOpen = true;
        }

        private void PicItem5_MouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip_item_1.IsOpen = false;
        }

        private void PicItem6_MouseEnter(object sender, MouseEventArgs e)
        {
                setTooltip(t_data_6);
            Tooltip_item_1.IsOpen = true;
        }

        private void PicItem6_MouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip_item_1.IsOpen = false;
        }

        private void PicHero_MouseEnter(object sender, MouseEventArgs e)
        {
            setToolTipHeroElementVisible();
            Tooltip_hero.IsOpen = true;
            Hero_Pic.MediaEnded += Hero_Pic_OnMediaEnded;
            Hero_features.MediaEnded += Hero_features_OnMediaEnded;
            Hero_Pic.Play();
            Hero_features.Play();
        }
        private void Hero_Pic_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            Hero_Pic.Position = TimeSpan.Zero;
        }
        private void Hero_features_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            Hero_features.Position = TimeSpan.Zero;
        }

        private void PicHero_MouseLeave(object sender, MouseEventArgs e)
        {
            setToolTipHeroElementInvisible();
            Tooltip_hero.IsOpen = false;
            Hero_Pic.Stop();
            Hero_features.Stop();
        }

        void setToolTipHeroElementInvisible()
        {
            //Tip_1_Name.Visibility = Visibility.Collapsed;
            Hero_Pic.Visibility = Visibility.Collapsed;
            Hero_Id.Visibility = Visibility.Collapsed;
        }
        void setToolTipHeroElementVisible()
        {
            //Tip_1_Name.Visibility = Visibility.Visible;
            Hero_Pic.Visibility = Visibility.Visible;
            Hero_Id.Visibility = Visibility.Visible;
            //Hero_Pic.Play();
        }
    }
}

