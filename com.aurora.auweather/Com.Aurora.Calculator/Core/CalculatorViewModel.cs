using System;
using Com.Aurora.Shared.MVVM;

namespace Com.Aurora.Calculator.Core
{
    internal class CalculatorViewModel : ViewModelBase
    {
        private double val;
        public double Value
        {
            get
            {
                return val;
            }
            set
            {
                SetProperty(ref val, value);
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        internal void ChangeValue(string text)
        {
            if (text == "" || text == string.Empty)
            {
                Value = 0d;
                return;
            }
            var d = double.Parse(text);
            Value = d;
        }
    }
}