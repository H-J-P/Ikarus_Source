﻿#pragma checksum "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "95F5A51DF414980D1BE18574FFD3ADC4"
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
    /// MI8_TurnIndicator
    /// </summary>
    public partial class MI8_TurnIndicator : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas MI8_TurnIndicator1;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas background;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EUP_sideslip;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas dial;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas EUP_speed;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas front;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/mi8/mi8_turnindicator.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
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
            
            #line 7 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
            ((Ikarus.MI8_TurnIndicator)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MI8_TurnIndicator1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.background = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.EUP_sideslip = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.dial = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.EUP_speed = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.front = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 59 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 60 "..\..\..\..\Gauges\MI8\MI8_TurnIndicator.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
