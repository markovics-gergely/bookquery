using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace bookquery.Models
{
    /// <summary>
    /// Segédosztály a könyvek fejlécei lekérdezésének fogadására
    /// </summary>
    public class BookHeaderGroup
    {
        public ObservableCollection<BookHeader> Docs { get; set; } =
            new ObservableCollection<BookHeader>();
    }

    /// <summary>
    /// Könyvek részleges adatait tartalmazó osztály
    /// </summary>
    public class BookHeader : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Subtitle { get; set; } = "";
        public string[] ISBN { get; set; } = new string[0];
        public List<string> Author_name { get; set; } = new List<string>();
        public string Authors { 
            get
            {
                return string.Join(", ", Author_name);
            }
        }
        public string Cover_edition_key { get; set; } = "";

        private string _imageURL = "";
        public string ImageURL
        {
            get
            {
                return _imageURL;
            }
            set
            {
                _imageURL = value;
                ImageSource = new BitmapImage(new Uri(_imageURL));
            }
        }
        public ImageSource ImageSource { get; set; }

        private bool _isClicked = false;
        /// <value>Megmondja melyik a kiválasztott elem a listáról</value>
        public bool isClicked { 
            get { return _isClicked; } 
            set
            {
                if (ISBN.Length > 0)
                    _isClicked = value;
                else _isClicked = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isClicked)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
