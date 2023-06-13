namespace Bazaar.BuildingBlocks.Transactions
{
    public class Lock
    {
        private List<TransactionRef> _owners = new();
        public LockMode CurrentMode;

        public Lock()
        {
            CurrentMode = LockMode.READ;
        }

        public Lock(LockMode mode)
        {
            CurrentMode = mode;
        }

        public bool IsCompatible(TransactionRef txn, LockMode requestedMode)
        {
            return _owners.Count == 0
                    || (requestedMode == LockMode.READ && CurrentMode == LockMode.READ)
                    || (_owners.Contains(txn) && requestedMode == LockMode.READWRITE);
        }

        public void AddOwner(TransactionRef txn, LockMode requestedMode)
        {
            if (!_owners.Contains(txn))
                _owners.Add(txn);
            CurrentMode = requestedMode;
        }

        public void RemoveOwner(TransactionRef txn)
        {
            if (_owners.Contains(txn))
                _owners.Remove(txn);
        }

        public bool HasOwner(TransactionRef txn) => _owners.Contains(txn);
    }

    public enum LockMode
    {
        READ = 1,
        READWRITE = 2,
    }
}