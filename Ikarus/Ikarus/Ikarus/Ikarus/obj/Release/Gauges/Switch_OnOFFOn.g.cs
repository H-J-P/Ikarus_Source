﻿#pragma checksum "..\..\..\Gauges\Switch_OnOFFOn.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C8F21BBA470AEAFBF27391EFF9E6E9C0"
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
    /// Switch_OnOFFOn
    /// </summary>
    public partial class Switch_OnOFFOn : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image SwitchUp;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image SwitchMiddle;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image SwitchDown;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle DesignFrame;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle UpperRec;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LowerRec;
        
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/switch_onoffon.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
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
            
            #line 8 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            ((Ikarus.Switch_OnOFFOn)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SwitchUp = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.SwitchMiddle = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.SwitchDown = ((System.Windows.Controls.Image)(target));
            return;
            case 5:
            this.DesignFrame = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 6:
            this.UpperRec = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 47 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.UpperRec.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.UpperRec_MouseDown);
            
            #line default
            #line hidden
            
            #line 48 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.UpperRec.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.UpperRec_MouseUp);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.UpperRec.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.UpperRec_TouchDown);
            
            #line default
            #line hidden
            
            #line 51 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.UpperRec.TouchUp += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.UpperRec_TouchUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.LowerRec = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 59 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.LowerRec.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.LowerRec_MouseDown);
            
            #line default
            #line hidden
            
            #line 60 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.LowerRec.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.LowerRec_MouseUp);
            
            #line default
            #line hidden
            
            #line 62 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.LowerRec.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.LowerRec_TouchDown);
            
            #line default
            #line hidden
            
            #line 63 "..\..\..\Gauges\Switch_OnOFFOn.xaml"
            this.LowerRec.TouchUp += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.LowerRec_TouchUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

