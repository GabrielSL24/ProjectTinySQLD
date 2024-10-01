using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        private SQLValidator validator = new SQLValidator();
        private ExtractParameters extractor = new ExtractParameters();

        public OperationStatus Execute(string sentence)
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
                throw new NotImplementedException();
                //return new SetDatabase().Execute(dbName);
            }
            // Verificar si la sentencia corresponde a "CREATE TABLE"
            else if (validator.IsCreateTable(sentence))
            {
                var tableParams = extractor.ExtractCreateTableParameters(sentence);
                return new CreateTable().Execute(tableParams);
            }
            // Verificar si la sentencia corresponde a "SELECT"
            else if (validator.IsSelect(sentence))
            {
                var selectParams = extractor.ExtractSelectParameters(sentence);
                return new Select().Execute();
            }
            // Verificar otras sentencias SQL (DROP TABLE, INSERT INTO, etc.)
            else if (validator.IsDropTable(sentence))
            {
                string tableName = extractor.ExtractTableName(sentence);
                throw new NotImplementedException();
                //return new DropTable().Execute(tableName);
            }
            else if (validator.IsInsertInto(sentence))
            {
                var insertParams = extractor.ExtractInsertParameters(sentence);
                throw new NotImplementedException();
                //return new InsertInto().Execute(insertParams);
            }
            else if (validator.IsUpdate(sentence))
            {
                var updateParams = extractor.ExtractUpdateParameters(sentence);
                throw new NotImplementedException();
                //return new Update().Execute(updateParams);
            }
            else if (validator.IsDelete(sentence))
            {
                var deleteParams = extractor.ExtractDeleteParameters(sentence);
                throw new NotImplementedException();
                //return new Delete().Execute(deleteParams);
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
