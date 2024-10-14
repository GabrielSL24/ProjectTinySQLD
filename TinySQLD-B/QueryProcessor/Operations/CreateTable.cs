using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute((string tableName, Dictionary<string, (string type, int? length)> columns) tableParams)
        {
            (string tableName, Dictionary<string, (string type, int? length)> columns) = tableParams;

            var fields = columns.Select(column => ConvertColumnType(column.Key, column.Value.type)).ToArray();
            return Store.GetInstance().CreateTable(tableName);
        }

        private (object value, ColumnType type) ConvertColumnType(string columnName, string columnType)
        {
            switch (columnType.ToLower())
            {
                case "integer":
                    return (0, ColumnType.Integer);
                case "datetime":
                    return (DateTime.Now, ColumnType.DateTime);
                case "double":
                    return (0.0, ColumnType.Double);
                case "varchar":
                    return (string.Empty, ColumnType.Varchar);
                default:
                    throw new InvalidOperationException($"Tipo no soportado: {columnType}");
            }
        }
    }
}
