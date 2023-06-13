namespace Bazaar.BuildingBlocks.Transactions;
using System;
using System.Collections.Generic;

public class LockManager<TIndex> where TIndex : IEquatable<TIndex>
{
    private Dictionary<TIndex, Lock> _indexLocks = new();

    public void Aquire(TIndex index, TransactionRef txn, LockMode requestedMode)
    {
        if (_indexLocks.ContainsKey(index))
        {
            if (!_indexLocks[index].IsCompatible(txn, requestedMode))
                throw new InvalidOperationException("Requested lock violates existing locks.");
            _indexLocks[index].CurrentMode = requestedMode;
        }
        else _indexLocks.Add(index, new Lock(requestedMode));

        if (!_indexLocks[index].HasOwner(txn))
            _indexLocks[index].AddOwner(txn, requestedMode);
    }

    public void Aquire(IEnumerable<TIndex> indexes, TransactionRef txn, LockMode requestedMode)
    {
        foreach (var index in indexes)
            Aquire(index, txn, requestedMode);
    }

    public void Release(TIndex index, TransactionRef txn)
    {
        if (_indexLocks.ContainsKey(index) && _indexLocks[index].HasOwner(txn))
        {
            _indexLocks[index].RemoveOwner(txn);
            _indexLocks.Remove(index);
        }
    }

    public void Release(IEnumerable<TIndex> indexes, TransactionRef txn)
    {
        foreach (var index in indexes)
            Release(index, txn);
    }
}