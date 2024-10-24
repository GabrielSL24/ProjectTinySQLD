using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        static private SQLValidator validator = new SQLValidator();
        static private ExtractParameters extractor = new ExtractParameters();
        static private string setDB = null;

        public static OperationStatus Execute(string sentence)
        {
            // Verificar si la sentencia corresponde a "CREATE DATABASE"
            if (validator.IsCreateDatabase(sentence))
            {
                string dbName = extractor.ExtractDatabaseName(sentence);
                return new CreateDataBase().Execute(dbName);
            }
            // Verificar si la sentencia corresponde a "SET DATABASE"
            else if (validator.IsSetDatabase(sentence))
            {
                string dbName = extractor.ExtractDatabaseName(sentence); 
                return new SetDatabase().Execute(dbName);
            }
            // Verificar si la sentencia corresponde a "CREATE TABLE"
            else if (validator.IsCreateTable(sentence))
            {
                var tableParams = extractor.ExtractCreateTableParameters(sentence);
                List<(string columnName, ColumnType type, int? length)> columnInfo;
                return new CreateTable().Execute(tableParams, out columnInfo);
            }
            // Verificar si la sentencia corresponde a "SELECT"
            else if (validator.IsSelect(sentence))
            {
                var selectParams = extractor.ExtractSelectParameters(sentence);
                var (column, tableName, whereClause, orderBy) = selectParams;
                return new Select().Execute(tableName);
            }
            // Verificar si la sentencia corresponde a "DROP TABLE"
            else if (validator.IsDropTable(sentence))
            {
                string tableName = extractor.ExtractTableName(sentence);
                return new DropTable().Execute(tableName);
            }
            // Verificar si la sentencia corresponde a "INSERT INTO"
            else if (validator.IsInsertInto(sentence))
            {
                var (tablename, values) = extractor.ExtractInsertParameters(sentence); 
                return new Insert().Execute(tablename, values);
            }
            // Verificar si la sentencia corresponde a "UPDATE"
            else if (validator.IsUpdate(sentence))
            {
                var updateParams = extractor.ExtractUpdateParameters(sentence);
                throw new NotImplementedException();
                //return new Update().Execute(updateParams);
            }
            // Verificar si la sentencia corresponde a "DELETE"
            else if (validator.IsDelete(sentence))
            {
                var deleteParams = extractor.ExtractDeleteParameters(sentence);
                throw new NotImplementedException();
                //return new Delete().Execute(deleteParams);
            }
            // Verificar si la sentencia corresponde a "INDEX"
            else if (validator.IsCreateIndex(sentence))
            {
                var indexParams = extractor.ExtractCreateIndexParameters(sentence);
                throw new NotImplementedException();
                //return new CreateIndex().Execute(indexParams);
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
