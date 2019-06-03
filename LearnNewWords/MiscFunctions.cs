using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LearnNewWords
{
    static class MiscFunctions
    {
        public static async void MessageBox(string title, string content, string closeButton="OK")
        {
            await new ContentDialog()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButton
            }.ShowAsync();
        }
    }
}
