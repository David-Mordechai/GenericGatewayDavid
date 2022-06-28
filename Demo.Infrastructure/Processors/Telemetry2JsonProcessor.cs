using System.Text.Json;
using AeroCodeGenProtocols;
using Demo.Core.Interfaces;
using Demo.Core.Models;

namespace Demo.Infrastructure.Processors;

internal class Telemetry2JsonProcessor : IProcessor
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