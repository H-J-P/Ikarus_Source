using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
	/// <summary>
	/// Interaction logic for Spit_FuelP_WL.xaml
	/// </summary>
	public partial class Spit_FuelP_WL : UserControl
	{
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        double pointer = 0.0;
        double lpointer = 0.0;

        RotateTransform rtpointer = new RotateTransform();

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Spit_FuelP_WL()
		{
			this.InitializeComponent();
		}


        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}