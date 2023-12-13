namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface ILotRepository
{
    Lot? GetById(int id);
    Lot? GetByLotNumber(string lotNumber);
    IEnumerable<Lot> GetManyByLotNumber(IEnumerable<string> lotNumbers);
    Lot? GetFulfillableById(int id);
    Lot? GetUnfulfillableById(int id);
    IEnumerable<Lot> GetUnfulfillables();
    Lot Create(Lot lot);
    void Update(Lot lot);
    void UpdateRange(IEnumerable<Lot> lot);
}
