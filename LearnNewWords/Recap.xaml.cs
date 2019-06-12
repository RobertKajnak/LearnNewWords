using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class Recap : Page
    {
        ConceptHandler handler;
        List<Concept> concepts;
        string WordOrder;
        Random random;
        Concept currentConcept;
        //TODO: Check roaming across devices
        ApplicationDataContainer roamingSettings;

        public Recap()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            random = new Random();
            roamingSettings = ApplicationData.Current.RoamingSettings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };

            string wordOrder = (string)roamingSettings.Values["WordOrder"];
            if (wordOrder == null)
            {
                roamingSettings.Values["WordOrder"] = "Intelligent";
                wordOrder = "Intelligent";
            }
            else
            {
                foreach (var control in Panel_WordOrder.Children)
                {
                    if (control is RadioButton && ((RadioButton)control).Tag.Equals(wordOrder))
                    {
                        ((RadioButton)control).IsChecked = true;
                    }
                }
            }
            this.WordOrder = wordOrder;

            if (e.Parameter is ConceptHandler)
            {
                this.handler = (ConceptHandler)e.Parameter;
                this.concepts = handler.GetAllConcepts();
            }
            else
            {
                MiscFunctions.MessageBox("Unexpected Error", "Invalid concept list. Returning to main menu");
                this.Frame.GoBack();
            }
            currentConcept = concepts[0];
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            Concept concept;
            switch (this.WordOrder)
            {
                case ("Intelligent"):
                    int minscore = concepts.Min(x => x.Score);
                    var worstKnow = concepts.FindAll(x => x.Score == minscore);
                    concept = worstKnow[random.Next(worstKnow.Count)];
                    break;
                case ("Random"):
                    concept = concepts[random.Next(concepts.Count)];
                    break;
                case ("Sequential"):
                    concept = concepts[concepts.IndexOf(currentConcept) + 1];
                    break;
                default:
                    concept = currentConcept;
                    MiscFunctions.MessageBox("OOps", "Invalid Word Order selected. Somehow.");
                    break;
            }

            TextBlock_Question.Text = concept.Question;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.BackAction();
        }

        private void BackAction()
        {
            this.Frame.GoBack();
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Escape: BackAction(); break;
                default: break;
            }
        }

        private Visibility BoolToVisibility(bool isVisible)
        {
            if (isVisible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        private void ToggleButtons(bool isAnswerChecked)
        {
            Button_CheckCorrect.IsEnabled = !isAnswerChecked;
            Button_CheckCorrect.Visibility = BoolToVisibility(!isAnswerChecked);
            
            Button_Next.IsEnabled = isAnswerChecked;
            Button_Next.Visibility = BoolToVisibility(isAnswerChecked);

            Button_ForceCorrect.IsEnabled = isAnswerChecked;
            Button_ForceCorrect.Visibility = BoolToVisibility(isAnswerChecked);
            Button_ForceIncorrect.IsEnabled = isAnswerChecked;
            Button_ForceIncorrect.Visibility = BoolToVisibility(isAnswerChecked);
        }

        private void Button_CheckCorrect_Click(object sender, RoutedEventArgs e)
        {
            foreach (var answer in currentConcept.Answers)
            {
                ListView_CorrectAnswers.Items.Add(new TextBlock() { Text = answer});
            }

            ToggleButtons(true);
        }

        private void Button_Force_Click(object sender, RoutedEventArgs e)
        {
            ListView_CorrectAnswers.Items.Clear();
            ToggleButtons(false);
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            ListView_CorrectAnswers.Items.Clear();
            ToggleButtons(false);
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            SplitView_Settings.IsPaneOpen = !SplitView_Settings.IsPaneOpen;
        }
        private void OnWordOrderChecked(object sender, RoutedEventArgs e)
        {
            string selectedOrder = ((RadioButton)sender)?.Tag?.ToString();

            if (selectedOrder != null)
            {
                roamingSettings.Values["WordOrder"] = selectedOrder;
                this.WordOrder = selectedOrder;
            }
        }
    }
}
