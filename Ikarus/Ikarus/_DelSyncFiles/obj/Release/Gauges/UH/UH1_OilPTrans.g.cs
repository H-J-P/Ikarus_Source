﻿#pragma checksum "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A4684FC1FCDD895081EB529062D98A36"
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
    /// UH1_OilPTrans
    /// </summary>
    public partial class UH1_OilPTrans : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UH1_OilP_trans;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas dial1;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas TransmOilPress;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Light;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/gauges/uh/uh1_oilptrans.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
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
            
            #line 7 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
            ((Ikarus.UH1_OilPTrans)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UH1_OilP_trans = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.dial1 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.TransmOilPress = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.Light = ((System.Windows.Controls.Image)(target));
            
            #line 72 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
            this.Light.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Frame = ((System.Windows.Controls.Image)(target));
            
            #line 73 "..\..\..\..\Gauges\UH\UH1_OilPTrans.xaml"
            this.Frame.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Light_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
