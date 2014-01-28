using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using Xkcd;

namespace UpdateTileCountAgent {
    public class ScheduledAgent : ScheduledTaskAgent {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent() {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            if(Debugger.IsAttached) {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task) {
            if(NetworkInterface.GetIsNetworkAvailable()) {
                try {
                    var comic = await XkcdInterface.GetCurrentComic();
                    var tileData = new FlipTileData {
                       BackgroundImage = new Uri(comic.ImageUri),
                       WideBackgroundImage = new Uri(comic.ImageUri)
                    };
                    ShellTile tile = ShellTile.ActiveTiles.First();
                    if(tile != null) {
                        tile.Update(tileData);
                    }
                } catch(WebException) { } catch(InvalidOperationException) { }
            }
            //NotifyComplete();
        }
    }
}