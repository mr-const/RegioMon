using System;
using System.Net;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RegioMon.RegioJet;
using RegioMon.ViewModels;
using RegioMon.Views;

namespace RegioMon;

public partial class App : Application
{
    public IServiceProvider? Locator { get => _host?.Services; }
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var builder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
        builder.Logging.AddDebug();

        // we need to set the base path and re-set the appsettings.json file
        // otherwise single file application will not find the configuration file.
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        ConfigureServices(builder, builder.Services);

        _host = builder.Build();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();

        await _host.StartAsync();
    }

    private static void ConfigureServices(HostApplicationBuilder context, IServiceCollection collection)
    {
        collection.AddHostedService<Worker>();

        collection
            .AddTransient<MainWindowViewModel>()
            .AddTransient((prov) => new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            .AddSingleton<RegioJetApi>();
    }
}
