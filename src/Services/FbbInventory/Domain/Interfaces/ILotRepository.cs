namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface ILotRepository
{
    Lot? GetById(int id);
    Lot? GetByLotNumber(string lotNumber);
    IEnumerable<Lot> GetUnfulfillables();
    Lot Create(Lot lot);
    void Update(Lot lot);
    void UpdateRange(IEnumerable<Lot> lot);
}
