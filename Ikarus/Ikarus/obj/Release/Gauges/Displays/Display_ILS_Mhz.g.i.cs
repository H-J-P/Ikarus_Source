﻿#pragma checksum "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7725FD03255548A736AF9DF43A940873"
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
    /// Display_ILS_Mhz
    /// </summary>
    public partial class Display_ILS_Mhz : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Frame;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas VHF1V2;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Layer_1;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Display;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/displays/display_ils_mhz.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
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
            
            #line 8 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
            ((Ikarus.Display_ILS_Mhz)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 17 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 3:
            this.VHF1V2 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.Layer_1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.Display = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 75 "..\..\..\..\Gauges\Displays\Display_ILS_Mhz.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

