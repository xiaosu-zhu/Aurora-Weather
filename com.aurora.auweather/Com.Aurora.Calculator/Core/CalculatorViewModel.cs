// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

    internal class ProgramViewModel : ViewModelBase
    {
        private uint parameter;
        public uint Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
        private string val;
        public string Value
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
                Value = "0";
                return;
            }
            Value = text;
        }

        public ProgramViewModel(string t, uint p)
        {
            Title = t;
            Parameter = p;
        }
    }
}