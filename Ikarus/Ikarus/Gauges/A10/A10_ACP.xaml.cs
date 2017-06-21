using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_ACP.xaml
    /// </summary>
    public partial class A10_ACP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public A10_ACP()
        {
            InitializeComponent();

            RDSREM.Text = "";

            Ready1.Visibility = System.Windows.Visibility.Hidden;
            Empty1.Visibility = System.Windows.Visibility.Hidden;
            Loaded1.Visibility = System.Windows.Visibility.Hidden;

            Ready2.Visibility = System.Windows.Visibility.Hidden;
            Empty2.Visibility = System.Windows.Visibility.Hidden;
            Loaded2.Visibility = System.Windows.Visibility.Hidden;

            Ready3.Visibility = System.Windows.Visibility.Hidden;
            Empty3.Visibility = System.Windows.Visibility.Hidden;
            Loaded3.Visibility = System.Windows.Visibility.Hidden;

            Ready4.Visibility = System.Windows.Visibility.Hidden;
            Empty4.Visibility = System.Windows.Visibility.Hidden;
            Loaded4.Visibility = System.Windows.Visibility.Hidden;

            Ready5.Visibility = System.Windows.Visibility.Hidden;
            Empty5.Visibility = System.Windows.Visibility.Hidden;
            Loaded5.Visibility = System.Windows.Visibility.Hidden;

            Ready6.Visibility = System.Windows.Visibility.Hidden;
            Empty6.Visibility = System.Windows.Visibility.Hidden;
            Loaded6.Visibility = System.Windows.Visibility.Hidden;

            Ready7.Visibility = System.Windows.Visibility.Hidden;
            Empty7.Visibility = System.Windows.Visibility.Hidden;
            Loaded7.Visibility = System.Windows.Visibility.Hidden;

            Ready8.Visibility = System.Windows.Visibility.Hidden;
            Empty8.Visibility = System.Windows.Visibility.Hidden;
            Loaded8.Visibility = System.Windows.Visibility.Hidden;

            Ready9.Visibility = System.Windows.Visibility.Hidden;
            Empty9.Visibility = System.Windows.Visibility.Hidden;
            Loaded9.Visibility = System.Windows.Visibility.Hidden;

            Ready10.Visibility = System.Windows.Visibility.Hidden;
            Empty10.Visibility = System.Windows.Visibility.Hidden;
            Loaded10.Visibility = System.Windows.Visibility.Hidden;

            Ready11.Visibility = System.Windows.Visibility.Hidden;
            Empty11.Visibility = System.Windows.Visibility.Hidden;
            Loaded11.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
        }

        public double GetSize()
        {
            return 354.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           // Armament Control Panel

                           // CannonShellsCounter,
                           // Station1, Station2, Station3, Station4, Station5, Station6, Station7, Station8, Station9, Station10, Station11

                           // Station 1 bis 11 können folgende drei Werte beinhalten.
                           // 0.0 = aus
                           // 0.1 = E (Empty, gelbes E unten rechts)
                           // 0.2 = 1 (beladen, weiße 1 unten links)
                           // 0.3 = RR (active, zwei grüne R oben, zusammen mit der weißen 1 unten links)

                           vals = strData.Split(';');

                           double cannonShellsCounter = 0.0;
                           double[] station = new double[11] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                           try
                           {
                               if (vals.Length > 0) { cannonShellsCounter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                                   
                               for (int n = 0; n < 11; n++)
                               {
                                   if (vals.Length > n) { station[n] = Convert.ToDouble(vals[n + 1], CultureInfo.InvariantCulture); }
                               }
                           }
                           catch { return; }

                           RDSREM.Text = cannonShellsCounter.ToString();

                           for (int n = 0; n < 11; n++)
                           {
                               switch (n)
                               {
                                   case 0:
                                       if (station[n] == 0.0)
                                       {
                                           Ready1.Visibility = System.Windows.Visibility.Hidden;
                                           Empty1.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded1.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready1.Visibility = System.Windows.Visibility.Hidden;
                                           Empty1.Visibility = System.Windows.Visibility.Visible;
                                           Loaded1.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready1.Visibility = System.Windows.Visibility.Hidden;
                                           Empty1.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded1.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready1.Visibility = System.Windows.Visibility.Visible;
                                           Empty1.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded1.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 1:
                                       if (station[n] == 0.0)
                                       {
                                           Ready2.Visibility = System.Windows.Visibility.Hidden;
                                           Empty2.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded2.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready2.Visibility = System.Windows.Visibility.Hidden;
                                           Empty2.Visibility = System.Windows.Visibility.Visible;
                                           Loaded2.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready2.Visibility = System.Windows.Visibility.Hidden;
                                           Empty2.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded2.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready2.Visibility = System.Windows.Visibility.Visible;
                                           Empty2.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded2.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 2:
                                       if (station[n] == 0.0)
                                       {
                                           Ready3.Visibility = System.Windows.Visibility.Hidden;
                                           Empty3.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded3.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready3.Visibility = System.Windows.Visibility.Hidden;
                                           Empty3.Visibility = System.Windows.Visibility.Visible;
                                           Loaded3.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready3.Visibility = System.Windows.Visibility.Hidden;
                                           Empty3.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded3.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready3.Visibility = System.Windows.Visibility.Visible;
                                           Empty3.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded3.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 3:
                                       if (station[n] == 0.0)
                                       {
                                           Ready4.Visibility = System.Windows.Visibility.Hidden;
                                           Empty4.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded4.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready4.Visibility = System.Windows.Visibility.Hidden;
                                           Empty4.Visibility = System.Windows.Visibility.Visible;
                                           Loaded4.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready4.Visibility = System.Windows.Visibility.Hidden;
                                           Empty4.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded4.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready4.Visibility = System.Windows.Visibility.Visible;
                                           Empty4.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded4.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 4:
                                       if (station[n] == 0.0)
                                       {
                                           Ready5.Visibility = System.Windows.Visibility.Hidden;
                                           Empty5.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded5.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready5.Visibility = System.Windows.Visibility.Hidden;
                                           Empty5.Visibility = System.Windows.Visibility.Visible;
                                           Loaded5.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready5.Visibility = System.Windows.Visibility.Hidden;
                                           Empty5.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded5.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready5.Visibility = System.Windows.Visibility.Visible;
                                           Empty5.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded5.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 5:
                                       if (station[n] == 0.0)
                                       {
                                           Ready6.Visibility = System.Windows.Visibility.Hidden;
                                           Empty6.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded6.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready6.Visibility = System.Windows.Visibility.Hidden;
                                           Empty6.Visibility = System.Windows.Visibility.Visible;
                                           Loaded6.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready6.Visibility = System.Windows.Visibility.Hidden;
                                           Empty6.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded6.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready6.Visibility = System.Windows.Visibility.Visible;
                                           Empty6.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded6.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 6:
                                       if (station[n] == 0.0)
                                       {
                                           Ready7.Visibility = System.Windows.Visibility.Hidden;
                                           Empty7.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded7.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready7.Visibility = System.Windows.Visibility.Hidden;
                                           Empty7.Visibility = System.Windows.Visibility.Visible;
                                           Loaded7.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready7.Visibility = System.Windows.Visibility.Hidden;
                                           Empty7.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded7.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready7.Visibility = System.Windows.Visibility.Visible;
                                           Empty7.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded7.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 7:
                                       if (station[n] == 0.0)
                                       {
                                           Ready8.Visibility = System.Windows.Visibility.Hidden;
                                           Empty8.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded8.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready8.Visibility = System.Windows.Visibility.Hidden;
                                           Empty8.Visibility = System.Windows.Visibility.Visible;
                                           Loaded8.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready8.Visibility = System.Windows.Visibility.Hidden;
                                           Empty8.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded8.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready8.Visibility = System.Windows.Visibility.Visible;
                                           Empty8.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded8.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 8:
                                       if (station[n] == 0.0)
                                       {
                                           Ready9.Visibility = System.Windows.Visibility.Hidden;
                                           Empty9.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded9.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready9.Visibility = System.Windows.Visibility.Hidden;
                                           Empty9.Visibility = System.Windows.Visibility.Visible;
                                           Loaded9.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready9.Visibility = System.Windows.Visibility.Hidden;
                                           Empty9.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded9.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready9.Visibility = System.Windows.Visibility.Visible;
                                           Empty9.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded9.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 9:
                                       if (station[n] == 0.0)
                                       {
                                           Ready10.Visibility = System.Windows.Visibility.Hidden;
                                           Empty10.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded10.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready10.Visibility = System.Windows.Visibility.Hidden;
                                           Empty10.Visibility = System.Windows.Visibility.Visible;
                                           Loaded10.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready10.Visibility = System.Windows.Visibility.Hidden;
                                           Empty10.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded10.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready10.Visibility = System.Windows.Visibility.Visible;
                                           Empty10.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded10.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                                   case 10:
                                       if (station[n] == 0.0)
                                       {
                                           Ready11.Visibility = System.Windows.Visibility.Hidden;
                                           Empty11.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded11.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.1)
                                       {
                                           Ready11.Visibility = System.Windows.Visibility.Hidden;
                                           Empty11.Visibility = System.Windows.Visibility.Visible;
                                           Loaded11.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else if (station[n] == 0.2)
                                       {
                                           Ready11.Visibility = System.Windows.Visibility.Hidden;
                                           Empty11.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded11.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       else if (station[n] == 0.3)
                                       {
                                           Ready11.Visibility = System.Windows.Visibility.Visible;
                                           Empty11.Visibility = System.Windows.Visibility.Hidden;
                                           Loaded11.Visibility = System.Windows.Visibility.Visible;
                                       }
                                       break;
                               }
                           }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
