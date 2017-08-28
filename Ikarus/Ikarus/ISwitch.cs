using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing;

namespace Ikarus
{
    public interface I_Switches
    {
        string Name
        {
            get;
        }

        void SetID(string _dataImportID);

        string GetID();

        void SetWindowID(int _windowID);

        int GetWindowID();

        void SwitchLight(bool _on);

        void SetInput(string _input);

        void SetOutput(string _output);
        
        double GetSize();

        double GetSizeY();

        void UpdateGauge(string strData);
    }
}
