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
    /// Interaction logic for KA50Gear.xaml
    /// </summary>
    public partial class KA50Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double lampNoseGearUp = 0.0;
        double lampNoseGearDown = 0.0;
        double lampRightMainGearUp = 0.0;
        double lampRightMainGearDown = 0.0;
        double lampLeftMainGearUp = 0.0;
        double lampLeftMainGearDown = 0.0;

        public KA50Gear()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            KA50_NUp_Gear.Visibility = System.Windows.Visibility.Hidden;
            KA50_NDn_Gear.Visibility = System.Windows.Visibility.Hidden;
            KA50_RUp_Gear.Visibility = System.Windows.Visibility.Hidden;
            KA50_RDn_Gear.Visibility = System.Windows.Visibility.Hidden;
            KA50_LUp_Gear.Visibility = System.Windows.Visibility.Hidden;
            KA50_LDn_Gear.Visibility = System.Windows.Visibility.Hidden;
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
            return 221.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { lampNoseGearUp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { lampNoseGearDown = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { lampRightMainGearUp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { lampRightMainGearDown = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { lampLeftMainGearUp = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { lampLeftMainGearDown = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               KA50_NUp_Gear.Visibility = (lampNoseGearUp > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_NDn_Gear.Visibility = (lampNoseGearDown > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_RUp_Gear.Visibility = (lampRightMainGearUp > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_RDn_Gear.Visibility = (lampRightMainGearDown > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_LUp_Gear.Visibility = (lampLeftMainGearUp > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_LDn_Gear.Visibility = (lampLeftMainGearDown > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
