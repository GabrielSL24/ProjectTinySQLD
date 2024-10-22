using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute((string tableName, Dictionary<string, (string type, int? length)> columns) tableParams,        //Parámetro que contiene nombre de la tabla, columnas con su tipo y longitud
                                        out List<(string columnName, ColumnType type, int? length)> columnInfo)                 //Parámetro de salida que contendrá información de las columnas convertidas
        {
            //Desempaqueta el parámetro tableParams en variables locales
            (string tableName, Dictionary<string, (string type, int? length)> columns) = tableParams;

            columnInfo = columns.Select(column => ConvertColumnType(column.Key, column.Value.type, column.Value.length)).ToList();
            //(columnName, type, length)
            var fields = columnInfo.Select(column => (column.columnName, column.type, column.length)).ToArray();

            return Store.GetInstance().CreateTable(tableName, fields);
        }
        //Convierte el tipo de columna de string ("integer", "varchar", etc...) al tipo de datos interno (ColumnType)
        private (string columnName, ColumnType type, int? length) ConvertColumnType(string columnName, string columnType, int? length)
        {
            //Primero se verifica si el type es varchar
            if (columnType.ToLower().StartsWith("varchar"))
            {
                if (length.HasValue)
                {
                    return (columnName, ColumnType.Varchar, length);
                }
                else
                {
                    throw new InvalidOperationException("El tipo VARCHAR requiere un valor de longitud");
                }
            }

            //Devuelve tupla con el nombre de la columna y su tipo enumerado
            switch (columnType.ToLower())
            {
                case "integer":
                    return (columnName, ColumnType.Integer, null);
                case "datetime":
                    return (columnName, ColumnType.DateTime, null);
                case "double":
                    return (columnName, ColumnType.Double, null);
                default:
                    throw new InvalidOperationException($"Tipo no soportado: {columnType}");
            }
        }
    }
}
