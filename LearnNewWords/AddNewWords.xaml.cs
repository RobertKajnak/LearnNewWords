using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LearnNewWords
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewWords : Page
    {
        private List<Concept> concepts;
        private ConceptHandler handler;
        public AddNewWords()
        {
            this.InitializeComponent();


            LockFile();
        }

        private async void LockFile()
        {
            
            //

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
                this.concepts = handler.GetAllConcepts();
                RefreshExistingWordList();
            }
            
            //this.handler = new ConceptHandler(Path.Combine(installedLocation.Path,"default.cpt"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);

        }

        private void AddWord()
        {
            if ((this.TextBoxAnswer.Text.Length < 1 || this.TextBoxQuestion.Text.Length < 1)
                || (this.TextBoxAnswer.Text.Equals("Answer") || this.TextBoxQuestion.Text.Equals("Question")))
            {
                MiscFunctions.MessageBox("Field Left Empty", "Both Question and Answer need to be filled in");
            }
            else
            {
                var nc = new Concept(TextBoxQuestion.Text, TextBoxAnswer.Text);
                this.concepts.Add(nc);
                this.handler.Add(nc);
                RefreshExistingWordList();
            }
        }

        private void Add_new_word_Click(object sender, RoutedEventArgs e)
        {
            AddWord();
        }

        private void RefreshExistingWordList()
        {
            ListView_Concepts.Items.Clear();
            foreach (var w in this.concepts)
            {
                ListView_Concepts.Items.Add(new TextBlock() {Text= w.Question });
            }
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter: AddWord(); break;
                case Windows.System.VirtualKey.Escape: BackAction(); break;
                default: break;
            }
        }

        private void BackAction()
        {

            this.handler.SaveChanges();
            this.Frame.GoBack();
            /*System.Threading.Tasks.Task.Run(() => {
                try
                {
                }
                catch (Exception ex)
                {
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => {
                        MiscFunctions.MessageBox("Unexpected Error Occured", ex.Message);
                    });
                }

            });*/
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.BackAction();   
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}