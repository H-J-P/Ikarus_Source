using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10AltimeterGauge : UserControl, IHawgTouchGauge
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

        public A10AltimeterGauge(/*string strDataImportID*/)
        {
            InitializeComponent();
            // _DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');


            //double angle = Convert.ToDouble(strData);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {

                           double needle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                           double tenk = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture) * 100000;
                           double thousand = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture) * 10000;
                           double hundred = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture) * 1000;
                           double total = tenk + thousand + hundred;
                           string TOTAL = ((int)total).ToString();
                           if (total > 10000)
                               this.TenThousandCover.Visibility = Visibility.Hidden;
                           else
                               this.TenThousandCover.Visibility = Visibility.Visible;

                           RotateTransform rt = new RotateTransform();
                           rt.Angle = needle * 360;
                           this.Needle.RenderTransform = rt;
                           string ten10str = tenk.ToString();
                           TenK.Text = ten10str;

                           if(TOTAL.Length == 5)
                               OneKOneH.Text = TOTAL[1] + " " + TOTAL[2];
                           else if (TOTAL.Length == 4)
                           {
                               string firstK = TOTAL[0].ToString();
                               int totalk = Convert.ToInt32(firstK) * 1000;
                               int fk = Convert.ToInt32(TOTAL);
                               totalk = totalk + (int)hundred;
                               TOTAL = totalk.ToString();
                               if (TOTAL[1] == '9')
                               {
                                   totalk = totalk + (int)hundred - 2000;
                                   TOTAL = totalk.ToString();
                               }

                               OneKOneH.Text = TOTAL[0] + " " + TOTAL[1];
                           }
                           else if (TOTAL.Length == 3)
                               OneKOneH.Text = "0 " + TOTAL[0];
                           //else if (TOTAL.Length == 2)
                           //    OneKOneH.Text = "0 " + TOTAL[0];
                       }));
        }
    }
}
