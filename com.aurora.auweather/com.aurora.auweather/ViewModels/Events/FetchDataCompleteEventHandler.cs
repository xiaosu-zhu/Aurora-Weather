// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Windows.ApplicationModel.Resources;

namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class FetchDataCompleteEventArgs
    {
        public object Parameter { get; private set; }

        public FetchDataCompleteEventArgs()
        {

        }

        public FetchDataCompleteEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }

    public class FetchDataFailedEventArgs
    {
        public string Message { get; private set; }

        public FetchDataFailedEventArgs(string message)
        {
            var loader = new ResourceLoader();
            try
            {
                var s = loader.GetString(message);
                if (s != null && s != "")
                    this.Message = s;
            }
            catch (System.Exception)
            {
                this.Message = message;
            }
        }
    }
}