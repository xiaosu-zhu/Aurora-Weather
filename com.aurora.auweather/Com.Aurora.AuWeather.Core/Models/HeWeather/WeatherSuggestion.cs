// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class WeatherSuggestion
    {

        public Suggestion Comfortable
        {
            get; private set;
        }

        public Suggestion CarWashing
        {
            get; private set;
        }

        public Suggestion Dressing
        {
            get; private set;
        }

        public Suggestion Flu
        {
            get; private set;
        }

        public Suggestion Sport
        {
            get; private set;
        }
        public Suggestion UV
        {
            get; private set;
        }
        public Suggestion Trav;

        public WeatherSuggestion(JsonContract.WeatherSuggestionContract suggestion)
        {
            if (suggestion == null)
            {
                return;
            }
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