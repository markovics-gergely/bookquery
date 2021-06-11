using bookquery.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bookquery.Services
{
    /// <summary>
    /// Könyvek lekérdezésének hálózati kommunikációját megvalósító osztály
    /// </summary>
    class BookSearchService
    {
        /// <value>Érték, ami felett figyelembe vesszük a keresési paramétert</value>
        public static readonly int minSearchChar = 3;

        /// <value>Használt Szerver címe</value>
        private readonly Uri serverUrl = new Uri("https://openlibrary.org");
        /// <value>Út az általános kereséshez</value>
        private readonly string searchPath = "search.json?";
        /// <value>Út szerző lekérdezéséhez</value>
        private readonly string authorPath = "authors/";
        /// <value>Út nyelv lekérdezéséhez</value>
        private readonly string languagePath = "languages/";

        /// <value>Cím, ami alatt a borítóképek találhatóak</value>
        private readonly string coverBaseURL = "http://covers.openlibrary.org/b/olid/";
        /// <value>Kis méretű kép kiterjesztése</value>
        private readonly string coverSmallExtension = "-S.jpg";
        /// <value>Nagy méretű kép kiterjesztése</value>
        private readonly string coverLargeExtension = "-L.jpg";

        /// <value>Cím, ami alatt a kis méretű default kép található</value>
        private readonly string coverSmallPlaceHolderURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/64px-No-Image-Placeholder.svg.png";
        /// <value>Cím, ami alatt a nagy méretű default kép található</value>
        private readonly string coverLargePlaceHolderURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/256px-No-Image-Placeholder.svg.png";

        /// <value>Alcím default értéke</value>
        private readonly string defaultSubTitle = "No Subtitle";

        /// <summary>
        /// Lekérdezés küldése megadott honlapra, és kicsomagolása megadott osztályú objektumba
        /// </summary>
        /// <param name="uri">Lekérdezés célpontja</param>
        /// <returns>
        /// Lekérdezés eredménye típusosan
        /// </returns>
        private async Task<T> GetAsync<T>(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        /// <summary>
        /// Könyvek fejlécei listájának lekérdezése legfeljebb 3 paraméter alapján
        /// </summary>
        /// <param name="title">Keresett könyv címének része</param>
        /// <param name="author">Keresett könyv szerzője nevének része</param>
        /// <param name="genre">Keresett könyv témájának része</param>
        /// <returns>
        /// Könyvek fejleinek listája
        /// </returns>
        public async Task<ObservableCollection<BookHeader>> GetBookGroupAsync(string title, string author, string genre)
        {
            var fullURL = serverUrl + searchPath;

            //Paraméterek megadása UriBuilder segítségével (elvégzi a kódolást is)
            var uriBuilder = new UriBuilder(fullURL);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            //Csak az alsó korlát feletti paramétert vegye figyelembe
            if (title.Length >= minSearchChar) query["title"] = title;
            if (author.Length >= minSearchChar) query["author"] = author;
            if (genre.Length >= minSearchChar) query["subject"] = genre;

            uriBuilder.Query = query.ToString();
            //URL kiegészítése a query értékekkel
            fullURL = uriBuilder.ToString();
            
            //lekérdezés aszinkron bevárása a json konvencióihoz igazított osztályba
            var newHeaderList = await GetAsync<BookHeaderGroup>(new Uri(fullURL));
            //Cél listánk kiolvasása a lekérdezésből
            var headerCollection = newHeaderList.Docs;

            for (int i = 0; i < headerCollection.Count; i++)
            {
                //Ha van elérési uta a képnek, betöltjük kis méretben
                if (headerCollection[i].Cover_edition_key != null && 
                    headerCollection[i].Cover_edition_key.Length > 0)
                {
                    headerCollection[i].ImageURL = coverBaseURL + headerCollection[i].Cover_edition_key + coverSmallExtension;
                } 
                //Különben default kép jelenik meg kis méretben
                else headerCollection[i].ImageURL = coverSmallPlaceHolderURL;

                //Ha nincs alcím, default értéke legyen
                if (headerCollection[i].Subtitle != null && 
                    headerCollection[i].Subtitle.Length == 0)
                {
                    headerCollection[i].Subtitle = defaultSubTitle;
                }
            }
            return headerCollection;
        }

        /// <summary>
        /// Adott könyv részletes adatainak lekérdezése ISBN kulcs alapján
        /// </summary>
        /// <param name="isbn">Keresett könyv ISBN kulcsa</param>
        /// <returns>
        /// Részletes könyv adatok
        /// </returns>
        public async Task<Book> GetBookAsync(string isbn)
        {
            if (isbn.Length == 0) return null;

            var fullURL = serverUrl + searchPath;
            //ISBN Paraméter megadása UriBuilder segítségével (elvégzi a kódolást is)
            var uriBuilder = new UriBuilder(fullURL);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["isbn"] = isbn;

            uriBuilder.Query = query.ToString();
            //URL kiegészítése a query értékkel
            fullURL = uriBuilder.ToString();

            var newHeaderList = await GetAsync<BookGroup>(new Uri(fullURL));
            //Max 1 könyvnek kell egyeznie, különben nincs találat
            var book = newHeaderList.Docs.Count > 0 ? newHeaderList.Docs[0] : new Book();

            //Ha van elérési uta a képnek, betöltjük nagy méretben
            if (book.Cover_edition_key.Length > 0)
            {
                book.ImageURL = coverBaseURL + book.Cover_edition_key + coverLargeExtension;
            }
            //Különben default kép jelenik meg nagy méretben
            else book.ImageURL = coverLargePlaceHolderURL;

            //Ha nincs alcím, default értéke legyen
            if (book.Subtitle.Length == 0)
            {
                book.Subtitle = "No Subtitle";
            }

            //Ha van a szerzőnek azonosítója, lekérdezzük van-e weboldala
            if(book.Author_key.Count > 0)
            {
                var authorSite = await GetAuthorSiteAsync(book.Author_key[0]);
                book.AuthorSite = authorSite;
            }
            return book;
        }

        /// <summary>
        /// Nyelvek teljes neveinek lekérdezése rövidítések listájából
        /// </summary>
        /// <param name="acr">Nyelv rövidítések listája</param>
        /// <returns>
        /// Nyelvek nevének listája
        /// </returns>
        public async Task<List<string>> GetFullLanguagesAsync(List<string> acr)
        {
            var langArray = new List<string>();
            foreach(string lang in acr)
            {
                var fullLang = await GetAsync<Language>(new Uri(serverUrl + languagePath + lang + ".json"));
                //Ha talált nyelvet a rövidítéssel, adja hozzá
                if (fullLang.Name.Length > 0 && fullLang.Code.Length > 0)
                {
                    langArray.Add(fullLang.Name + " (" + fullLang.Code + ")");
                }
                //Különben maradjon a rövidítés
                else langArray.Add(lang);
            }
            return langArray;
        }

        /// <summary>
        /// Adott szerző honlapja címének lekérdezése
        /// </summary>
        /// <param name="Author_Key">Szerző azonosítója</param>
        /// <returns>
        /// Szerző honlapja, vagy üres string ha nincs
        /// </returns>
        private async Task<string> GetAuthorSiteAsync(string Author_Key)
        {
            if (Author_Key.Length == 0) return "";

            var fullURL = serverUrl + authorPath + Author_Key + ".json";

            var author = await GetAsync<Author>(new Uri(fullURL));
            var url = "";
            //Ha talált weboldalt visszaadjuk azt, különben üres
            if(author.Links.Count > 0)
            {
                url = author.Links[0].URL;
            }
            return url;
        }
    }
}
