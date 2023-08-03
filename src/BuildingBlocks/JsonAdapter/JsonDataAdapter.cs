using Newtonsoft.Json.Linq;

namespace Bazaar.BuildingBlocks.JsonAdapter;

public class JsonDataAdapter
{
    private readonly string READ_PATH;

    public JsonDataAdapter(string path)
    {
        READ_PATH = path;
    }

    public IEnumerable<T> ReadToObjects<T>(string fromSection)
    {
        var dataJson = JObject.Parse(File.ReadAllText(READ_PATH));
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
