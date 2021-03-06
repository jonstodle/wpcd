﻿#pragma checksum "C:\Dev\wpcd\wpcd\Pages\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FD3CCF4E5D4510405C1E7DFFAD32F700"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using wpcd;
using wpcd.Converters;


namespace wpcd.Pages {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal wpcd.Converters.VisibilityDependentOnFavoriteConverter VDF;
        
        internal wpcd.Converters.BrushDependentOnUnreadConverter BDU;
        
        internal wpcd.Converters.StarDependontOnFavoriteConverter SDF;
        
        internal wpcd.Converters.VisibilityDependentOnStringConverter VDS;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.Pivot MainPivot;
        
        internal Telerik.Windows.Controls.RadJumpList AllList;
        
        internal Telerik.Windows.Controls.RadJumpList UnreadList;
        
        internal Telerik.Windows.Controls.RadJumpList FavoritesList;
        
        internal System.Windows.Controls.Border SortTypeBorder;
        
        internal System.Windows.Controls.TextBlock SortTypeText;
        
        internal Telerik.Windows.Controls.RadWindow ComicWindow;
        
        internal Telerik.Windows.Controls.RadMoveYAnimation ComicWindowOpenAnimation;
        
        internal Telerik.Windows.Controls.RadMoveYAnimation ComicWindowCloseAnimation;
        
        internal System.Windows.Controls.Grid WindowGrid;
        
        internal wpcd.CustomPanAndZoomImage ComicImage;
        
        internal System.Windows.Controls.Grid OverlayGrid;
        
        internal Telerik.Windows.Controls.RadWindow NotificationWindow;
        
        internal System.Windows.Controls.Border NotificationBorder;
        
        internal System.Windows.Controls.TextBlock Notification;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/wpcd;component/Pages/MainPage.xaml", System.UriKind.Relative));
            this.VDF = ((wpcd.Converters.VisibilityDependentOnFavoriteConverter)(this.FindName("VDF")));
            this.BDU = ((wpcd.Converters.BrushDependentOnUnreadConverter)(this.FindName("BDU")));
            this.SDF = ((wpcd.Converters.StarDependontOnFavoriteConverter)(this.FindName("SDF")));
            this.VDS = ((wpcd.Converters.VisibilityDependentOnStringConverter)(this.FindName("VDS")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.MainPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("MainPivot")));
            this.AllList = ((Telerik.Windows.Controls.RadJumpList)(this.FindName("AllList")));
            this.UnreadList = ((Telerik.Windows.Controls.RadJumpList)(this.FindName("UnreadList")));
            this.FavoritesList = ((Telerik.Windows.Controls.RadJumpList)(this.FindName("FavoritesList")));
            this.SortTypeBorder = ((System.Windows.Controls.Border)(this.FindName("SortTypeBorder")));
            this.SortTypeText = ((System.Windows.Controls.TextBlock)(this.FindName("SortTypeText")));
            this.ComicWindow = ((Telerik.Windows.Controls.RadWindow)(this.FindName("ComicWindow")));
            this.ComicWindowOpenAnimation = ((Telerik.Windows.Controls.RadMoveYAnimation)(this.FindName("ComicWindowOpenAnimation")));
            this.ComicWindowCloseAnimation = ((Telerik.Windows.Controls.RadMoveYAnimation)(this.FindName("ComicWindowCloseAnimation")));
            this.WindowGrid = ((System.Windows.Controls.Grid)(this.FindName("WindowGrid")));
            this.ComicImage = ((wpcd.CustomPanAndZoomImage)(this.FindName("ComicImage")));
            this.OverlayGrid = ((System.Windows.Controls.Grid)(this.FindName("OverlayGrid")));
            this.NotificationWindow = ((Telerik.Windows.Controls.RadWindow)(this.FindName("NotificationWindow")));
            this.NotificationBorder = ((System.Windows.Controls.Border)(this.FindName("NotificationBorder")));
            this.Notification = ((System.Windows.Controls.TextBlock)(this.FindName("Notification")));
        }
    }
}

