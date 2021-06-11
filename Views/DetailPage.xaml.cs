﻿
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace bookquery.Views
{
    /// <summary>
    /// Könyvek részletes adatait mutató felület
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            this.InitializeComponent();
        }

        private void OpenWebsite(object sender, RoutedEventArgs e)
        {
            ViewModel.openWebsiteAsync();
        }

        private void downloadImage(object sender, RoutedEventArgs e)
        {
            ViewModel.downloadImage(bookCover);
        }

        private void Navigate_to_Main(object sender, RoutedEventArgs e)
        {
            ViewModel.Navigate_to_Main();
        }
    }
}
