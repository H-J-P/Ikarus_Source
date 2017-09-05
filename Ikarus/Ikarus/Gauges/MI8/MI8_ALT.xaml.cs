using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
        GaugesHelper helper = null;

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

            //RotateTransform rtvd10KL10Press = new RotateTransform();
            //rtvd10KL10Press.Angle = 5;
            //Pressure.RenderTransform = rtvd10KL10Press;
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
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
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
                                   rtvd10KL10Press.Angle = (vd10KL10Press * -360); // + 5;
                                   Pressure.RenderTransform = rtvd10KL10Press;
                               }
                               lvd10KL100Ind = vd10KL100Ind;
                               lvd10KL10Ind = vd10KL10Ind;
                               lvd10KL10Press = vd10KL10Press;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
