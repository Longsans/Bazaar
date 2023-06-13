namespace Bazaar.BuildingBlocks.Transactions.Abstractions
{
    public interface IResourceLocationResolver
    {
        public IList<string> GetResourceNodesFromIndexes(IList<string> indexes);
    }
}
