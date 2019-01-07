using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_Turn.xaml
    /// </summary>
    public partial class AV8BNA_Turn : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double turn = 0.0;
        double ball = 0.0;
        double flag = 0.0;

        double lturn = 0.0;
        double lball = 0.0;
        double lflag = 0.0;

        TranslateTransform ttTurn = new TranslateTransform();
        RotateTransform rtBall = new RotateTransform();

        public AV8BNA_Turn()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            TURN.Visibility = System.Windows.Visibility.Visible;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { turn = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { ball = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { flag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lturn != turn)
                               {
                                   ttTurn.X = turn * 74;
                                   TURN.RenderTransform = ttTurn;
                               }
                               if (lball != ball)
                               {
                                   rtBall.Angle = ball * 15;
                                   SLIP.RenderTransform = rtBall;
                               }
                               if (lflag != flag)
                               {
                                   OFF_FLAG.Visibility = flag > 0.5 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               }
                               lturn = turn;
                               lball = ball;
                               lflag = flag;
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
