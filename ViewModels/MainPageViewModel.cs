using bookquery.Models;
using bookquery.Services;
using bookquery.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace bookquery.ViewModels
{
    /// <summary>
    /// Könyvkereső felületet kezelő nézetmodell
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        /// <value>Könyvkeresési eredmények listája</value>
        public ObservableCollection<BookHeader> bookHeaders { get; set; } =
            new ObservableCollection<BookHeader>();

        /// <value>Érték, ami felett figyelembe vesszük a keresési paramétert</value>
        private readonly int minSearchLength = BookSearchService.minSearchChar;
        /// <value>Keresési segítő szöveg a minimum hosszról</value>
        public string SearchHelpText { get; }

        private string _searchTitle = "";
        /// <value>Keresés könyv címére</value>
        public string SearchTitle
        {
            get { return _searchTitle; }
            set
            {
                Set(ref _searchTitle, value);
                QueryCommand.RaiseCanExecuteChanged();
            }
        }
        private string _searchAuthor = "";
        /// <value>Keresés könyv szerzőjére</value>
        public string SearchAuthor
        {
            get { return _searchAuthor; }
            set
            {
                Set(ref _searchAuthor, value);
                QueryCommand.RaiseCanExecuteChanged();
            }
        }
        private string _searchGenre = "";
        /// <value>Keresés könyv műfajára</value>
        public string SearchGenre
        {
            get { return _searchGenre; }
            set
            {
                Set(ref _searchGenre, value);
                QueryCommand.RaiseCanExecuteChanged();
            }
        }
        /// <value>Flag, hogy elindult már egy keresés</value>
        private bool requestSent = false;

        /// <value>Kiválasztott könyv indexe</value>
        private int selectedIndex = -1;

        /// <value>Keresést indító parancs</value>
        public DelegateCommand QueryCommand { get; }

        public MainPageViewModel()
        {
            QueryCommand = new DelegateCommand(searchBooksAsync, searchable);
            SearchHelpText = "At least " + BookSearchService.minSearchChar + " characters";
        }

        /// <summary>
        /// Ha a DetailPage-ről térünk vissza, indítsuk újra a keresést az előző paraméterekkel
        /// </summary>
        public override Task OnNavigatedToAsync(
            object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter == null)
            {
                var storeService = new StorageService();
                var resp = storeService.readSearchables();
                if(resp.Count == 3)
                {
                    SearchTitle = resp[0];
                    SearchAuthor = resp[1];
                    SearchGenre = resp[2];
                    if (searchable()) searchBooksAsync();
                }
                return Task.CompletedTask;
            }
            dynamic jsonObject = JsonConvert.DeserializeObject((string)parameter);
            if (jsonObject == null) return Task.CompletedTask;
            SearchTitle = (string)jsonObject.Title ?? "";
            SearchAuthor = (string)jsonObject.Author ?? "";
            SearchGenre = (string)jsonObject.Genre ?? "";

            if (searchable()) searchBooksAsync();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Keresési lekérdezés elküldése és eredmény beolvasása
        /// </summary>
        private async void searchBooksAsync()
        {
            //flag beállítása, hogy elkezdtük a lekérdezést
            requestSent = true;
            QueryCommand.RaiseCanExecuteChanged();
            try
            {
                //Lekérdezés a 3 paraméterrel
                var service = new BookSearchService();
                var newHeaders = await service.GetBookGroupAsync(SearchTitle, SearchAuthor, SearchGenre);

                //Eredmény beolvasása
                bookHeaders.Clear();
                foreach (var item in newHeaders)
                {
                    bookHeaders.Add(item);
                }
                selectedIndex = -1;

                //Nem elég hosszú paraméterek törlődjenek is
                if (SearchTitle.Length < minSearchLength) SearchTitle = "";
                if (SearchAuthor.Length < minSearchLength) SearchAuthor = "";
                if (SearchGenre.Length < minSearchLength) SearchGenre = "";

                var storeService = new StorageService();
                storeService.writeSearchables(SearchTitle + ";" + SearchAuthor + ";" + SearchGenre);
            }
            catch
            {
                bookHeaders.Clear();
            }
            //Kész vagyunk, flag visszaállítása
            requestSent = false;
            QueryCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Keresés indításának predikátuma, kell egy paraméter ami legalább a minimum hosszot eléri
        /// </summary>
        private bool searchable()
        {
            if ((SearchTitle.Length >= minSearchLength || SearchAuthor.Length >= minSearchLength || 
                 SearchGenre.Length >= minSearchLength) && !requestSent)
                return true;
            return false;
        }

        /// <summary>
        /// Könyv fejléc kijelölésének érzékelése és indexének eltárolása
        /// </summary>
        /// <param name="header">Kiválasztott fejléc</param>
        public void BookHeader_Clicked(BookHeader header)
        {
            ChangeClicked(selectedIndex, false);
            selectedIndex = bookHeaders.IndexOf(header);
            ChangeClicked(selectedIndex, true);
        }

        /// <summary>
        /// Adott indexen a fejléc kiválasztottságának beállítása
        /// </summary>
        /// <param name="index">Könyv indexe a listán</param>
        /// <param name="clicked">Beállítandó érték</param>
        private void ChangeClicked(int index, bool clicked)
        {
            if (index >= 0 && index < bookHeaders.Count)
            {
                bookHeaders[index].isClicked = clicked;
            }
        }

        /// <summary>
        /// Kiválasztott könyv részletes adatainak megjelenítése új felületen
        /// </summary>
        public void Navigate_to_Details()
        {
            if (selectedIndex >= 0 && selectedIndex < bookHeaders.Count)
            {
                var selectedItem = bookHeaders[selectedIndex];

                if (selectedItem.ISBN.Length > 0)
                {
                    //Paraméterek JSON-be csomagolása
                    //A választott könyvet egy kiadásának ISBN kódja alapján azonosítjuk be
                    string json = JsonConvert.SerializeObject(new
                    {
                        ISBN = selectedItem.ISBN[0],
                        Title = SearchTitle,
                        Author = SearchAuthor,
                        Genre = SearchGenre
                    });
                    NavigationService.NavigateAsync(typeof(DetailPage), json);
                }
            }
        }
    }
}

