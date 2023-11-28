namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface ILotRepository
{
    Lot? GetById(int id);
    Lot? GetByLotNumber(string lotNumber);
    IEnumerable<Lot> GetManyByLotNumber(IEnumerable<string> lotNumbers);
    FulfillableLot? GetFulfillableById(int id);
    UnfulfillableLot? GetUnfulfillableById(int id);
    IEnumerable<UnfulfillableLot> GetUnfulfillables();
    FulfillableLot CreateFulfillable(FulfillableLot lot);
    UnfulfillableLot CreateUnfulfillable(UnfulfillableLot lot);
    void Update(Lot lot);
    void UpdateRange(IEnumerable<Lot> lot);
}
