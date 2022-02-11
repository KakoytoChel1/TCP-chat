using Chat.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            MainViewModel mainView = new MainViewModel();

            this.DataContext = mainView;


            btnClose.Click += (s, e) => { this.Close();  };

            btnFullScreen.Click += (s, e) =>
            {
                this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            };
            btnMinimize.Click += (s, e) => { this.WindowState = WindowState.Minimized; };

            this.Closing += mainView.OnWindowClosing;

        }
    }
}
