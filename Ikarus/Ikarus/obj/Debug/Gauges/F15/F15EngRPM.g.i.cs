﻿#pragma checksum "..\..\..\..\Gauges\F15\F15EngRPM.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4E22BD69E2F64259430A0B5071E1525C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Ikarus {
    
    
    /// <summary>
    /// F15EngRPM
    /// </summary>
    public partial class F15EngRPM : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas F15_REV_Eng;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas background;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EngineRPM_100;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EngineRPM_10;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EngineRPM_1;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas dial;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EngineRPM;
        
        #line default
        #line hidden
        
        
        #line 175 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock RPMValue;
        
        #line default
        #line hidden
        
        
        #line 177 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 178 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Frame;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/f15/f15engrpm.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
            ((Ikarus.F15EngRPM)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.F15_REV_Eng = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.background = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.EngineRPM_100 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.EngineRPM_10 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.EngineRPM_1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.dial = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.EngineRPM = ((System.Windows.Controls.Canvas)(target));
            return;
            case 9:
            this.RPMValue = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 177 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 11:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 178 "..\..\..\..\Gauges\F15\F15EngRPM.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
