using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Bazaar.BuildingBlocks.JsonAdapter;

public class JsonDataAdapter
{
    private readonly string DATA_STORE_PATH;

    public JsonDataAdapter(IConfiguration config)
    {
        DATA_STORE_PATH = config["DataStorePath"]!;
    }

    public IEnumerable<T> ReadToObjects<T>(string fromSection)
    {
        var dataJson = JObject.Parse(File.ReadAllText(DATA_STORE_PATH));
        var itemsJson = dataJson[fromSection]!.Children().ToList();
        var items = new List<T>();
        for (int i = 0; i < itemsJson.Count; i++)
        {
            var item = itemsJson[i].ToObject<T>();
            items.Add(item);
        }
        return items;
    }
}
