using System.Text;
using System.Text.Json;

namespace Bazaar.BuildingBlocks.Transactions.Utility
{
    public static class TransmissionUtil
    {
        public static StringContent SerializeToJson(object content)
            => new(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
    }
}
