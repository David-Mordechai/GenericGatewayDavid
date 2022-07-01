using System.Text.Json;
using AeroCodeGenProtocols;
using Demo.Core.Interfaces.Outgoing;
using Demo.Core.Models;

namespace Demo.Infrastructure.Outgoing.Processors;

internal class Telemetry2JsonProcessor : IOutgoingProcessor
{
    public object Process(object obj)
    {
        if (obj is not GcsLightsRep gcsLightsRep) return "";

        var payload = new
        {
            gcsLightsRep.IsNavLightsOn,
            gcsLightsRep.IsStrobLightsOn
        };

        var message = new Message("GcsLightsRep", JsonSerializer.Serialize(payload));
        return message;
    }
}