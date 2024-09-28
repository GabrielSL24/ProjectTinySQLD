using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string NameTB)
        {
            return Store.GetInstance().CreateTable(NameTB);
        }
    }
}
