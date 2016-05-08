using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10ADIGauge : UserControl, IHawgTouchGauge
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

        public A10ADIGauge(/*string strDataImportID*/)
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

                           double pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                           double bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
                           double slipball = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);
                           double turn = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture);

                           TransformGroup grp = new TransformGroup();
                           

                           RotateTransform rt = new RotateTransform();
                           TranslateTransform tt = new TranslateTransform();
                           
                           tt.Y = pitch * -270;
                           rt.Angle = bank * -190;
                           grp.Children.Add(tt);
                           grp.Children.Add(rt);
                           this.Meter.RenderTransform = grp;

                           TranslateTransform sbT = new TranslateTransform();
                           sbT.X = slipball * 40;
                           this.Slipball.RenderTransform = sbT;

                           TranslateTransform tnT = new TranslateTransform();
                           tnT.X = turn * 125 + 137;
                           this.BankNeedle.RenderTransform = tnT;
                           //
                           //this.Meter.RenderTransform = rt;
                           //else if (TOTAL.Length == 2)
                           //    OneKOneH.Text = "0 " + TOTAL[0];
                       }));
        }
    }
}
