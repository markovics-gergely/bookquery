
using bookquery.Models;
using bookquery.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace bookquery.Views
{
    /// <summary>
    /// Fő felület, ahol könyveket lehet keresni
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BookHeader_Clicked(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var newHeader = (BookHeader)e.AddedItems[0];
                ViewModel.BookHeader_Clicked(newHeader);
            }
        }
        private void Navigate_to_Details(object sender,RoutedEventArgs e)
        {
            ViewModel.Navigate_to_Details();
        }
    }
}
