using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10HSI : UserControl, IHawgTouchGauge
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

        public A10HSI(/*string strDataImportID*/)
        {
            InitializeComponent();
            // _DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           
                           double card = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                           double no1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
                           double no2 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);
                           double marker = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture);
                           double arrow = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture);
                           double deviation = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture);
                           double hundred = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture) * 1000;
                           double ten = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture) * 100;
                           double one = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture) * 10;
                           double to = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture);
                           double from = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture);
                           double total = hundred + ten + one;
                           string TOTAL = ((int)total).ToString();

                           RotateTransform rt = new RotateTransform();
                           RotateTransform rt2 = new RotateTransform();
                           RotateTransform rt3 = new RotateTransform();
                           RotateTransform rt4 = new RotateTransform();
                           RotateTransform rt5 = new RotateTransform();
                           RotateTransform rt6 = new RotateTransform();
                           rt.Angle = card * 360;
                           this.CompassCard.RenderTransform = rt;
                           rt2.Angle = no1 * 360;
                           this.Indicator1.RenderTransform = rt2;
                           rt3.Angle = no2 * 360;
                           this.Indicator2.RenderTransform = rt3;
                           rt4.Angle = marker * 360;
                           this.CourseIndicator.RenderTransform = rt4;
                           rt5.Angle = arrow * 360;

                           this.TopPlate.RenderTransform = rt5;
                           TranslateTransform tt = new TranslateTransform();
                           tt.X = deviation * 65;
                           this.CDI.RenderTransform = tt;

                           int finalcourse = 0;
                            finalcourse = ((int)((RotateTransform)this.TopPlate.RenderTransform).Angle - (int)((RotateTransform)this.CompassCard.RenderTransform).Angle) * 1;
                            if (finalcourse < 0)
                                finalcourse = 360 - (finalcourse * -1);


                           CourseText.Text = finalcourse.ToString();

                           SolidColorBrush white = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
                           SolidColorBrush black = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,255,255));

                           if(to == 1.0000)
                               this.FROM.Fill = white;
                           else
                               this.FROM.Fill = black;

                           
                           if(from == 1.0000)
                               this.TO.Fill = white;
                           else
                               this.TO.Fill = black;

                           if (TOTAL.Length == 1)
                               MilesText.Text = "0 0 " + TOTAL[0];
                           else if (TOTAL.Length == 2)
                               MilesText.Text = "0 " + TOTAL[0] + " " + TOTAL[1];
                           else if (TOTAL.Length == 3)
                               MilesText.Text = TOTAL[0] + " " + TOTAL[1] + " " + TOTAL[2];
                       }));
        }
    }
}
