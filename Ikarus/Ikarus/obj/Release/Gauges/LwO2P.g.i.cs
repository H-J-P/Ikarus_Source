﻿#pragma checksum "..\..\..\Gauges\LwO2P.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6E041AB191E7F459334AA45FBD07A1B"
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
    /// LwO2P
    /// </summary>
    public partial class LwO2P : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Gauges\LwO2P.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas LwO2P1;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\Gauges\LwO2P.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Lw_O2P_Bezel;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Gauges\LwO2P.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Lw_O2P_Dial;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\Gauges\LwO2P.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Lw_O2P_Needle;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\..\Gauges\LwO2P.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\Gauges\LwO2P.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/lwo2p.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gauges\LwO2P.xaml"
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
            
            #line 7 "..\..\..\Gauges\LwO2P.xaml"
            ((Ikarus.LwO2P)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LwO2P1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.Lw_O2P_Bezel = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.Lw_O2P_Dial = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.Lw_O2P_Needle = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 132 "..\..\..\Gauges\LwO2P.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 133 "..\..\..\Gauges\LwO2P.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
