﻿#pragma checksum "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A7F2C857EE38722BDA17E379AAC718B1"
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
    /// LampOnOffOld
    /// </summary>
    public partial class LampOnOffOld : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image LampOn;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image LampOff;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle UpperRec;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle DesignFrame;
        
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/lamp/lamponoffold.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
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
            
            #line 7 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
            ((Ikarus.LampOnOffOld)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LampOn = ((System.Windows.Controls.Image)(target));
            
            #line 9 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
            this.LampOn.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 3:
            this.LampOff = ((System.Windows.Controls.Image)(target));
            
            #line 10 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
            this.LampOff.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 4:
            this.UpperRec = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 11 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
            this.UpperRec.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.UpperRec_MouseDown);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\Gauges\Lamp\LampOnOffOld.xaml"
            this.UpperRec.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.UpperRec_TouchDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DesignFrame = ((System.Windows.Shapes.Rectangle)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

