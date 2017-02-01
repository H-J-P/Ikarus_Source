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
    /// Interaction logic for MI8_ALT.xaml
    /// </summary>
    public partial class MI8_ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double vd10KL100Ind = 0.0;
        double vd10KL10Ind = 0.0;
        double vd10KL10Press = 0.0;

        double lvd10KL100Ind = 0.0;
        double lvd10KL10Ind = 0.0;
        double lvd10KL10Press = 0.0;

        RotateTransform rtvd10KL100Ind = new RotateTransform();
        RotateTransform rtvd10KL10Ind = new RotateTransform();
        RotateTransform rtvd10KL10Press = new RotateTransform();

        public MI8_ALT()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            RotateTransform rtvd10KL10Press = new RotateTransform();
            rtvd10KL10Press.Angle = 5;
            VD_10K_L_PRESS.RenderTransform = rtvd10KL10Press;
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { vd10KL100Ind = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { vd10KL10Ind = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { vd10KL10Press = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lvd10KL100Ind != vd10KL100Ind)
                               {
                                   rtvd10KL100Ind.Angle = vd10KL100Ind * 360;
                                   VD_10K_L_100_Ind.RenderTransform = rtvd10KL100Ind;
                               }
                               if (lvd10KL10Ind != vd10KL10Ind)
                               {
                                   rtvd10KL10Ind.Angle = vd10KL10Ind * 360;
                                   VD_10K_L_10_Ind.RenderTransform = rtvd10KL10Ind;
                               }
                               if (lvd10KL10Press != vd10KL10Press)
                               {
                                   rtvd10KL10Press.Angle = (vd10KL10Press * -360) + 5;
                                   VD_10K_L_PRESS.RenderTransform = rtvd10KL10Press;
                               }
                               lvd10KL100Ind = vd10KL100Ind;
                               lvd10KL10Ind = vd10KL10Ind;
                               lvd10KL10Press = vd10KL10Press;
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
