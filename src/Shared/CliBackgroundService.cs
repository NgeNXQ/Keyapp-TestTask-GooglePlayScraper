using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Controllers;

namespace Keyapp.TestTask.GooglePlayScraper.Shared;

internal sealed class CliBackgroundService : BackgroundService {

    private readonly CliController _cliController;
    private readonly IHostApplicationLifetime _lifetime;

    public CliBackgroundService(
        CliController cliController,
        IHostApplicationLifetime lifetime
    ) {
        _lifetime = lifetime;
        _cliController = cliController;
    }

    protected override async Task ExecuteAsync(CancellationToken token) {
        try {
            await _cliController.WorkAsync(token);
        }
        catch (OperationCanceledException) {
            Console.WriteLine("Cancelled");
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
        finally {
            _lifetime.StopApplication();
        }
    }
}
