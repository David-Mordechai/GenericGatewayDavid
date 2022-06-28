using AeroCodeGenProtocols;
using Demo.Core.Interfaces;

namespace Demo.Infrastructure.Importers;

internal class TelemetryImporter : IImporter
{
    public void Start(CancellationToken cancellationToken)
    {
        // Simulates UAV telemetry report
        _ = Task.Factory.StartNew(() =>
        {
            var random = new Random();

            while (cancellationToken.IsCancellationRequested is false)
            {
                var value = random.Next(0, 10000) % 2 == 0;
                var gcsLightsRep = new GcsLightsRep(value, value);

                DataReady?.Invoke(this, gcsLightsRep);

                Task.Delay(1000, cancellationToken);
            }
        }, cancellationToken);
    }

    public event EventHandler<object>? DataReady;
}