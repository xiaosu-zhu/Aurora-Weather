using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Com.Aurora.AuWeather.Models;
using Windows.Storage;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class ImmersiveSettingViewModel : ViewModelBase
    {
        private Immersive immersive;

        public ImmersiveGroup PivotList { get; set; }
        public CurrentImmersiveList CurrentList { get; set; }

        public ImmersiveBackgroundState SunnyState
        {
            get
            {
                return sunnyState;
            }

            set
            {
                sunnyState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Sunny = value;
                SetProperty(ref sunnyState, value);
            }
        }
        public ImmersiveBackgroundState StarryState
        {
            get
            {
                return starryState;
            }

            set
            {
                starryState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Starry = value;
                SetProperty(ref starryState, value);
            }
        }
        public ImmersiveBackgroundState CloudyState
        {
            get
            {
                return cloudyState;
            }

            set
            {
                cloudyState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Cloudy = value;
                SetProperty(ref cloudyState, value);
            }
        }
        public ImmersiveBackgroundState RainnyState
        {
            get
            {
                return rainnyState;
            }

            set
            {
                rainnyState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Rainny = value;
                SetProperty(ref rainnyState, value);
            }
        }
        public ImmersiveBackgroundState SnowyState
        {
            get
            {
                return snowyState;
            }

            set
            {
                snowyState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Snowy = value;
                SetProperty(ref snowyState, value);
            }
        }
        public ImmersiveBackgroundState FoggyState
        {
            get
            {
                return foggyState;
            }

            set
            {
                foggyState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Foggy = value;
                SetProperty(ref foggyState, value);
            }
        }
        public ImmersiveBackgroundState HazeState
        {
            get
            {
                return hazeState;
            }

            set
            {
                hazeState = ImmersiveBackgroundState.Fallback;
                if (value == ImmersiveBackgroundState.Fallback)
                {
                    value = ImmersiveBackgroundState.Assets;
                }
                immersive.Haze = value;
                SetProperty(ref hazeState, value);
            }
        }

        public int sunnyP;
        public int starryP;
        public int cloudyP;
        public int rainnyP;
        public int snowyP;
        public int foggyP;
        public int hazeP;

        private ImmersiveBackgroundState sunnyState;
        private ImmersiveBackgroundState starryState;
        private ImmersiveBackgroundState cloudyState;
        private ImmersiveBackgroundState rainnyState;
        private ImmersiveBackgroundState snowyState;
        private ImmersiveBackgroundState foggyState;
        private ImmersiveBackgroundState hazeState;
        public StorageFile localFile;

        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;
        public event EventHandler<FetchDataCompleteEventArgs> SetLocalComplete;

        public ImmersiveSettingViewModel()
        {
            var task = ThreadPool.RunAsync(async (work) =>
            {
                immersive = Immersive.Get();
                PivotList = new ImmersiveGroup();
                CurrentList = await CurrentImmersiveList.Get(PivotList[0].Title);
                var lUri = await Immersive.GetFileFromLocalAsync(PivotList[0].Title);
                await CheckandSetlocalFile(lUri);
                immersive.CheckLocal(PivotList[0].Title, lUri);
                rePick();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    SunnyState = immersive.Sunny;
                    StarryState = immersive.Starry;
                    CloudyState = immersive.Cloudy;
                    RainnyState = immersive.Rainny;
                    SnowyState = immersive.Snowy;
                    FoggyState = immersive.Foggy;
                    HazeState = immersive.Haze;
                }));
                OnFetchDataComplete();
            });
        }

        private void rePick()
        {
            sunnyP = immersive.SunnyPicked;
            starryP = immersive.StarryPicked;
            cloudyP = immersive.CloudyPicked;
            rainnyP = immersive.RainnyPicked;
            foggyP = immersive.FoggyPicked;
            snowyP = immersive.SnowyPicked;
            hazeP = immersive.HazePicked;
        }

        private void OnFetchDataComplete()
        {
            var h = FetchDataComplete;
            if (h != null)
            {
                h(this, new FetchDataCompleteEventArgs());
            }
        }

        internal void SaveAll()
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                immersive.Save();
            });
        }

        internal void ChoseLocal(string title, StorageFile file)
        {
            var task = ThreadPool.RunAsync(async (work) =>
            {
                var uri = await immersive.SaveLocalFile(title, file);
                SaveAll();
                await CheckandSetlocalFile(uri);
                OnSetLocalComplete(title);
            });
        }

        private void OnSetLocalComplete(string title)
        {
            var h = this.SetLocalComplete;
            if (h != null)
            {
                h(this, new FetchDataCompleteEventArgs(title));
            }
        }

        private async Task CheckandSetlocalFile(Uri uri)
        {
            if (uri == null)
            {
                localFile = null;
            }
            else
            {
                localFile = await FileIOHelper.GetFilebyUriAsync(uri);
            }
        }

        internal void ChangeCurrentList(int index)
        {
            var task = ThreadPool.RunAsync(async (work) =>
             {
                 CurrentList = null;
                 CurrentList = await CurrentImmersiveList.Get(PivotList[index].Title);
                 var lUri = await Immersive.GetFileFromLocalAsync(PivotList[index].Title);
                 await CheckandSetlocalFile(lUri);
                 immersive.CheckLocal(PivotList[index].Title, lUri);
                 SaveAll();
                 OnFetchDataComplete();
             });

        }

        internal void DeleteLocal(string title)
        {
            switch (title)
            {
                case "Sunny":
                    SunnyState = ImmersiveBackgroundState.Assets;
                    break;
                case "Starry":
                    StarryState = ImmersiveBackgroundState.Assets;
                    break;
                case "Cloudy":
                    CloudyState = ImmersiveBackgroundState.Assets;
                    break;
                case "Rainny":
                    RainnyState = ImmersiveBackgroundState.Assets;
                    break;
                case "Snowy":
                    SnowyState = ImmersiveBackgroundState.Assets;
                    break;
                case "Foggy":
                    FoggyState = ImmersiveBackgroundState.Assets;
                    break;
                case "Haze":
                    HazeState = ImmersiveBackgroundState.Assets;
                    break;
                default:
                    break;
            }
            SaveAll();
            localFile = null;
            OnFetchDataComplete();
        }

        internal void ChangeSelection(int rootSelect, int listSelect)
        {
            if (listSelect == -1)
                return;
            immersive.Pick(PivotList[rootSelect].Title, listSelect);
            SunnyState = immersive.Sunny;
            StarryState = immersive.Starry;
            CloudyState = immersive.Cloudy;
            RainnyState = immersive.Rainny;
            SnowyState = immersive.Snowy;
            FoggyState = immersive.Foggy;
            HazeState = immersive.Haze;
            SaveAll();
            rePick();
            OnFetchDataComplete();
        }
    }

    public class CurrentImmersiveList : ObservableCollection<BackgroundSelector>
    {
        internal static async Task<CurrentImmersiveList> Get(string title)
        {
            var uris = await Immersive.GetThumbnailsFromAssetsAsync(title);
            var self = new CurrentImmersiveList();
            foreach (var item in uris)
            {
                self.Add(new BackgroundSelector(item.Key, item.Value));
            }
            return self;
        }
    }

    public class BackgroundSelector
    {
        public BackgroundSelector(Uri path, string title)
        {
            Path = path;
            Title = "Photo by " + title;
        }

        public Uri Path { get; set; }
        public string Title { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }

    public class ImmersiveGroup : ObservableCollection<ImmersiveSelector>
    {
        public ImmersiveGroup()
        {
            Add(new ImmersiveSelector("Sunny"));
            Add(new ImmersiveSelector("Starry"));
            Add(new ImmersiveSelector("Cloudy"));
            Add(new ImmersiveSelector("Rainny"));
            Add(new ImmersiveSelector("Snowy"));
            Add(new ImmersiveSelector("Foggy"));
            Add(new ImmersiveSelector("Haze"));
        }
    }

    public class ImmersiveSelector
    {
        public ImmersiveSelector(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
    }
}
