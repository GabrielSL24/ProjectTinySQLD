using Entities;
using StoreDataManager;


namespace QueryProcessor.Operations
{
    internal class CreateDataBase
    {
        internal OperationStatus Execute(string NameDB)
        {
            return Store.GetInstance().CreateDataBase(NameDB);
        }
    }
}
