using System;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10ILS : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;

        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(300, 100);
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

        public A10ILS(/*string strDataImportID*/)
        {
            InitializeComponent();
            //_DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {

            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {

                           double left = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture) * 10;
                           double middle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture) * 10;
                           double right = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture) * 10;
                           double tleft = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture) * 10;
                           double tright = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture) * 10;

                           this.LeftT.Text = ((int)left).ToString();
                           this.MiddleT.Text = ((int)middle).ToString();
                           this.RightT.Text = ((int)right).ToString();
                           this.TLeft.Text = ((int)tleft).ToString();
                           this.TRight.Text = ((int)tright).ToString();

                       }));
        }
    }
}
