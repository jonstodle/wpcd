using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Xkcd;
using System.Collections.ObjectModel;
using System.Threading;

namespace wpcd.Pages {
    public partial class MainPage : PhoneApplicationPage {
        private bool FirstLoad = true;
        private const string BackgroundTaskName = "UpdateTileTask";
        private const string NoNetworkMessage = "No network connection available";
        private const string CannotLoadNewerMessage = "Newer comics couldn't be loaded\nTap to try again";
        private const string CannotLoadOlderMessage = "Older comics couldn't be loaded\nTap to try again";

        public MainPage() {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            FilterTimer.Tick += FilterTimer_Tick;
            OverlayGridTimer.Tick += OverlayGridTimer_Tick;
            NotificationTimer.Tick += NotificationTimer_Tick;

            new Thread(() => FilterLists()).Start();
        }

        private void FilterLists() {
            Dispatcher.BeginInvoke(() => {
                UnreadList.FilterDescriptors.Add(new GenericFilterDescriptor<Comic>(c => c.Unread));
                FavoritesList.FilterDescriptors.Add(new GenericFilterDescriptor<Comic>(c => c.Favorite));
            });
        }

        #region Page
        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            if(FirstLoad) {
                FirstLoad = false;
                MainPivot.SelectionChanged += MainPivot_SelectionChanged;
                if(NetworkInterface.GetIsNetworkAvailable()) {
                    if((DataContext as Settings).ComicList.Count == 0) {
                        try {
                            (DataContext as Settings).ComicList.Add(await XkcdInterface.GetCurrentComic());
                            System.Diagnostics.Debug.WriteLine((DataContext as Settings).ComicList.Count);
                        } catch(WebException) {
                            ShowNotification("Comic couldn't be loaded", 4000);
                        }
                    } else {
                        GetNewerComics();
                    }
                    if(ScheduledActionService.Find(BackgroundTaskName) == null) {
                        var task = new PeriodicTask(BackgroundTaskName) {
                            Description = "Gets the number of new comics from xkcd"
                        };
                        ScheduledActionService.Add(task);
                    }
                } else {
                    ShowNotification(NoNetworkMessage, 4000);
                }
            }
        }

