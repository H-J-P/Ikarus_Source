﻿#pragma checksum "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E5D912A39B7D2A0ABB99F0C3125905FF"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
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
    /// Button
    /// </summary>
    public partial class Button : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image SwitchUp;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image SwitchDown;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle UpperRec;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/buttonandswitches/button.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
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
            
            #line 8 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
            ((Ikarus.Button)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SwitchUp = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.SwitchDown = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.UpperRec = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 30 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
            this.UpperRec.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.UpperRec_MouseDown);
            
            #line default
            #line hidden
            
            #line 31 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
            this.UpperRec.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.UpperRec_MouseUp);
            
            #line default
            #line hidden
            
            #line 33 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
            this.UpperRec.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.UpperRec_TouchDown);
            
            #line default
            #line hidden
            
            #line 34 "..\..\..\..\Gauges\ButtonAndSwitches\Button.xaml"
            this.UpperRec.TouchUp += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.UpperRec_TouchUp);
            
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

