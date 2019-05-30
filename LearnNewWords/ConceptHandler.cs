using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LearnNewWords
{
    class ConceptHandler
    {
        private readonly string filename;
        private readonly XElement words;

        public ConceptHandler(string filename)
        {
            this.filename = filename;
            try
            {
                 words = XElement.Load(filename);
            }
            catch(Exception ex)
            {
                words = new XElement("Words");
            }
        }

        public void Add(Concept concept)
        {
            words.Add(new XElement(concept.Question, concept.Answers));
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

        public void SaveChanges()
        {
            using (FileStream DestinationStream = File.Create(this.filename))
            {
                this.words.Save(DestinationStream);
            }
        }
    }
}
