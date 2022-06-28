namespace AeroCodeGenProtocols;

public class GcsLightsRep
{
    public GcsLightsRep(bool isNavLightsOn, bool isStrobLightsOn)
    {
        IsNavLightsOn = isNavLightsOn;
        IsStrobLightsOn = isStrobLightsOn;
    }

    public bool IsNavLightsOn { get; set; }
    public bool IsStrobLightsOn { get; set; }
}