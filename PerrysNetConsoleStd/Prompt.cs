using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class Prompt
    {

        public static string DEFAULT_PREFIX = "Please enter";
        public static string DEFAULT_ERRORMESSAGE = "No valid input";

        public bool AllowEmpty { get; set; }
        public string Default { get; set; }
        public bool SingleCharacter { get; set; }
        public string Prefix { get; set; }
        public Dictionary<string, string> ChoicesText { get; set; }
        public bool ValidateChoices { get; set; }
        public Regex ValidationRegex { get; set; }
        public string ErrorMessage { get; set; }
        
        public string[] Choices
        {
            get
            {
                return this.ChoicesText != null && this.ChoicesText.Count > 0 ? this.ChoicesText.Select(v => v.Key).ToArray() : null;
            }
            set
            {
                this.ChoicesText = new Dictionary<string, string>();
                if (value != null)
                {
                    value.ToList().ForEach(v => this.ChoicesText.Add(v, null));
                }
            }
        }

        public Prompt()
        {
            this.AllowEmpty = false;
            this.SingleCharacter = false;
            this.Prefix = DEFAULT_PREFIX;
            this.ValidateChoices = false;
            this.ValidationRegex = null;
            this.ErrorMessage = DEFAULT_ERRORMESSAGE;
        }

        protected virtual void ValidateInput(string input, ref bool valid)
        {
            if (this.SingleCharacter)
            {
                CoEx.WriteLine();
            }

            if (this.AllowEmpty == false || !string.IsNullOrEmpty(input))
            {
                if (valid != false && this.ValidationRegex != null)
                {
                    valid = this.ValidationRegex.IsMatch(input);
                }

                if (valid != false && this.ValidateChoices && this.Choices != null && this.Choices.Length > 0)
                {
                    valid = this.Choices.Contains(input);
                }
            }

            if (valid == false && !string.IsNullOrWhiteSpace(this.ErrorMessage))
            {
                CoEx.WriteLine(this.ErrorMessage);
            }
        }

        public virtual string DoPrompt()
        {
            bool valid = true;
            string temp = null;

            do
            {
                valid = true;
                temp = null;

                if (this.ChoicesText != null && this.ChoicesText.Select(v => v.Value).Any(v => v != null))
                {
                    int maxlen = this.ChoicesText.Select(v => v.Key.Length).Max();
                    foreach (var choice in this.ChoicesText)
                    {
                        CoEx.WriteLine("[{0}] {1}", choice.Key.PadRight(maxlen, ' '), choice.Value ?? "Unknown");
                    }
                    CoEx.WriteLine();
                }

                if (!string.IsNullOrWhiteSpace(this.Prefix))
                {
                    CoEx.Write(this.Prefix);
                }

                if (this.Choices != null && this.Choices.Length > 0)
                {
                    CoEx.Write(" (" + string.Join("/", this.Choices) + ")");
                }

                if (!string.IsNullOrEmpty(this.Default))
                {
                    CoEx.Write(" [" + this.Default + "]");
                }

                CoEx.Write(": ");

                temp = this.SingleCharacter ? CoEx.ReadKeyChar() : CoEx.ReadLine();

                if (!string.IsNullOrEmpty(this.Default) && string.IsNullOrEmpty(temp))
                {
                    temp = this.Default;
                }

                this.ValidateInput(temp, ref valid);
            }
            while (valid == false);

            return temp;
        }

    }
}
