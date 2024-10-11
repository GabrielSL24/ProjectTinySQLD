using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute((string tableName, Dictionary<string, (string type, int? length)> columns) tableParams)
        {
            (string tableName, Dictionary<string, (string type, int? length)> columns) = tableParams;
            return Store.GetInstance().CreateTable(tableName);
        }
    }
}
