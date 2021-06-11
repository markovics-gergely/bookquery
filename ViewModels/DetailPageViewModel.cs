using bookquery.Models;
using bookquery.Services;
using bookquery.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace bookquery.ViewModels
{
    /// <summary>
    /// Választott könyv részletes adatait kezelő nézetmodell
    /// </summary>
    public class DetailPageViewModel : ViewModelBase
    {
        private string _searchTitle = "";
        public string SearchTitle
        {
            get { return _searchTitle; }
            set
            {
                Set(ref _searchTitle, value);
            }
        }
        private string _searchAuthor = "";
        public string SearchAuthor
        {
            get { return _searchAuthor; }
            set
            {
                Set(ref _searchAuthor, value);
            }
        }
        private string _searchGenre = "";
        public string SearchGenre
        {
            get { return _searchGenre; }
            set
            {
                Set(ref _searchGenre, value);
            }
        }

        private Book _book = new Book();
        /// <value>Választott Könyv adatai</value>
        public Book Book
        {
            get { return _book; }
            set
            {
                Set(ref _book, value);
            }
        }

        /// <summary>
        /// Érkezéskor könyv részletes adatainak betöltése
        /// </summary>
        public override async Task OnNavigatedToAsync(
            object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject((string)parameter);
            SearchTitle = (string)jsonObject.Title;
            SearchAuthor = (string)jsonObject.Author;
            SearchGenre = (string)jsonObject.Genre;

            var bookService = new BookSearchService();
            
            var isbn = (string)jsonObject.ISBN;
            Book = await bookService.GetBookAsync(isbn);

            //Kapott nyelv rövidítések alapján nyelvek teljes nevének betöltése
            var lang = await bookService.GetFullLanguagesAsync(Book.Language);
            Book.FullLanguages = lang;
        }

        /// <summary>
        /// Visszalépés a keresőfelületre, és keresés ugyanezekkel a paraméterekkel
        /// </summary>
        public void Navigate_to_Main()
        {
            string json = JsonConvert.SerializeObject(new
            {
                Title = SearchTitle,
                Author = SearchAuthor,
                Genre = SearchGenre
            });
            NavigationService.NavigateAsync(typeof(MainPage), json);
        }

        /// <summary>
        /// Szerző honlapjának megnyitása
        /// </summary>
        public async void openWebsiteAsync()
        {
            if(Book.AuthorSite.Length > 0)
            {
                string uriToLaunch = Book.AuthorSite;
                var uri = new Uri(uriToLaunch);

                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        /// <summary>
        /// Könyv borítóképének lementése Jpeg fájlba
        /// </summary>
        /// <param name="bookCover">Borítókép képe</param>
        public async void downloadImage(Image bookCover)
        {
            string filename = Book.Authors.Replace(" ", "_").Replace(",", "") + "-" +
                              Book.Title.Replace(" ", "_").Replace(",", "");
            var service = new StorageService();
            await service.downloadImageAsync(bookCover, filename);
        }
    }
}

