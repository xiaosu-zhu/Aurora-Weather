namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class WeatherSuggestion
    {

        internal Suggestion Comfortable
        {
            get; private set;
        }

        internal Suggestion CarWashing
        {
            get; private set;
        }

        internal Suggestion Dressing
        {
            get; private set;
        }

        internal Suggestion Flu
        {
            get; private set;
        }

        internal Suggestion Sport
        {
            get; private set;
        }
        internal Suggestion UV
        {
            get; private set;
        }
        internal Suggestion Trav;

        public WeatherSuggestion(JsonContract.WeatherSuggestionContract suggestion)
        {
            Comfortable = new Suggestion(suggestion.comf);
            CarWashing = new Suggestion(suggestion.cw);
            Dressing = new Suggestion(suggestion.drsg);
            Flu = new Suggestion(suggestion.flu);
            Sport = new Suggestion(suggestion.sport);
            UV = new Suggestion(suggestion.uv);
            Trav = new Suggestion(suggestion.trav);
        }
    }

    public class Suggestion
    {
        public Suggestion(JsonContract.SuggestionContract suggestion)
        {
            Brief = suggestion.brf;
            Text = suggestion.txt;
        }

        public string Brief
        {
            get; private set;
        }

        public string Text
        {
            get; private set;
        }
    }
}