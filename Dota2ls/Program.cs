using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Dota2ls
{
    public class Program : System.Windows.Application
    {
        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;

#line 5 "..\..\App.xaml"


#line default
#line hidden
            System.Uri resourceLocater = new System.Uri("/Dota2ls;component/app.xaml", System.UriKind.Relative);

#line 1 "..\..\App.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
            //#line hidden
        }

        public static System.Diagnostics.Process asp_Proc = new System.Diagnostics.Process();
        public static MainPresenter mp;

        [System.STAThreadAttribute()]
        public static void Main()
        {
            //System.Threading.Thread t1 = new System.Threading.Thread(() =>
            // {
            // Program app1 = new Program();

            // app1.InitializeComponent();
            //  app1.Run(su);
            //});
            //t1.Start();
            Startup su = new Startup();
            su.Show();

            Program app = new Program();
            app.InitializeComponent();
           
            System.Threading.Thread th = new System.Threading.Thread(StatisticLib.Helper.LoadItems);
            th.Start();
            MainWindow mw = new MainWindow();
            mp = new MainPresenter(mw);
            app.Run(mw);
            
        }

    }
}