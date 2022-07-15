using AeroCodeGenProtocols;
using Avro;
using Demo.Core.Interfaces;

namespace Demo.Infrastructure.Processors;

internal class Avro2TelemetryProcessor : IProcessor
{
    public object Process(object obj)
    {
        if (obj is not GcsLightsAvro gcsLightsAvro) return "";

        var gcsLightsRep = new GcsLightsRep(gcsLightsAvro.IsNavLightsOn, gcsLightsAvro.IsStrobLightsOn);

        return gcsLightsRep;
    }
}