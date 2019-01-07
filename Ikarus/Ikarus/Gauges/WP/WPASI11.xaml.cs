using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPASI11.xaml
    /// </summary>
    public partial class WPASI11 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double ias = 0.0;
        double tas = 0.0;
        const int valueScaleIndex = 12;

        double lias = 0.0;
        double ltas = 0.0;

        RotateTransform rtIAS = new RotateTransform();
        RotateTransform rtTAS = new RotateTransform();

        public WPASI11()
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

                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { tas = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               // IAS               		                     {   0,     1,     2,     3,     4,     5,     6,     7,     8,     9,    10,    11 }
                               double[] valueScale = new double[valueScaleIndex] { 0.0, 0.091, 0.181, 0.272, 0.364, 0.455, 0.545, 0.636, 0.727, 0.818, 0.909, 1.000 };
                               double[] degreeDial = new double[valueScaleIndex] { 0, 9, 39, 90, 150, 180, 210, 239, 265, 293, 322, 350 };

                               if (lias != ias)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (ias >= valueScale[n] && ias <= valueScale[n + 1])
                                       {
                                           rtIAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ias - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   IAS.RenderTransform = rtIAS;
                               }
                               if (ltas != tas)
                               {
                                   const int index = 3;
                                   // TAS               		  {   0,     4,    11 }
                                   valueScale = new double[index] { 0.0, 0.001, 1.000 };
                                   degreeDial = new double[index] { 0, -0.1, -328 };


                                   for (int n = 0; n < index - 1; n++)
                                   {
                                       if (tas >= valueScale[n] && tas <= valueScale[n + 1])
                                       {
                                           rtTAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tas - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   TAS.RenderTransform = rtTAS;
                               }
                               lias = ias;
                               ltas = tas;
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
