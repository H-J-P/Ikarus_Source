using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_ADI_Stby.xaml
    /// </summary>
    public partial class SA342_ADI_Stby : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pitch = 0.0;
        double bank = 0.0;
        double flagOff = 0.0;
        double silhouette = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lflagOff = 1.0;
        double lsilhouette = 0.0;

        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();
        RotateTransform rtFlagOff = new RotateTransform();
        RotateTransform rtTurn = new RotateTransform();
        TranslateTransform ttSilhouette = new TranslateTransform();

        public SA342_ADI_Stby()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
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

                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { flagOff = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { silhouette = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lpitch != pitch || lbank != bank)
                               {
                                   TransformGroup grp = new TransformGroup();

                                   tt.Y = pitch * (242);
                                   rt.Angle = bank * -180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);
                                   Bank_pitch.RenderTransform = grp;

                                   rtTurn.Angle = bank * -45;
                                   Bank.RenderTransform = rt;
                               }

                               if (lsilhouette != silhouette)
                               {
                                   if (silhouette < 0.0) { silhouette = 0.0; }
                                   //  0.0 0.5 1.0  <= SA342
                                   // -0.5 0.0 0.5  <= -0.5
                                   // -1.0 0.0 1.0  <= * 2
                                   ttSilhouette.Y = (silhouette - 0.5) * 2 * -28;
                                   Silhouette.RenderTransform = ttSilhouette;
                               }

                               if (lflagOff != flagOff)
                                   Flagg_off.Visibility = (flagOff > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lbank = bank;
                               lflagOff = flagOff;
                               lsilhouette = silhouette;
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
