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
    public class ValueSavingQuantityBox : TextBox
    {
        public ValueSavingQuantityBox()
        {
            TextChanged += TextSavingTextBox_TextChanged;
            ParentChanged += TextSavingTextBox_ParentChanged;
        }
        public Quantity Value { get; set; }

        private void TextSavingTextBox_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                try
                {
                    getText(Name, Value);
                    Text = Value.ScaledValue;
                }
                catch { }
            }
        }

        bool created = false;
        private static void getText(string name, Quantity quantity)
        {
            try
            {
                if (name == "")
                    return;
                if (!File.Exists("textBoxTexts.txt"))
                    File.WriteAllText("textBoxTexts.txt", "");
                var pairs = File.ReadAllLines("textBoxTexts.txt")
                    .Select(line => line.Split(new char[] { '=' }, 2).Select(part => part.Replace("{equal}", "=").Replace("{bsr}", "\r").Replace("{bsn}", "\n")).ToArray()
                    ).ToList();
                string ans = "";
                if (pairs.Find(pair => pair[0] == name) != null)
                    ans = pairs.Find(pair => pair[0] == name)[1];
                string v = ans.Split(new char[] { ';' })[0];
                string unit = ans.Split(new char[] { ';' }, 2)[1];
                var allUnits = new IUnit[] { 
                    new Units.none(), 
                    new Units.cm(), new Units.Inch(), new Units.mm(), new Units.um(),
                    new Units.cc(), new Units.ml(), new Units.ul(),
                    new Units.ccPerMinutes(), new Units.mlPerMinute(), new Units.mlPerSecond(), new Units.ulPerMinutes(), new Units.ulPerSecond(),
                    new Units.hours(), new Units.mills(), new Units.minutes(), new Units.seconds(),
                    new Units.inchesPerMinute(), new Units.inchesPerSecond(), new Units.mmPerMinute(), new Units.mmPerSecond(), 
                    new Units.kelvin(), new Units.celsius(), new Units.fahrenheit(), 
                    new Units.radpersec(), new Units.revpermin(), new Units.revpersec(),
                };
                quantity.CurrentUnit = allUnits.ToList().Find(u => u.Suffix == unit);
                if (quantity.CurrentUnit.IsStandard)
                    quantity.StandardValue = double.Parse(v);
                else
                    quantity.StandardValue = double.Parse(quantity.CurrentUnit.F_(v));

            }
            catch { }
        }

        public void SaveText()
        {
            if (!created)
                return;
            saveText(Name, Value);
        }

        private static void saveText(string name, Quantity q)
        {
            try
            {
                string toSave = q.ScaledValue.ToString() + ";" + q.CurrentUnit.Suffix;
                if (!File.Exists("textBoxTexts.txt"))
                    File.WriteAllText("textBoxTexts.txt", "");
                var pairs = File.ReadAllLines("textBoxTexts.txt").Select(line => line.Split(new char[] { '=' })).ToList();
                if (pairs.Find(pair => pair[0] == name) != null)
                    pairs.Remove(pairs.Find(pair => pair[0] == name));
                pairs.Add(new string[] { name, toSave });
                File.WriteAllLines("textBoxTexts.txt", pairs.Select(
                    pair => string.Join("=", pair.Select(part => part.Replace("=", "{equal}").Replace("\r", "{bsr}").Replace("\n", "{bsn}")).ToArray())
                    ));
            }
            catch { }
        }
        public void NotifyControlCreated() { OnCreateControl(); }
        protected override void OnCreateControl()
        {
            if (Value != null)
                // already created
                return;
            Value = new Quantity(0, new Units.none());
            getText(Name, Value);
            if (Value.CurrentUnit == null)
                ;
            Text = Value.ScaledValue;
            created = true;
        }

        private void TextSavingTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Parent == null)
                return;

            if (Value != null)
            {
                try { double.Parse(Text); } catch { return; }
                Value.StandardValue = double.Parse(Value.CurrentUnit.F_(Text));
            }
            SaveText();
        }
    }
}
