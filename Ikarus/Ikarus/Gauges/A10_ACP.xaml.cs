using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

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

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }


        public A10_ACP()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

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
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frame = dataRows[0]["ImageFrame"].ToString();
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frame.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame))
                            Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                    }
                    if (light.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                            Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                    }
                    SwitchLight(false);
                }
                catch { }
            }
        }

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');
            
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

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
        }
    }
}
