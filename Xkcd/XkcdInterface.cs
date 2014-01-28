using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Xkcd {
    public class XkcdInterface {
        private const string xkcd = "http://xkcd.com";
        private const string info = "/info.0.json";
        public async static Task<Comic> GetCurrentComic() {
            return JsonConvert.DeserializeObject(await FetchJson(0), typeof(Comic)) as Comic;
        }

        public async static Task<int> NumberOfNewerComics(int lastNumber) {
            var comic = await GetCurrentComic();
            return comic.Number - lastNumber;
        }

        public async static Task<Comic> GetComic(int comicNumber) {
            return JsonConvert.DeserializeObject(await FetchJson(comicNumber), typeof(Comic)) as Comic;
        }

        public async static Task<List<Comic>> GetComics(int from, int to) {
            var cc = await GetCurrentComic();
            var returnList = new List<Comic>();
            if(from <= to) {
                for(int i = from; i < to + 1; i++) {
                    if(i < 1 || i > cc.Number || i == 404) continue;
                    returnList.Add(await GetComic(i));
                }
            } else {
                throw new ArgumentOutOfRangeException();
            }
            return returnList;
        }

        private async static Task<string> FetchJson(int comicNumber) {
            string json = "";
            var client = new WebClient();
            if(comicNumber == 0) {
                json = await client.DownloadStringTaskAsync(new Uri(xkcd + info));
            } else {
                json = await client.DownloadStringTaskAsync(new Uri(xkcd + "/" + comicNumber.ToString() + info));
            }
            return json;
        }
    }

    public class Comic : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _title;
        private int _number;
        private string _link;
        private string _safeTitle;
        private string _day;
        private string _month;
        private string _year;
        private string _news;
        private string _transcript;
        private string _alt;
        private string _imageUri;
        private bool _unread;
        private bool _favorite;

        public Comic() {
            _unread = true;
            _favorite = false;
        }

        [JsonProperty("title")]
        public string Title {
            get {
                return _title;
            }

            set {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        [JsonProperty("num")]
        public int Number {
            get {
                return _number;
            }

            set {
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        [JsonProperty("link")]
        public string Link {
            get {
                return _link;
            }

            set {
                _link = value;
                OnPropertyChanged("Link");
            }
        }

        [JsonProperty("safetitle")]
        public string SafeTitle {
            get {
                return _safeTitle;
            }

            set {
                _safeTitle = value;
                OnPropertyChanged("SafeTitle");
            }
        }

        [JsonProperty("day")]
        public string Day {
            get {
                return _day;
            }

            set {
                _day = value;
                OnPropertyChanged("Day");
                OnPropertyChanged("Date");
            }
        }

        [JsonProperty("month")]
        public string Month {
            get {
                return _month;
            }

            set {
                _month = value;
                OnPropertyChanged("Month");
                OnPropertyChanged("Date");
            }
        }

        [JsonProperty("year")]
        public string Year {
            get {
                return _year;
            }

            set {
                _year = value;
                OnPropertyChanged("Year");
                OnPropertyChanged("Date");
            }
        }

        public string Date {
            get {
                return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day)).ToString("d");
            }
        }

        [JsonProperty("news")]
        public string News {
            get {
                return _news;
            }

            set {
                _news = value;
                OnPropertyChanged("News");
            }
        }

        [JsonProperty("transcript")]
        public string Transcript {
            get {
                return _transcript;
            }

            set {
                _transcript = value;
                OnPropertyChanged("Transcript");
            }
        }

        [JsonProperty("alt")]
        public string Alt {
            get {
                return _alt;
            }

            set {
                _alt = value;
                OnPropertyChanged("Alt");
            }
        }

        [JsonProperty("img")]
        public string ImageUri {
            get {
                return _imageUri;
            }

            set {
                _imageUri = value;
                OnPropertyChanged("ImageUri");
            }
        }

        public ImageSource Image {
            get {
                var bi = new BitmapImage();
                var ImageLoaded = false;
                try {
                    using(var isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                        if(isoStore.FileExists(Number.ToString() + ".jpg")) {
                            using(var isoStoreFs = new IsolatedStorageFileStream(Number.ToString() + ".jpg", System.IO.FileMode.Open, isoStore)) {
                                bi.SetSource(isoStoreFs);
                                ImageLoaded = true;
                            }
                        } else {
                            DownloadImage(ImageUri);
                            ImageLoaded = true;
                        }
                    }
                } catch(Exception) { }
                if(!ImageLoaded) {
                    bi.UriSource = new Uri(ImageUri);
                }
                return bi;
            }
        }

        [JsonProperty("unread")]
        public bool Unread {
            get {
                return _unread;
            }

            set {
                _unread = value;
                OnPropertyChanged("Unread");
            }
        }

        [JsonProperty("favorite")]
        public bool Favorite {
            get {
                return _favorite;
            }

            set {
                _favorite = value;
                OnPropertyChanged("Favorite");
            }
        }

        private void OnPropertyChanged(string changedProperty) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(changedProperty));
            }
        }

        private async void DownloadImage(string uri) {
            try {
                var client = new WebClient();
                var bi = new BitmapImage();
                using(var result = await client.OpenReadTaskAsync(uri)) {
                    bi.SetSource(result);
                }
                var wb = new WriteableBitmap(bi);
                using(var isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                    if(!isoStore.FileExists(Number.ToString() + ".jpg")) {
                        using(var isoStoreFs = new IsolatedStorageFileStream(Number.ToString() + ".jpg", System.IO.FileMode.Create, isoStore)) {
                            wb.SaveJpeg(isoStoreFs, wb.PixelWidth, wb.PixelHeight, 0, 100);
                        }
                    }
                }
                OnPropertyChanged("Image");
            } catch(Exception) { }

        }
    }
}
