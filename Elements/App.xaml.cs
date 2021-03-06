﻿using System;
using System.Windows;

[assembly: CLSCompliant(true)]
namespace Elements
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Exception Handler
        private void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.Message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        // Startup
        private void AppStartup(object sender, StartupEventArgs e)
        {
            // Add Exception Handler
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);

            // Launch Main
            Main main = new Elements.Main();
            main.Show();
        }
    }
}
