using AeroCodeGenProtocols;
using Avro;
using Demo.Core.Interfaces;
using Demo.Core.Models;

namespace Demo.Infrastructure.Processors;

internal class Telemetry2AvroProcessor : IProcessor
{
    public object Process(object obj)
    {
        if (obj is not GcsLightsRep gcsLightsRep) return "";

        var payload = new GcsLightsAvro
        {
            IsNavLightsOn = gcsLightsRep.IsNavLightsOn,
            IsStrobLightsOn = gcsLightsRep.IsStrobLightsOn
        };

        var message = new MessageDto("GcsLightsAvro", payload);
        return message;
    }
}