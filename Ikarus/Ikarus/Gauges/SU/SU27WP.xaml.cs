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
    /// Interaktionslogik für SU27WP.xaml
    /// </summary>
    public partial class SU27WP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double[] weaponPresend = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        double[] weaponActive = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

        public SU27WP()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            Loaded1.Visibility = System.Windows.Visibility.Hidden;
            Loaded2.Visibility = System.Windows.Visibility.Hidden;
            Loaded3.Visibility = System.Windows.Visibility.Hidden;
            Loaded4.Visibility = System.Windows.Visibility.Hidden;
            Loaded5.Visibility = System.Windows.Visibility.Hidden;
            Loaded6.Visibility = System.Windows.Visibility.Hidden;
            Loaded7.Visibility = System.Windows.Visibility.Hidden;
            Loaded8.Visibility = System.Windows.Visibility.Hidden;
            Loaded9.Visibility = System.Windows.Visibility.Hidden;
            Loaded10.Visibility = System.Windows.Visibility.Hidden;

            Ready1.Visibility = System.Windows.Visibility.Hidden;
            Ready2.Visibility = System.Windows.Visibility.Hidden;
            Ready3.Visibility = System.Windows.Visibility.Hidden;
            Ready4.Visibility = System.Windows.Visibility.Hidden;
            Ready5.Visibility = System.Windows.Visibility.Hidden;
            Ready6.Visibility = System.Windows.Visibility.Hidden;
            Ready7.Visibility = System.Windows.Visibility.Hidden;
            Ready8.Visibility = System.Windows.Visibility.Hidden;
            Ready9.Visibility = System.Windows.Visibility.Hidden;
            Ready10.Visibility = System.Windows.Visibility.Hidden;
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
        }

        public double GetSize()
        {
            return 382.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               for (int n = 0; n < 10; n++)
                               {
                                   if (vals.Length > n) { weaponPresend[n] = Convert.ToDouble(vals[n], CultureInfo.InvariantCulture); }
                               }

                               for (int n = 10; n < 20; n++)
                               {
                                   if (vals.Length > n) { weaponActive[n - 10] = Convert.ToDouble(vals[n], CultureInfo.InvariantCulture); }
                               }

                               Loaded1.Visibility = (weaponPresend[0] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded2.Visibility = (weaponPresend[1] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded3.Visibility = (weaponPresend[2] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded4.Visibility = (weaponPresend[3] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded5.Visibility = (weaponPresend[4] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded6.Visibility = (weaponPresend[5] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded7.Visibility = (weaponPresend[6] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded8.Visibility = (weaponPresend[7] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded9.Visibility = (weaponPresend[8] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded10.Visibility = (weaponPresend[9] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Ready1.Visibility = (weaponActive[0] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready2.Visibility = (weaponActive[1] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready3.Visibility = (weaponActive[2] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready4.Visibility = (weaponActive[3] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready5.Visibility = (weaponActive[4] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready6.Visibility = (weaponActive[5] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready7.Visibility = (weaponActive[6] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready8.Visibility = (weaponActive[7] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready9.Visibility = (weaponActive[8] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready10.Visibility = (weaponActive[9] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           }
                           catch { return; }
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

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;


            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
