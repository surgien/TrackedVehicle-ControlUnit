﻿using ControlUnit.Controller.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ControlUnit.Controller.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ControllerPage : Page
    {
        public ControllerViewModel ViewModel { get; set; }

        public ControllerPage()
        {
            this.InitializeComponent();
            ViewModel = new ControllerViewModel(App.Container);
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var device = e.Parameter as BluetoothDevice;
            ViewModel.SelectedDevice = device;
        }

        private void Slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.LockedTurn)
            {
                ViewModel.ResetVelocity();
            }
        }
    }
}
