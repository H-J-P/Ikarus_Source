﻿#pragma checksum "..\..\..\Gauges\KA50ADIStby.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9798D839667F0A985923BC7D64934BF1"
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
    /// KA50ADIStby
    /// </summary>
    public partial class KA50ADIStby : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_ADI_stby;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_pitch_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_backgrd_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_dial_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 175 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_plane_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 178 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_needleSLIP_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 189 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_knobCage_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 455 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_needleOFF_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 460 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas KA50_bezel_ADIstb;
        
        #line default
        #line hidden
        
        
        #line 467 "..\..\..\Gauges\KA50ADIStby.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 468 "..\..\..\Gauges\KA50ADIStby.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/ka50adistby.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gauges\KA50ADIStby.xaml"
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
            
            #line 7 "..\..\..\Gauges\KA50ADIStby.xaml"
            ((Ikarus.KA50ADIStby)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.KA50_ADI_stby = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.KA50_pitch_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.KA50_backgrd_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.KA50_dial_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.KA50_plane_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.KA50_needleSLIP_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.KA50_knobCage_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 9:
            this.KA50_needleOFF_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 10:
            this.KA50_bezel_ADIstb = ((System.Windows.Controls.Canvas)(target));
            return;
            case 11:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 467 "..\..\..\Gauges\KA50ADIStby.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 12:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 468 "..\..\..\Gauges\KA50ADIStby.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

