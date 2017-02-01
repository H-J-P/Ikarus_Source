using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for A10FuelGauge.xaml
    /// </summary>
    public partial class A10FuelGauge : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;

        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(400, 400);
            }
        }

        public string DataImportID
        {
            get
            {
                return _DataImportID;
            }
            set
            {
                _DataImportID = value;
            }
        }

        public A10FuelGauge(/*string strDataImportID*/)
        {
            InitializeComponent();
            // _DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');
            double angelLeft = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture);
            double angelRight = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture);

         
            //double angle = Convert.ToDouble(strData);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double tens = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture) * 100000;
                           double thousands = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture) * 10000;
                           double hundreds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture) * 1000;
                           double total = tens + thousands + hundreds;
                           string TOTAL = ((int)total).ToString();

                           if (TOTAL.Length == 1)
                               TotalLBS.Content = "0 0 0 0 " + TOTAL[0];
                           else if (TOTAL.Length == 2)
                               TotalLBS.Content = "0 0 0 " + TOTAL[0] + " " + TOTAL[1];
                           else if (TOTAL.Length == 3)
                               TotalLBS.Content = "0 0 " + TOTAL[0] + " " + TOTAL[1] + " " + TOTAL[2];
                           else if (TOTAL.Length == 4)
                               TotalLBS.Content = "0 " + TOTAL[0] + " " + TOTAL[1] + " " + TOTAL[2] + " " + TOTAL[3];
                           else if (TOTAL.Length == 5)
                               TotalLBS.Content = TOTAL[0] + " " + TOTAL[1] + " " + TOTAL[2] + " " + TOTAL[3] + " " + TOTAL[4];

                           RotateTransform rt = new RotateTransform();
                           RotateTransform rl = new RotateTransform();
                           rt.Angle = angelLeft * 155;
                           rl.Angle = (angelRight * -155) -12;
                           this.LNeedle.RenderTransform = rt;
                           this.RNeedle.RenderTransform = rl;
                       }));
        }
    }
}
