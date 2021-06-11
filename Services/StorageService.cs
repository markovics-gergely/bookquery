using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bookquery.Services
{
    /// <summary>
    /// Helyi fájlokkal való kommunikációt megvalósító osztály
    /// </summary>
    class StorageService
    {
        /// <value>Érték jelezni, hogy ez az első hívás</value>
        private static bool firstRun = true;

        /// <value>Fájl neve, ahova az értékeket rakjuk</value>
        private readonly string storeFile = "SearchableStore.txt";

        /// <summary>
        /// Értékek kiolvasása a fájlból, ha ez az első olvasás
        /// </summary>
        /// <returns>
        /// Értékek listája, amik a fájlban ";" jellel voltak elválasztva
        /// </returns>
        public List<string> readSearchables()
        {
            List<string> response = new List<string>();

            if (!firstRun) return response;
            firstRun = false;

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (isoStore.FileExists(storeFile))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(storeFile, FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        Debug.WriteLine("Reading " + storeFile  +" contents");
                        string resp = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");
                        string[] splitted = resp.Split(';');
                        response.AddRange(splitted);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Értékek lementése helyi fájlba
        /// </summary>
        /// <param name="toWrite">Lementendő érték</param>
        public void writeSearchables(string toWrite)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            using(IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(storeFile, FileMode.Create, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(toWrite);
                    Debug.WriteLine("You have written to the " + storeFile + " file.");
                }
            }
        }

        /// <summary>
        /// Kép lementésének indítása és lementési hely választása
        /// </summary>
        /// <param name="bookCover">Lementendő kép</param>
        /// <param name="filename">Lementendő kép alapértelmezett neve</param>
        public async Task downloadImageAsync(Image bookCover, string filename)
        {
            //Borító Bitmapbe rakása
            var coverBitMap = new RenderTargetBitmap();
            await coverBitMap.RenderAsync(bookCover);

            //Mentsük alapértelmezetten a Letöltésekbe
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            fileSavePicker.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            // Alapértelmezett neve legyen a fájlnak a könyv szerzője és címe
            fileSavePicker.SuggestedFileName = filename;

            var outputFile = await fileSavePicker.PickSaveFileAsync();
            //Ha félbehagytuk a lementést ne csináljon semmit
            if (outputFile != null)
            {
                var pixels = await coverBitMap.GetPixelsAsync();
                using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    //Kép lementése Jpeg-ként
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    byte[] bytes = pixels.ToArray();
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                            BitmapAlphaMode.Ignore,
                                            (uint)coverBitMap.PixelWidth,
                                            (uint)coverBitMap.PixelHeight,
                                            200,
                                            200,
                                            bytes);
                    await encoder.FlushAsync();
                }
            }
            else Debug.WriteLine(fileSavePicker.SuggestedFileName + " Save Cancelled");
        }
    }
}
