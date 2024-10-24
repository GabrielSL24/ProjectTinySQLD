using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public OperationStatus Execute(string nameTable)
        {
            // This is only doing the query but not returning results.
            return Store.GetInstance().Select(nameTable);
        }
    }
}
