using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace bookquery.Models
{
    /// <summary>
    /// Segédosztály a könyv lekérdezésének fogadására
    /// </summary>
    public class BookGroup
    {
        public ObservableCollection<Book> Docs { get; set; }
    }

    /// <summary>
    /// Könyvek részletes adatait tartalmazó osztály
    /// </summary>
    public class Book : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Subtitle { get; set; } = "";
        public List<string> ISBN { get; set; } = new List<string>();

        public List<string> Author_name { get; set; } = new List<string>();
        public string Authors
        {
            get
            {
                return string.Join(", ", Author_name);
            }
        }
        public string AuthorSite { get; set; } = "";
        public List<string> Author_key { get; set; } = new List<string>();
        public string First_Author_key {
            get
            {
                if (Author_key.Count > 0)
                {
                    return Author_key[0];
                }
                return "";
            } 
        }

        public List<string> Author_Alternative_Name { get; set; } = new List<string>();
        public string Author_Names { 
            get
            {
                return string.Join("\n ", Author_Alternative_Name);
            }
        }

        public string Cover_edition_key { get; set; } = "";

        private string _imageURL = "";
        public string ImageURL {
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
        
        public List<string> Person { get; set; } = new List<string>();
        public List<string> Place { get; set; } = new List<string>();
        public List<string> Language { get; set; } = new List<string>();
        private List<string> _fullLanguages = new List<string>();
        public List<string> FullLanguages {
            get
            {
                return _fullLanguages;
            }
            set
            {
                _fullLanguages = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullLanguages)));
            }
        }
        public List<string> Subject { get; set; } = new List<string>();

        public List<string> Publish_Year { get; set; } = new List<string>();
        public List<string> Publish_Place { get; set; } = new List<string>();
        public List<string> Publisher { get; set; } = new List<string>();

        public List<string> Contributor { get; set; } = new List<string>();

        public string First_Sentences {
            get
            {
                return string.Join("\n ", First_Sentence);
            } 
        }
        public List<string> First_Sentence { get; set; } = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
