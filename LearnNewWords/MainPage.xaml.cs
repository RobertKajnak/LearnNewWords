using System;
using System.Collections.Generic;
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

namespace LearnNewWords
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Recap));
        }

        private void Quit()
        {
            Application.Current.Exit();
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Escape: Quit(); break;
                default: break;
            }
        }

        private async void Change_dict_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

            picker.FileTypeFilter.Add(".dict");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            
            if (file != null)
            {
                var handler = new ConceptHandler(file.Name);
            }
            else
            {
                ShowNoFileSelected();
            }
        }

        private async void ShowNoFileSelected()
        {
            ContentDialog NoFileSelected = new ContentDialog()
            {
                Title = "Dictionary selection cancelled",
                Content = "Dictionary left unchanged",
                CloseButtonText = "Ok"
            };

            await NoFileSelected.ShowAsync();
        }

        private void Add_Words_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddNewWords));
        }
    }
}
