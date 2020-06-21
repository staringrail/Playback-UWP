using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Toolkit.Uwp.UI.Animations;

using Playback.Core.Models;
using Playback.Core.Services;
using Playback.Services;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Playback.Views
{
    public sealed partial class VideoLibraryDetailPage : Page, INotifyPropertyChanged
    {
        private SampleOrder _item;

        public SampleOrder Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public VideoLibraryDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RegisterElementForConnectedAnimation("animationKeyVideoLibrary", itemHero);
            if (e.Parameter is long orderID)
            {
                var data = await SampleDataService.GetContentGridDataAsync();
                Item = data.First(i => i.OrderID == orderID);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(Item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
