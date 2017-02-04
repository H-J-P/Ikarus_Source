﻿#pragma checksum "..\..\MessageBoxAccessories.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BD114B4F322E1C0B5B862EDAAEA8E6DE"
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
    /// MessageBoxAccessories
    /// </summary>
    public partial class MessageBoxAccessories : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ImageOn;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImageOnSelect;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox WindowID;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PosX;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PosY;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Size;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Classname;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Desc;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\MessageBoxAccessories.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Rotate;
        
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
            System.Uri resourceLocater = new System.Uri("/Ikarus;component/messageboxaccessories.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MessageBoxAccessories.xaml"
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
            this.ImageOn = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.ImageOnSelect = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\MessageBoxAccessories.xaml"
            this.ImageOnSelect.Click += new System.Windows.RoutedEventHandler(this.ImageOnSelect_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.WindowID = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            
            #line 65 "..\..\MessageBoxAccessories.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.PosX = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.PosY = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.Size = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.Classname = ((System.Windows.Controls.ComboBox)(target));
            
            #line 121 "..\..\MessageBoxAccessories.xaml"
            this.Classname.DropDownClosed += new System.EventHandler(this.Classname_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Desc = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.Rotate = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

