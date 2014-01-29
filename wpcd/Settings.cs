using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using Xkcd;

namespace wpcd {
    public class Settings : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private IsolatedStorageSettings settings;
        public Settings() {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public Comic SelectedComic {
            get {
                if(!settings.Contains("SelectedComic")) {
                    settings["SelectedComic"] = new ObservableCollection<Comic>();
                }
                return (Comic)settings["SelectedComic"];
            }

            set {
                settings["SelectedComic"] = value;
                OnPropertyChanged("SelectedComic");
            }
        }

        public ObservableCollection<Comic> ComicList {
            get {
                if(!settings.Contains("ComicList")) {
                    settings["ComicList"] = new ObservableCollection<Comic>();
                }
                return (ObservableCollection<Comic>)settings["ComicList"];
            }

            set {
                settings["ComicList"] = value;
                OnPropertyChanged("ComicList");
            }
        }

        public bool AppIsRunning {
            get {
                if(!settings.Contains("AppIsRunning")) {
                    settings["AppIsRunning"] = false;
                }
                return (bool)settings["AppIsRunning"];
            }

            set {
                settings["AppIsRunning"] = value;
                OnPropertyChanged("AppIsRunning");
            }
        }

        public void OnPropertyChanged(string propertyName) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Save() {
            settings.Save();
        }

    }
}
