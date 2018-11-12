using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class FilePrompt : Prompt
    {

        public static string DEFAULT_NOVALIDFILETYPEMSG = "Path is not a file";
        public static string DEFAULT_NOVALIDPATHMSG = "Not a valid path/filename";
        public static string DEFAULT_FILENOTFOUNDMSG = "Path/File does not exist";

        public bool AllowWildcard { get; set; }
        public enum FILETYPE { BOTH, FILE, DIR };
        public FILETYPE Filetype { get; set; }
        public string NoValidFiletypeMessage { get; set; }
        public bool ValidatePath { get; set; }
        public string NoValidPathMessage { get; set; }
        public bool ValidateFileExists { get; set; }
        public string FileNotFoundMessage { get; set; }

        public FilePrompt() : base()
        {
            this.Filetype = FILETYPE.FILE;
            this.NoValidFiletypeMessage = DEFAULT_NOVALIDFILETYPEMSG;
            this.ValidatePath = true;
            this.NoValidPathMessage = DEFAULT_NOVALIDPATHMSG;
            this.ValidateFileExists = false;
            this.FileNotFoundMessage = DEFAULT_FILENOTFOUNDMSG;
            this.AllowWildcard = false;
        }

        protected override void ValidateInput(string input, ref bool valid)
        {
            base.ValidateInput(input, ref valid);
            input = input.Trim().Trim(new char[] { '"', '\'' }).Trim();

            if (this.AllowEmpty == false || !string.IsNullOrEmpty(input))
            {
                if (this.ValidatePath && (this.AllowWildcard && input.Contains("*")) == false)
                {
                    try { new FileInfo(input); }
                    catch
                    {
                        valid = false;
                        CoEx.WriteLine(this.NoValidPathMessage);
                    }
                }

                if (valid != false && (this.AllowWildcard && input.Contains("*"))==false && 
                    this.ValidateFileExists && File.Exists(input) == false && Directory.Exists(input) == false)
                {
                    valid = false;
                    CoEx.WriteLine(this.FileNotFoundMessage);
                }

                if (valid != false && (this.AllowWildcard && input.Contains("*")) == false && 
                    this.Filetype != FILETYPE.BOTH && (File.Exists(input) || Directory.Exists(input)))
                {
                    FileAttributes attr = File.GetAttributes(input);
                    valid = (this.Filetype == FILETYPE.FILE && attr.HasFlag(FileAttributes.Directory) == false) ||
                        (this.Filetype == FILETYPE.DIR && attr.HasFlag(FileAttributes.Directory));

                    if (valid == false)
                    {
                        CoEx.WriteLine(this.NoValidFiletypeMessage);
                    }
                }
            }
        }

        public override string DoPrompt()
        {
            return base.DoPrompt().Trim().Trim(new char[] { '"', '\'' }).Trim();
        }

        public IEnumerable<string> DoPromptWildcard()
        {
            string temp = this.DoPrompt();

            if (this.AllowEmpty && string.IsNullOrEmpty(temp))
            {
                yield return "";
                yield break;
            }

            string path = Path.GetDirectoryName(temp);
            string file = Path.GetFileName(temp);

            if (Directory.Exists(path))
            {
                foreach (var searchfile in Directory.GetFiles(path, file, SearchOption.TopDirectoryOnly))
                {
                    yield return searchfile;
                }
            }
            yield break;
        }

    }
}
