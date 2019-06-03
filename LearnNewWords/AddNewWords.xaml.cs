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
                if (!concepts.Exists(x => x.Question.Equals(TextBoxQuestion.Text)))
                {
                    var nc = new Concept(TextBoxQuestion.Text, TextBoxAnswer.Text);
                    this.concepts.Add(nc);
                    this.handler.Add(nc);
                }
                else
                {
                    MiscFunctions.MessageBox("Question already exists", "Remove existing quesiton first. It would cause ambiguity otherwise");
                }

                RefreshExistingWordList();
            }
        }

        private void Add_new_word_Click(object sender, RoutedEventArgs e)
        {
            AddWord();
        }

        private void DeleteSelectedWord()
        {
            //MiscFunctions.MessageBox("", ((TextBlock)ListView_Concepts.SelectedItem).Text.ToString());
            if (ListView_Concepts.SelectedItem != null)
            {
                string question = ((TextBlock)ListView_Concepts.SelectedItem).Text;
                handler.Remove(question);
                this.concepts.RemoveAll(x => x.Question.Equals(question));
                RefreshExistingWordList();
            }
            else
            {
                MiscFunctions.MessageBox("Error", "No item selected");
            }
        }

        private void RefreshExistingWordList()
        {
            
            var noLongerExist = new List<TextBlock>();
            foreach (TextBlock block in ListView_Concepts.Items)
            {
                if (!concepts.Exists(x=>x.Question.Equals(block.Text)))
                {
                    noLongerExist.Add(block);
                }
            }
            foreach (var item in noLongerExist)
            {
                ListView_Concepts.Items.Remove(item);
            }

            var conceptsToAdd = new List<Concept>();
            foreach (var concept in concepts)
            {
                if (!ListView_Concepts.Items.ToList().Exists(x =>((TextBlock)x).Text.Equals(concept.Question)))
                {
                    conceptsToAdd.Add(concept);
                }
            }
            foreach (var concept in conceptsToAdd)
            {
                ListView_Concepts.Items.Add(new TextBlock() { Text = concept.Question });
            }

            
            /*ListView_Concepts.Items.Clear();
            foreach (var w in this.concepts)
            {
                ListView_Concepts.Items.Add(new TextBlock() {Text= w.Question });
            }*/
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter: AddWord(); break;
                case Windows.System.VirtualKey.Escape: BackAction(); break;
                case Windows.System.VirtualKey.Delete: DeleteSelectedWord(); break;
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
        
        private void Button_DeleteConcept_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedWord();
        }

        private void Button_ShowAnswer_Click(object sender, RoutedEventArgs e)
        {
            string question = ((TextBlock)ListView_Concepts.SelectedItem).Text;
            var concept = concepts.Find(x => x.Question.Equals(question));
            string answers = "";
            foreach (string asnwer in concept.Answers)
            {
                answers += asnwer + '\n';
            }
            MiscFunctions.MessageBox(concept.Question, answers);
        }

        private void ListView_Concepts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem == null)
            {
                Button_ShowAnswer.IsEnabled = false;
                Button_DeleteConcept.IsEnabled = false;
            }
            else
            {
                Button_ShowAnswer.IsEnabled = true;
                Button_DeleteConcept.IsEnabled = true;

            }
        }
    }
}