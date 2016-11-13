using System;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace Com.Aurora.AuWeather.Background
{
    public sealed class CortanaCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;
        SettingsModel settings = SettingsModel.Current;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated.
            this.serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails =
              taskInstance.TriggerDetails as AppServiceTriggerDetails;
            await FileIOHelper.AppendLogtoCacheAsync("AppServiceTriggered." + Environment.NewLine);
            if (triggerDetails != null && triggerDetails.Name == "AuroraWeatherCortanaCommandService")
            {
                await FileIOHelper.AppendLogtoCacheAsync("AppServiceHitted." + Environment.NewLine);
                try
                {
                    voiceServiceConnection =
                      VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                        triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted +=
                      VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await
                    voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "showWeatherinSomewhere":
                        case "showWeatherinSomewhereShort":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'showWeatherinSomwhere'." + Environment.NewLine);
                                var where =
                                  voiceCommand.Properties["where"][0];
                                CitySettingsModel ci;
                                if (settings.Cities.LocatedCity != null && settings.Cities.LocatedCity.City == where)
                                {
                                    ci = settings.Cities.LocatedCity;
                                }
                                else if (!settings.Cities.SavedCities.IsNullorEmpty())
                                {
                                    ci = Array.Find(settings.Cities.SavedCities, x =>
                                    {
                                        return x.City == where;
                                    });
                                }
                                else
                                {
                                    ci = null;
                                }
                                SendWeatherForecastIn(ci);
                                break;
                            }
                        case "showWeatherForecast":
                        case "showWeatherForecastShort":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'showWeatherForecast'." + Environment.NewLine);
                                var when =
                                  voiceCommand.Properties["when"][0];
                                SendWeatherForecastAt(when);
                                break;
                            }
                        case "showTemprature":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'showTemprature'." + Environment.NewLine);
                                var when = voiceCommand.Properties["when"][0];
                                SendTemperatureAt(when);
                                break;
                            }
                        case "showTempratureinSomeWhere":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'showTempratureinSomeWhere'." + Environment.NewLine);
                                var where = voiceCommand.Properties["where"][0];
                                CitySettingsModel ci;
                                if (settings.Cities.LocatedCity != null && settings.Cities.LocatedCity.City == where)
                                {
                                    ci = settings.Cities.LocatedCity;
                                }
                                else if (!settings.Cities.SavedCities.IsNullorEmpty())
                                {
                                    ci = Array.Find(settings.Cities.SavedCities, x =>
                                    {
                                        return x.City == where;
                                    });
                                }
                                else
                                {
                                    ci = null;
                                }
                                SendTemperatureIn(ci);
                                break;
                            }
                        case "askRain":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'askRain'." + Environment.NewLine);
                                var when = voiceCommand.Properties["when"][0];
                                SendRainAt(when);
                                break;
                            }
                        case "askRaininSomeWhere":
                            {
                                await FileIOHelper.AppendLogtoCacheAsync("Cortana Command 'askRaininSomeWhere'." + Environment.NewLine);
                                var where = voiceCommand.Properties["where"][0];
                                SendRainIn(where);
                                break;
                            }

                        // As a last resort, launch the app in the foreground.
                        default:
                            await FileIOHelper.AppendLogtoCacheAsync("Cortana Command not fit." + Environment.NewLine);
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch(Exception e)
                {
                    await FileIOHelper.AppendLogtoCacheAsync("Exception occured." + Environment.NewLine);
                    await FileIOHelper.AppendLogtoCacheAsync(e.ToString() + Environment.NewLine);
                }
                finally
                {
                    if (this.serviceDeferral != null)
                    {
                        // Complete the service deferral.
                        this.serviceDeferral.Complete();
                    }
                }
            }
        }

        private void SendRainIn(string when)
        {
            throw new NotImplementedException();
        }

        private void SendRainAt(string when)
        {
            throw new NotImplementedException();
        }

        private void SendTemperatureIn(CitySettingsModel where)
        {
            throw new NotImplementedException();
        }

        private void SendTemperatureAt(string when)
        {
            throw new NotImplementedException();
        }

        private void SendWeatherForecastAt(string when)
        {
            throw new NotImplementedException();
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            throw new NotImplementedException();
        }

        private void VoiceCommandCompleted(
          VoiceCommandServiceConnection sender,
          VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                // Insert your code here.
                // Complete the service deferral.
                this.serviceDeferral.Complete();
            }
        }

        private async void SendWeatherForecastIn(CitySettingsModel where)
        {
            // Take action and determine when the next trip to destination
            // Insert code here.

            // Replace the hardcoded strings used here with strings 
            // appropriate for your application.

            // First, create the VoiceCommandUserMessage with the strings 
            // that Cortana will show and speak.
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = where.City + "的天气状况：";
            userMessage.SpokenMessage = "这是" + where.City + "的天气状况";

            // Optionally, present visual information about the answer.
            // For this example, create a VoiceCommandContentTile with an 
            // icon and a string.
            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            var destinationTile = new VoiceCommandContentTile();
            destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWithText;
            // The user can tap on the visual content to launch the app. 
            // Pass in a launch argument to enable the app to deep link to a 
            // page relevant to the item displayed on the content tile.
            destinationTile.AppLaunchArgument = where.Id;
            destinationTile.Title = where.City;
            destinationTile.TextLine1 = "August 3rd 2015";
            destinationsContentTiles.Add(destinationTile);

            // Create the VoiceCommandResponse from the userMessage and list    
            // of content tiles.
            var response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);

            // Cortana will present a “Go to app_name” link that the user 
            // can tap to launch the app. 
            // Pass in a launch to enable the app to deep link to a page 
            // relevant to the voice command.
            response.AppLaunchArgument = where.Id;

            // Ask Cortana to display the user message and content tile and 
            // also speak the user message.
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "正在启动极光天气";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            // When launching the app in the foreground, pass an app 
            // specific launch parameter to indicate what page to show.
            response.AppLaunchArgument = "LaunchFromCortanaCommand";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

    }
}
