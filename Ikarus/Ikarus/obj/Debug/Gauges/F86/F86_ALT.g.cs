﻿#pragma checksum "..\..\..\..\Gauges\F86\F86_ALT.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BE643C6F16984F9693B37763DCC5B2F1"
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
    /// F86_ALT
    /// </summary>
    public partial class F86_ALT : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas P51_ALT;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Altimeter_Pressure;
        
        #line default
        #line hidden
        
        
        #line 696 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas dial;
        
        #line default
        #line hidden
        
        
        #line 806 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Altimeter_10000_footPtr;
        
        #line default
        #line hidden
        
        
        #line 810 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Altimeter_1000_footPtr;
        
        #line default
        #line hidden
        
        
        #line 814 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Altimeter_100_footPtr;
        
        #line default
        #line hidden
        
        
        #line 820 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas front;
        
        #line default
        #line hidden
        
        
        #line 1120 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 1121 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/f86/f86_alt.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
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
            
            #line 7 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
            ((Ikarus.F86_ALT)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.P51_ALT = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.Altimeter_Pressure = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.dial = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.Altimeter_10000_footPtr = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.Altimeter_1000_footPtr = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.Altimeter_100_footPtr = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.front = ((System.Windows.Controls.Canvas)(target));
            return;
            case 9:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 1120 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 1121 "..\..\..\..\Gauges\F86\F86_ALT.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

