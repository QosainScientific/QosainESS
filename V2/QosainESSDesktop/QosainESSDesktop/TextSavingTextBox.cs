using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QosainESSDesktop
{
    public class TextSavingTextBox : TextBox
    {
        public TextSavingTextBox()
        {
            TextChanged += TextSavingTextBox_TextChanged;
            ParentChanged += TextSavingTextBox_ParentChanged;
        }

        private void TextSavingTextBox_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                try
                {
                    Text = getText(Name);
                }
                catch { }
            }
        }

        bool created = false;
        private string getText(string name)
        {
            try
            {
                if (name == "")
                    return "";
                if (!File.Exists("textBoxTexts.txt"))
                    File.WriteAllText("textBoxTexts.txt", "");
                var pairs = File.ReadAllLines("textBoxTexts.txt")
                    .Select(line => line.Split(new char[] { '=' }, 2).Select(part => part.Replace("{equal}", "=").Replace("{bsr}", "\r").Replace("{bsn}", "\n")).ToArray()
                    ).ToList();
                if (pairs.Find(pair => pair[0] == name) != null)
                    return pairs.Find(pair => pair[0] == name)[1];
            }
            catch { }
            return "";
        }

        public void SaveText()
        {
            if (!created)
                return;
            saveText(Name, Text);
        }

        private void saveText(string name, string text)
        {
            try
            {
                if (!File.Exists("textBoxTexts.txt"))
                    File.WriteAllText("textBoxTexts.txt", "");
                var pairs = File.ReadAllLines("textBoxTexts.txt").Select(line => line.Split(new char[] { '=' })).ToList();
                if (pairs.Find(pair => pair[0] == name) != null)
                    pairs.Remove(pairs.Find(pair => pair[0] == name));
                pairs.Add(new string[] { name, text });
                File.WriteAllLines("textBoxTexts.txt", pairs.Select(
                    pair => string.Join("=", pair.Select(part => part.Replace("=", "{equal}").Replace("\r", "{bsr}").Replace("\n", "{bsn}")).ToArray())
                    ));
            }
            catch { }
        }
        protected override void OnCreateControl()
        {
            Text = getText(Name);
            created = true;
        }

        private void TextSavingTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Parent == null)
                return;
            SaveText();
        }
    }
}
