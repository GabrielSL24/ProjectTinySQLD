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

        public enum CreateTypeTB
        {
            INTEGER = 0,
            DOUBLE = 1
        }
    }
}
