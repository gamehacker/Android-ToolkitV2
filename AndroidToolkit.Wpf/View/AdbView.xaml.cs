﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using AndroidToolkit.Wpf.Presentation.Converters;
using AndroidToolkit.Wpf.Presentation.Presenter;
using AndroidToolkit.Wpf.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using FileDialog = AndroidToolkit.Infrastructure.Helpers.FileDialog;
using TextBox = System.Windows.Controls.TextBox;

namespace AndroidToolkit.Wpf.View
{
    /// <summary>
    /// Interaction logic for AdbView.xaml
    /// </summary>
    public partial class AdbView : MetroWindow
    {
        private readonly AdbViewModel _viewModel;

        public AdbView()
        {
            InitializeComponent();
            Logo.HeaderSubtitle.Text = "ADB";
            _viewModel = new AdbViewModel();
            this.DataContext = _viewModel;
            AddEvents();
            _FlyoutPresenter = Presentation.Presenter.FlyoutPresenter.Present;
        }

        #region AddEvent
        private void AddEvents()
        {
            this.Closing += delegate
            {
                _viewModel.Cleanup();
                GC.Collect();
            };

            this.Deactivated += delegate
            {
                GC.Collect();
            };

            this.RebootButton.Click += ButtonClickHandler;
            this.RebootRecoveryButton.Click += ButtonClickHandler;
            this.RebootBootloaderButton.Click += ButtonClickHandler;
            this.PushButton.Click += ButtonClickHandler;
            this.PullButton.Click += ButtonClickHandler;
            this.InstallButton.Click += ButtonClickHandler;
            this.UninstallButton.Click += ButtonClickHandler;
            this.RefreshApps.Click += ButtonClickHandler;

            this.ShowDevices.Click += (sender, args) => _FlyoutPresenter.Invoke(this, 3);
            this.ShowSettings.Click += (sender, args) => _FlyoutPresenter.Invoke(this, 0);

            this.ShowReboot.Click += (sender, args) => _FlyoutPresenter.Invoke(this, 1);

            this.LightTheme.Checked += (sender, args) =>
            {
                ThemeManager.ChangeAppTheme(this, "BaseLight");
                this.Right.Background = Brushes.White;
                this.RightTop.Background = Brushes.White;
                this.LightTheme.IsEnabled = false;
            };
            this.LightTheme.Unchecked += (sender, args) =>
            {
                ThemeManager.ChangeAppTheme(this, "BaseDark");
                this.Right.Background = (SolidColorBrush)this.Resources["FlyoutBackgroundBrush"];
                this.RightTop.Background = (SolidColorBrush)this.Resources["FlyoutBackgroundBrush"];
            };
        }

        private async void ButtonClickHandler(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                _FlyoutPresenter.Invoke(this, 4);
                using (Toast toast = new Toast("Working in background..."))
                {
                    toast.Show();
                }
                var timer = new System.Timers.Timer(2048);
                timer.Elapsed += async (s, args) =>
                {
                    await this.Dispatcher.InvokeAsync(() => StatusLabel.Content = "Working in background...");
                    await this.Dispatcher.InvokeAsync(() => _FlyoutPresenter.Invoke(this, 4));
                    timer.Dispose();
                };
                timer.Enabled = true;

            });
        }
        #endregion

        private readonly FlyoutPresenter _FlyoutPresenter;
        private delegate void FlyoutPresenter(MetroWindow context, int index);
    }
}
