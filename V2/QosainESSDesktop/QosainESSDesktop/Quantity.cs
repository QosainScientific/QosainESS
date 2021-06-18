using FivePointNine.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QosainESSDesktop
{
    public class Quantity
    {
        public Quantity(double value, IUnit currentUnit, bool isStandard = true) :this(value.ToString(), currentUnit, isStandard)
        { }
        public string As(IUnit unit)
        {
            return unit.F(StandardValue.ToString());
        }
        public Quantity(string value, IUnit currentUnit, bool isStandard = true)
        {
            try { double.Parse(value); } catch { value = "0"; }
            if (!isStandard)
                StandardValue = double.Parse(currentUnit.F_(value.ToString()));
            else
                StandardValue = double.Parse(value);
            CurrentUnit = currentUnit;
        }
        public double StandardValue = 0;
        public string ScaledValue { get { return double.Parse(CurrentUnit.F(StandardValue.ToString())).ToString("0.000").TrimEnd(new char[] { '0' }).TrimEnd(new char[] { '.' }); } }
        public IUnit CurrentUnit { get; set; } = null;
    }
    public class UnitChanger : Button
    {
        public UnitChanger()
        { 
        }
        public TextBox TargetControl { get; set; }
        Quantity Value;
        IUnit [] Units;
        public void Initialize(ValueSavingQuantityBox target, params IUnit[] units)
        {
            Value = target.Value;
            Units = units;
            Click += UnitChanger_Click;
            TargetControl = target;
            TargetControl.TextChanged += TargetControl_TextChanged;
            Text = Value.CurrentUnit.Suffix;
        }

        private void TargetControl_TextChanged(object sender, EventArgs e)
        {
            try { double.Parse(TargetControl.Text); } catch { return; }
            Value.StandardValue = double.Parse(Value.CurrentUnit.F_(TargetControl.Text));
        }

        private void UnitChanger_Click(object sender, EventArgs e2)
        {
            Value.CurrentUnit = Units[(Units.ToList().FindIndex(u => u.Suffix == Value.CurrentUnit.Suffix) + 1) % Units.Length];
            TargetControl.TextChanged -= TargetControl_TextChanged;
            //TargetControl.Text = Value.ScaledValue + "0"; // force value change
            TargetControl.Text = Value.ScaledValue;
            TargetControl.TextChanged += TargetControl_TextChanged;
            Text = Value.CurrentUnit.Suffix;
        }
    }

    public class Units
    {
        public class none : IUnit
        {
            public string Suffix { get { return ""; } }

            public bool IsStandard => true;

            public string F(string v)
            {
                return v;
            }

            public string F_(string v)
            {
                return v;
            }
        }
        public class ml : IUnit
        {
            public string Suffix { get { return "ml"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e9).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e9).ToString();
            }
        }
        public class cc : IUnit
        {
            public string Suffix { get { return "cc"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e9).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e9).ToString();
            }
        }
        public class ul : IUnit
        {
            public string Suffix { get { return "µl"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e12).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e12).ToString();
            }
        }
        public class seconds : IUnit
        {
            public string Suffix { get { return "seconds"; } }

            public bool IsStandard => true;

            public string F(string v)
            {
                return v;
            }

            public string F_(string v)
            {
                return v;
            }
        }
        public class minutes : IUnit
        {
            public string Suffix { get { return "minutus"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) / 60).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) * 60).ToString();
            }
        }
        public class hours : IUnit
        {
            public string Suffix { get { return "hours"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) / 3600).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) * 3600).ToString();
            }
        }
        public class mmPerSecond : IUnit
        {
            public string Suffix { get { return "mm/s"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e6).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e6).ToString();
            }
        }
        public class mmPerMinute : IUnit
        {
            public string Suffix { get { return "mm/min"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e6 * 60).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e6 / 60).ToString();
            }
        }
        public class inchesPerSecond : IUnit
        {
            public string Suffix { get { return "in/s"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e6 / 25.4).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e6 * 25.4).ToString();
            }
        }
        public class inchesPerMinute : IUnit
        {
            public string Suffix { get { return "in/min"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e6 * 60 / 25.4).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e6 / 60 * 25.4).ToString();
            }
        }
        
        public class mlPerSecond : IUnit
        {
            public string Suffix { get { return "ml/s"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e9).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e9).ToString();
            }
        }
        public class mlPerMinute : IUnit
        {
            virtual public string Suffix { get { return "ml/min"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e9 * 60).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e9 / 60).ToString();
            }
        }
        public class ccPerMinutes : mlPerMinute
        {
            public override string Suffix => "cc/min";

            public bool IsStandard => false;
        }

        public class ulPerSecond : IUnit
        {
            public string Suffix { get { return "µl/s"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e12).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e12).ToString();
            }
        }
        public class ulPerMinutes : IUnit
        {
            public string Suffix { get { return "µl/min"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e12 * 60).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e12 / 60).ToString();
            }
        }
        public class mm : IUnit
        {
            public string Suffix { get { return "mm"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1000).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1000).ToString();
            }
        }
        public class cm : IUnit
        {
            public string Suffix { get { return "cm"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 100).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 100).ToString();
            }
        }
        public class Inch : IUnit
        {
            public string Suffix { get { return "inch"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 39.3701).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 39.3701).ToString();
            }
        }
        public class mills : IUnit
        {
            public string Suffix { get { return "mil"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 39.3701 * 1000).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 39.3701 / 1000).ToString();
            }
        }
        public class um : IUnit
        {
            public string Suffix { get { return "µm"; } }

            public bool IsStandard => false;

            public string F(string v)
            {
                return (double.Parse(v) * 1e6).ToString();
            }

            public string F_(string v)
            {
                return (double.Parse(v) / 1e6).ToString();
            }
        }
    }
    public interface IUnit
    {
        string F(string v);
        string F_(string v);
        string Suffix { get; }
        bool IsStandard { get; }
    }
}
