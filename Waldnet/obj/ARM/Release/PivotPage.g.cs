﻿

#pragma checksum "C:\Users\speed\Documents\GitHub\WaldnetApp\Waldnet\PivotPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "80BB1F26AF10936DC4EFB1C5BA76CDC4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Waldnet
{
    partial class PivotPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 31 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Pivot)(target)).SelectionChanged += this.pivot_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 202 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.SearchTextbox_KeyDown;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 208 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.SearchResultList_ItemClick;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 142 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 64 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 302 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.WaldnetButton_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 303 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.WaldNetSearchButton_Click;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 304 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.RefreshButton_Click;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 306 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SettingsButton_Click;
                 #line default
                 #line hidden
                break;
            case 10:
                #line 307 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.PrivacyPolicyButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


