using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elements;

public class Element
{
    // Constants for default values
    private const string DEFAULT_CLASSIFICATION = "Non-Metal";
    private const string DEFAULT_LOCATION = "s-block";
    private const string DEFAULT_STATE = "Gas";
    private const int DEFAULT_PERIOD = 1;

    // Symbol
    public string Symbol { get; set; } = "";

    // Number
    [JsonPropertyName("Atomic Number")]
    public int Number { get; set; } = 1;

    // Name
    public string Name { get; set; } = "";

    // Mass
    public string Mass { get; set; } = null;

    // Group
    public int? Group { get; set; } = null;

    // Period
    public int Period { get; set; } = DEFAULT_PERIOD;

    // Element Classfication
    public string Classification { get; set; } = DEFAULT_CLASSIFICATION;

    // Location
    public string Location { get; set; } = DEFAULT_LOCATION;

    // Electron Shell Configuration
    [JsonPropertyName("Electron shell configuration")]
    public string ShellConfig { get; set; } = "";

    // Electron Subshell Configuration
    [JsonPropertyName("Electron subshell configuration")]
    public string SubshellConfig { get; set; } = "";

    // Ionisation Energy
    [JsonPropertyName("Ionisation energy")]
    public string Ionisation { get; set; } = "";

    // State at Room Temperature
    [JsonPropertyName("State at Room Temperature")]
    public string State { get; set; } = DEFAULT_STATE;

    // Boiling Point
    [JsonPropertyName("Boiling Point")]
    public string BoilingPoint { get; set; } = "";

    // Melting Point
    [JsonPropertyName("Melting Point")]
    public string MeltingPoint { get; set; } = "";

    // Isotopes
    public string Isotopes { get; set; } = "";

    // Discovery
    [JsonPropertyName("Discovered")]
    public string Discovery { get; set; } = "";

    // Element Description
    [JsonPropertyName("Element Description")]
    public string Description { get; set; } = "";

    // Generate JSON for Saving
    public string GenerateSaveJson()
    {
        return JsonSerializer.Serialize(this, options: new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

        }).Replace("\r\n", "\n") + "\n";
    }

    // Return Save FileName
    public string ReturnPartialFileName()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Element_{0:000}.json", Number);
    }

    // Reset Element
    public void Reset()
    {
        Symbol = "";
        Mass = null;
        ShellConfig = "";
        SubshellConfig = "";
        Ionisation = "";
        BoilingPoint = "";
        MeltingPoint = "";
        Discovery = "";
        Isotopes = "";
        Description = "";
        Classification = DEFAULT_CLASSIFICATION;
        Group = null;
        Period = DEFAULT_PERIOD;
        Location = DEFAULT_LOCATION;
        State = DEFAULT_STATE;
    }
}
