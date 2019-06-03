using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace LearnNewWords
{
    class ConceptHandler
    {
        private readonly StorageFile file;
        private XElement words;

        /// <summary>
        /// ReadXML should be called separately
        /// </summary>
        /// <param name="file"></param>
        public ConceptHandler(StorageFile file)
        {
            this.file = file;
        }

        public async Task ReadXML()
        {
            try
            {
                using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    words = XElement.Load(WindowsRuntimeStreamExtensions.AsStreamForRead(stream));
                }
            }
            catch (Exception ex)
            {
                words = new XElement("Words");
                MiscFunctions.MessageBox("Error reading file", ex.Message);
                try
                {

                }
                catch (Exception createException)
                {
                    MiscFunctions.MessageBox("Could not create new file", createException.Message);
                }
            }
        }

        public void Add(Concept concept)
        {
            words.Add(new XElement("Concept", 
                new XElement("question", concept.Question),
                new XElement("anwer", concept.Answers))
                );
        }

        public void Remove(string question)
        {
            var nodes = words.Elements().Where(x => x.Element("question").Value.Equals(question)).ToList();

            foreach (var node in nodes)
                node.Remove();
        }

        public List<Concept> GetAllConcepts()
        {
            var concepts = new List<Concept>();
            foreach (var elem in words.Elements())
            {
                var answers = new List<string>();
                foreach  (var e in elem.Elements("answers"))
                {
                    answers.Add(e.Value);
                }
                concepts.Add(new Concept(elem.Element("question").Value, answers ));
            }
            return concepts;
        }

        public async void SaveChanges()
        {
            /*using (FileStream DestinationStream = File.Create(this.filename))
            {
                this.words.Save(DestinationStream);
            }*/

            //Stream stream = await file.OpenStreamForWriteAsync();
            await FileIO.WriteTextAsync(file, "");
            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                this.words.Save(WindowsRuntimeStreamExtensions.AsStreamForWrite(stream));
            }
            
        }
    }
}
