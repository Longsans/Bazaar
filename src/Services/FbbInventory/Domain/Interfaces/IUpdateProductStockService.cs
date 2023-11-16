﻿namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IUpdateProductStockService
{
    Result ReduceStock(string productId, uint units);
    Result Restock(string productId, uint units);
}