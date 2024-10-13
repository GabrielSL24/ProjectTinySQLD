using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class SetDatabase
    {
        public SetDatabase()
        {
        }

        internal OperationStatus Execute(string dbName)
        {
            //Verifica que la DB exista, y si ese es el caso la setea, sino manda un warning
            return Store.GetInstance().SetDataBase(dbName);
        }
    }
}