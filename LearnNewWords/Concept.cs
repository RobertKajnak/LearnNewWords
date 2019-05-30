using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnNewWords
{
    class Concept
    {
        private readonly string question;
        private readonly List<string> answers;
        public bool caseSensitive = false;

        public List<string> Answers => answers;
        public string Question => question;

        public Concept(string question, IEnumerable<string> answers)
        {
            this.question = question;

            this.answers = new List<string>();
            foreach (var s in answers)
            {
                this.answers.Add(s);
            }
        }

        public Concept(string question, string answer)
        {
            this.question = question;
            this.answers = new List<string>
            {
                answer
            };
        }


        public bool CheckAnswer(string answer)
        {
            if (!this.caseSensitive)
                answer = answer.ToUpper();
            foreach (string ca in this.Answers)
            {
                if (!this.caseSensitive)
                {
                    if (ca.ToUpper().Equals(answer))
                        return true;
                }
                else
                {
                    if (ca.Equals(answer))
                        return true;
                }
            }
            return false;
        }

    }
}
