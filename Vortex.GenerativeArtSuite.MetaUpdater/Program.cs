using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;

Console.WriteLine("Paste json path");
var path = Console.ReadLine() ?? string.Empty;

var files = Directory.GetFiles(path);

Console.Clear();
Console.WriteLine($"Working on {files.Length} files in {path}");

Console.WriteLine("Enter new image link: (use {0} for id)");
var uri = Console.ReadLine() ?? string.Empty;

var People = new Dictionary<int, string>
{
    { 1, "Pontificus" },
    { 2, "Wabsilian" },
    { 3, "Halstone" },
    { 4, "Hoku" },
    { 5, "CK" },
    { 6, "6" },
    { 7, "Ashley DD" },
    { 8, "Devious" },
    { 9, "Megakent" },
};

var Warmachines = new Dictionary<int, string>
{
    { 412, "Arakkus" },
    { 1195, "Velorum" },
    { 2726, "Prosirus" },
    { 4286, "Doxxidus" },
    { 6119, "Aquila" },
    { 6388, "Cybeles" },
};

var Generals = new Dictionary<int, string>
{
    { 181, "Sirian" },
    { 644, "Atlantean" },
    { 2015, "Pleiadian" },
    { 2527, "Andromedan" },
    { 4173, "Acturian" },
    { 4523, "Orion" },
    { 5679, "Lightworker" },
    { 6060, "Maldek" },
};

foreach (var file in files)
{
    var data = JsonConvert.DeserializeObject<ERC721Metadata>(File.ReadAllText(file), new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Objects,
    });

    int id = int.Parse(Path.GetFileNameWithoutExtension(file));

    object result = new GenericMeta
    {
        Image = string.Format(uri, id),
        Name = data.Name,
        Attributes = data.Attributes,
        Id = id,
        Dna = data.Dna,
        Date = data.Date,
        Compiler = "Vortex Labs - GAS",
    };

    if (People.ContainsKey(id))
    {
        result = new RareMeta
        {
            Image = string.Format(uri, id),
            Name = People[id],
            Attributes = data.Attributes.Select(current =>
            {
                return new ERC721Trait
                {
                    LayerName = current.LayerName,
                    TraitName = current.LayerName == "Faction" ? "Unaffiliated" : "Unique",
                };
            }),
            Id = id,
            Compiler = "Vortex Labs - GAS",
        };
    }
    else if (Warmachines.ContainsKey(id))
    {
        result = new RareMeta
        {
            Image = string.Format(uri, id),
            Name = Warmachines[id],
            Attributes = data.Attributes.Select(current =>
            {
                return new ERC721Trait
                {
                    LayerName = current.LayerName,
                    TraitName = current.LayerName == "Faction" ? "Mythical Beast" : "Warmachine",
                };
            }),
            Id = id,
            Compiler = "Vortex Labs - GAS",
        };
    }
    else if (Generals.ContainsKey(id))
    {
        result = new RareMeta
        {
            Image = string.Format(uri, id),
            Name = $"{Generals[id]} General",
            Attributes = data.Attributes.Select(current =>
            {
                return new ERC721Trait
                {
                    LayerName = current.LayerName,
                    TraitName = current.LayerName == "Faction" ? Generals[id] : "General",
                };
            }),
            Id = id,
            Compiler = "Vortex Labs - GAS",
        };
    }

    File.WriteAllText(file, JsonConvert.SerializeObject(result));
}

Console.WriteLine("Done");

Console.ReadLine();

internal struct RareMeta
{
    /// <summary>
    /// This is the URL to the image of the item. Can be just about any type of image (including SVGs, which will be cached into PNGs by OpenSea), and can be IPFS URLs or paths. We recommend using a 350 x 350 image.
    /// </summary>
    [JsonProperty(PropertyName = "image")]
    public string Image { get; set; }

    /// <summary>
    /// Name of the item.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// These are the attributes for the item, which will show up on the OpenSea page for the item.
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public IEnumerable<ERC721Trait> Attributes { get; set; }

    /// <summary>
    /// Not shown on opensea, the unique token Id
    /// </summary>
    [JsonProperty(PropertyName = "edition")]
    public int Id { get; set; }

    /// <summary>
    /// Not shown on opensea, the generation compiler.
    /// </summary>
    [JsonProperty(PropertyName = "compiler")]
    public string Compiler { get; set; }
}

internal struct GenericMeta
{
    /// <summary>
    /// This is the URL to the image of the item. Can be just about any type of image (including SVGs, which will be cached into PNGs by OpenSea), and can be IPFS URLs or paths. We recommend using a 350 x 350 image.
    /// </summary>
    [JsonProperty(PropertyName = "image")]
    public string Image { get; set; }

    /// <summary>
    /// Name of the item.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// These are the attributes for the item, which will show up on the OpenSea page for the item.
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public IEnumerable<ERC721Trait> Attributes { get; set; }

    /// <summary>
    /// Not shown on opensea, the unique token Id
    /// </summary>
    [JsonProperty(PropertyName = "edition")]
    public int Id { get; set; }

    /// <summary>
    /// Not shown on opensea, unique asset DNA.
    /// </summary>
    [JsonProperty(PropertyName = "dna")]
    public string Dna { get; set; }

    /// <summary>
    /// Not shown on opensea, the generation timestamp.
    /// </summary>
    [JsonProperty(PropertyName = "date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Not shown on opensea, the generation compiler.
    /// </summary>
    [JsonProperty(PropertyName = "compiler")]
    public string Compiler { get; set; }
}
