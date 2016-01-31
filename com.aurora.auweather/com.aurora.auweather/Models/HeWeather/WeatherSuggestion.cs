namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class WeatherSuggestion
    {
        private Suggestion comfortable;
        private Suggestion carWashing;
        private Suggestion dressing;
        private Suggestion flu;
        private Suggestion sport;

        internal Suggestion Comfortable
        {
            get
            {
                return comfortable;
            }

            set
            {
                comfortable = value;
            }
        }

        internal Suggestion CarWashing
        {
            get
            {
                return carWashing;
            }

            set
            {
                carWashing = value;
            }
        }

        internal Suggestion Dressing
        {
            get
            {
                return dressing;
            }

            set
            {
                dressing = value;
            }
        }

        internal Suggestion Flu
        {
            get
            {
                return flu;
            }

            set
            {
                flu = value;
            }
        }

        internal Suggestion Sport
        {
            get
            {
                return sport;
            }

            set
            {
                sport = value;
            }
        }

        public WeatherSuggestion(JsonContract.WeatherSuggestionContract suggestion)
        {
            Comfortable = new Suggestion(suggestion.comf);
            CarWashing = new Suggestion(suggestion.cw);
            Dressing = new Suggestion(suggestion.drsg);
            Flu = new Suggestion(suggestion.flu);
            Sport = new Suggestion(suggestion.sport);
        }
    }

    internal class Suggestion
    {
        private string brief;
        private string text;

        public Suggestion(JsonContract.SuggestionContract suggestion)
        {
            Brief = suggestion.brf;
            Text = suggestion.txt;
        }

        public string Brief
        {
            get
            {
                return brief;
            }

            set
            {
                brief = value;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }
    }
}