﻿#pragma checksum "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AFB469B0A5F6ED435CAB25591949B323"
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
    /// UH1_RadioCompassIndicator2
    /// </summary>
    public partial class UH1_RadioCompassIndicator2 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UH1_RadioCompassIndicator_1;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas background;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas RMI_Heading;
        
        #line default
        #line hidden
        
        
        #line 290 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas RMI_CoursePointer2;
        
        #line default
        #line hidden
        
        
        #line 297 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas RMI_CoursePointer1;
        
        #line default
        #line hidden
        
        
        #line 303 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas front;
        
        #line default
        #line hidden
        
        
        #line 309 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 310 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/uh1_radiocompassindicator2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
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
            
            #line 7 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
            ((Ikarus.UH1_RadioCompassIndicator2)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UH1_RadioCompassIndicator_1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.background = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.RMI_Heading = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.RMI_CoursePointer2 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.RMI_CoursePointer1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.front = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 309 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 310 "..\..\..\Gauges\UH1_RadioCompassIndicator2.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

