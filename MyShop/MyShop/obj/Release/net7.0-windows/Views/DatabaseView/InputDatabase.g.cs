﻿#pragma checksum "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B943BF9A43B2A39472FA43B666DCB3A84B5D0891"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MyShop;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace MyShop.pages {
    
    
    /// <summary>
    /// InputDatabase
    /// </summary>
    public partial class InputDatabase : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image closeImageButton;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderDatabase;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textDatabase;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtDatabase;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button connectDatabaseButton;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button backButton;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar loadingBar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.11.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MyShop;component/views/databaseview/inputdatabase.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.11.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            ((MyShop.pages.InputDatabase)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.closeImageButton = ((System.Windows.Controls.Image)(target));
            
            #line 19 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.closeImageButton.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Image_MouseUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.borderDatabase = ((System.Windows.Controls.Border)(target));
            
            #line 51 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.borderDatabase.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.borderDatabase_MouseDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.textDatabase = ((System.Windows.Controls.TextBlock)(target));
            
            #line 62 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.textDatabase.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.textDatabase_MouseDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtDatabase = ((System.Windows.Controls.TextBox)(target));
            
            #line 63 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.txtDatabase.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtDatabase_TextChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.connectDatabaseButton = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.connectDatabaseButton.Click += new System.Windows.RoutedEventHandler(this.connectDatabaseButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.backButton = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\..\..\..\Views\DatabaseView\InputDatabase.xaml"
            this.backButton.Click += new System.Windows.RoutedEventHandler(this.backButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.loadingBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

