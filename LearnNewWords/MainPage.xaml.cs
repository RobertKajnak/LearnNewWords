using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
        private ConceptHandler handler;

        public MainPage()
        {
            this.InitializeComponent();

            if (handler == null)
                LockFile();
        }

        private async void LockFile()
        {
            Windows.Storage.StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync("default.cpt");
            }
            catch
            {
                file = await folder.CreateFileAsync(desiredName: "default.cpt");//, options: CreationCollisionOption.ReplaceExisting);
            }
            finally
            {
                this.handler = new ConceptHandler(file);
                await handler.ReadXML();
            }

            //this.handler = new ConceptHandler(Path.Combine(installedLocation.Path,"default.cpt"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Hyperlink_Folder.Content = ApplicationData.Current.LocalFolder.Path;
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Recap),this.handler);
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

            StorageFile file = await picker.PickSingleFileAsync();
            
            if (file != null)
            {
                this.handler = new ConceptHandler(file);
                await handler.ReadXML();
            }
            else
            {
                MiscFunctions.MessageBox("Dictionary selection cancelled", "Dictionary left unchanged");
            }
        }

        private void Add_Words_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddNewWords),this.handler);
        }

        private async void Hyperlink_Folder_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }
    }
}
