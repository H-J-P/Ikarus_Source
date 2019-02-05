using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG29HYD.xaml
    /// </summary>
    public partial class MIG29HYD : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pRight = 0.0;
        double pLeft = 0.0;
        double oRight = 0.0;
        double oLeft = 0.0;

        double lpRight = 0.0;
        double lpLeft = 0.0;
        double loRight = 0.0;
        double loLeft = 0.0;

        TranslateTransform ttpRight = new TranslateTransform();
        TranslateTransform ttpLeft = new TranslateTransform();
        TranslateTransform ttoRight = new TranslateTransform();
        TranslateTransform ttoLeft = new TranslateTransform();

        public MIG29HYD()
        {
            InitializeComponent();
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

                               if (vals.Length > 0) { pRight = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pLeft = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { oRight = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { oLeft = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (pRight < 0.0) pRight = 0.0;
                               if (pLeft < 0.0) pLeft = 0.0;
                               if (oRight < 0.0) oRight = 0.0;
                               if (oLeft < 0.0) oLeft = 0.0;

                               if (lpRight != pRight)
                               {
                                   ttpRight.Y = pRight * -160;
                                   P_right.RenderTransform = ttpRight;
                               }
                               if (lpLeft != pLeft)
                               {
                                   ttpLeft.Y = pLeft * -160;
                                   P_left.RenderTransform = ttpRight;
                               }
                               if (loRight != oRight)
                               {
                                   ttoRight.Y = oRight * -160;
                                   O_right.RenderTransform = ttoRight;
                               }
                               if (loLeft != oLeft)
                               {
                                   ttoLeft.Y = oLeft * -160;
                                   O_left.RenderTransform = ttoRight;
                               }
                               lpRight = pRight;
                               lpLeft = pLeft;
                               loRight = oRight;
                               loLeft = oLeft;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