        private async void GetNewerComics() {
            try {
                var newComicsCount = await XkcdInterface.NumberOfNewerComics((DataContext as Settings).ComicList[0].Number);
                if(newComicsCount > 0) {
                    var newComics = await XkcdInterface.GetComics((DataContext as Settings).ComicList[0].Number + 1, (DataContext as Settings).ComicList[0].Number + newComicsCount);
                    foreach(var i in newComics) {
                        (DataContext as Settings).ComicList.Insert(0, i);
                    }
                }
                action = NotificationAction.DoNothing;
            } catch(WebException) {
                action = NotificationAction.TryLoadingNewer;
                ShowNotification(CannotLoadNewerMessage, 4000);
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e) {
            WindowGrid.Height = LayoutRoot.ActualHeight;
            WindowGrid.Width = LayoutRoot.ActualWidth;
            NotificationBorder.Width = LayoutRoot.ActualWidth;

        }

        private async void ContentPanel_Hold(object sender, System.Windows.Input.GestureEventArgs e) {
            switch(MainPivot.SelectedIndex) {
                case 0:
                    if(DialogResult.OK == (await RadMessageBox.ShowAsync("MARK AS UNREAD?", MessageBoxButtons.YesNo, "Do you want mark all comics as unread?", vibrate: false)).Result) {
                        foreach(var i in (DataContext as Settings).ComicList) {
                            i.Unread = true;
                        }
                    }
                    break;
                case 1:
                    if(DialogResult.OK == (await RadMessageBox.ShowAsync("MARK AS READ?", MessageBoxButtons.YesNo, "Do you want mark all comics as read?", vibrate: false)).Result) {
                        foreach(var i in (DataContext as Settings).ComicList) {
                            i.Unread = false;
                        }
                        UnreadList.RefreshData();
                    }
                    break;
                case 2:
                    if(DialogResult.OK == (await RadMessageBox.ShowAsync("UNFAVORITE?", MessageBoxButtons.YesNo, "Do you want to remove all favorites?", vibrate: false)).Result) {
                        foreach(var i in (DataContext as Settings).ComicList) {
                            i.Favorite = false;
                        }
                        UnreadList.RefreshData();
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Itemlists
        private void ItemList_ItemTap(object sender, Telerik.Windows.Controls.ListBoxItemTapEventArgs e) {
            (DataContext as Settings).SelectedComic = e.Item.DataContext as Comic;
            (DataContext as Settings).SelectedComic.Unread = false;
            ComicWindow.IsOpen = true;
            ResetWindow();
        }

        private void ItemList_DataRequested(object sender, EventArgs e) {
            if(NetworkInterface.GetIsNetworkAvailable()) {
                GetOlderComics();
            } else {
                ShowNotification(NoNetworkMessage, 4000);
            }
        }

        private async void GetOlderComics() {
            try {
                if((DataContext as Settings).ComicList.Count != 0) {
                    var cl = (DataContext as Settings).ComicList;
                    var lastComic = cl[cl.Count - 1];
                    var newComics = await XkcdInterface.GetComics(lastComic.Number - 6, lastComic.Number - 1);
                    newComics.Reverse();
                    foreach(var i in newComics) {
                        (DataContext as Settings).ComicList.Add(i);
                    }
                    if((DataContext as Settings).ComicList[(DataContext as Settings).ComicList.Count - 1].Number == 1) {
                        AllList.DataVirtualizationMode = DataVirtualizationMode.None;
                    }
                }
                action = NotificationAction.DoNothing;
            } catch(WebException) {
                action = NotificationAction.TryLoadingOlder;
                ShowNotification(CannotLoadOlderMessage, 4000);
            }
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var idx = (sender as Pivot).SelectedIndex;
            if(FilterTimer.IsEnabled) FilterTimer.Stop();
            if(idx == 0) {
                SortTypeText.Text = "ALL";
            } else if(idx == 1) {
                SortTypeText.Text = "UNREAD";
                UnreadList.RefreshData();
            } else if(idx == 2) {
                SortTypeText.Text = "FAVORITES";
                FavoritesList.RefreshData();
            }
            RadAnimationManager.Play(SortTypeBorder, new RadFadeAnimation { StartOpacity = 0, EndOpacity = .8, Duration = TimeSpan.FromMilliseconds(200) });
            FilterTimer.Start();
        }
        #endregion

        #region Filtering
        private DispatcherTimer FilterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        
        void FilterTimer_Tick(object sender, EventArgs e) {
            FilterTimer.Stop();
            RadAnimationManager.Play(SortTypeBorder, new RadFadeAnimation { StartOpacity = .8, EndOpacity = 0, Duration = TimeSpan.FromMilliseconds(200) });
        }
        #endregion

        #region Comic Window
        private DispatcherTimer OverlayGridTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        private object sender;

        private void WindowGrid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
            if(e.IsInertial) {
                if(ComicImage.Zoom == 1 || OverlayGrid.Visibility == Visibility.Visible) {
                    if(GestureHelper.GetDirection(e.FinalVelocities.LinearVelocity.X, e.FinalVelocities.LinearVelocity.Y) == GestureHelper.Direction.Down) {
                        ComicWindow.IsOpen = false;
                    }
                }
            }
        }

        private void ResetWindow() {
            ComicImage.Zoom = 1;
            ComicImage.Pan = new Point(0, 0);
            OverlayGrid.Opacity = 0;
            OverlayGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private async void RadImageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            e.Handled = true;
            var tag = (sender as RadImageButton).Tag as string;
            if(tag == "favorite") {
                (DataContext as Settings).SelectedComic.Favorite = !(DataContext as Settings).SelectedComic.Favorite;
            } else if(tag == "link") {
                Clipboard.SetText("http://xkcd.com/" + (DataContext as Settings).SelectedComic.Number.ToString());
                ShowNotification("Link copied to the clipboard");
            } else {
                try {
                    var comicNumber = (DataContext as Settings).SelectedComic.Number.ToString();
                    using(var isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                        Picture pic;
                        using(var library = new MediaLibrary()) {
                            var bi = new BitmapImage();
                            if(isoStore.FileExists(comicNumber + ".jpg")) {
                                using(var isoStoreFs = new IsolatedStorageFileStream(comicNumber + ".jpg", FileMode.Open, isoStore)) {
                                    bi.SetSource(isoStoreFs);
                                    System.Diagnostics.Debug.WriteLine("");
                                }
                            } else {
                                var client = new WebClient();
                                using(var result = await client.OpenReadTaskAsync((DataContext as Settings).SelectedComic.ImageUri)) {
                                    bi.SetSource(result);
                                }
                            }
                            var wb = new WriteableBitmap(bi);
                            using(var stream = new MemoryStream()) {
                                wb.SaveJpeg(stream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                                stream.Position = 0;
                                pic = library.SavePicture(comicNumber + ".jpg", stream);
                            }
                        }
                        if(tag == "share") {
                            new ShareMediaTask { FilePath = pic.GetPath() }.Show();
                        } else {
                            ShowNotification("Image saved");
                        }
                    }
                } catch(Exception) {
                    MessageBox.Show("There was a problem handling the image. That happens sometimes. Try again later", App.Current.Resources["GenericErrorTitle"] as string, MessageBoxButton.OK);
                }
            }
        }

        private void OverlayToggleTap(object sender, System.Windows.Input.GestureEventArgs e) {
            e.Handled = true;
            this.sender = sender;
            OverlayGridTimer.Start();
        }

        void OverlayGridTimer_Tick(object sender, EventArgs e) {
            OverlayGridTimer.Stop();
            if(!ComicImage.DoubleTapOccured) {
                if(this.sender == ComicImage) {
                    OverlayGrid.Visibility = System.Windows.Visibility.Visible;
                    RadAnimationManager.Play(OverlayGrid, new RadFadeAnimation { StartOpacity = 0, EndOpacity = 1, Duration = new Duration(TimeSpan.FromMilliseconds(200)) });
                } else {
                    var ani = new RadFadeAnimation { StartOpacity = 1, EndOpacity = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)) };
                    ani.Ended += (sndr, args) => {
                        OverlayGrid.Visibility = System.Windows.Visibility.Collapsed;
                    };
                    RadAnimationManager.Play(OverlayGrid, ani);
                }
            }
            ComicImage.DoubleTapOccured = false;
        }

        private void ComicWindow_WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            new Thread(() => RefreshData()).Start();
        }

        private void RefreshData() {
            Dispatcher.BeginInvoke(() => {
                if(MainPivot.SelectedIndex == 1) {
                    UnreadList.RefreshData();
                } else if(MainPivot.SelectedIndex == 2) {
                    FavoritesList.RefreshData();
                }
            });
        }
        #endregion

        #region Notification
        private enum NotificationAction {
            DoNothing, TryLoadingOlder, TryLoadingNewer
        };
        private DispatcherTimer NotificationTimer = new DispatcherTimer();
        private NotificationAction action = NotificationAction.DoNothing;

        private void ShowNotification(string message, int milliseconds = 2000) {
            NotificationWindow.IsOpen = false;
            Notification.Text = message;
            NotificationTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            NotificationWindow.IsOpen = true;
        }

        private void Notification_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            if(NetworkInterface.GetIsNetworkAvailable()) {
                switch(action) {
                    case NotificationAction.TryLoadingOlder:
                        try {
                            GetOlderComics();
                        } catch(WebException) {
                            action = NotificationAction.TryLoadingOlder;
                            ShowNotification(CannotLoadOlderMessage, 4000);
                        }
                        break;
                    case NotificationAction.TryLoadingNewer:
                        try {
                            GetNewerComics();
                        } catch(WebException) {
                            action = NotificationAction.TryLoadingNewer;
                            ShowNotification(CannotLoadNewerMessage, 4000);
                        }
                        break;
                    default:
                        break;
                }
            } else {
                ShowNotification(NoNetworkMessage, 4000);
            }
        }

        private void NotificationBorder_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
            if(e.IsInertial) {
                if(GestureHelper.GetDirection(e.FinalVelocities.LinearVelocity.X, e.FinalVelocities.LinearVelocity.Y) == GestureHelper.Direction.Right) {
                    NotificationWindow.IsOpen = false;
                    if(NotificationTimer.IsEnabled) NotificationTimer.Stop();
                    e.Handled = true;
                }
            }
        }

        private void NotificationWindow_WindowOpened(object sender, EventArgs e) {
            NotificationTimer.Start();
        }

        void NotificationTimer_Tick(object sender, EventArgs e) {
            NotificationTimer.Stop();
            NotificationWindow.IsOpen = false;
        }
        #endregion
    }
}