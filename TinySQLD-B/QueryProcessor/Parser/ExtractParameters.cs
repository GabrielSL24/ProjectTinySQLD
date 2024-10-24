using QueryProcessor.Operations;
using QueryProcessor.Parser;
using StoreDataManager.Checks;
using System.Text.RegularExpressions;

public class ExtractParameters
{
    // Extraer el nombre de la base de datos (para CREATE DATABASE y SET DATABASE)
    public string ExtractDatabaseName(string sentence)
    {
        var match = Regex.Match(sentence, @"DATABASE\s+(\w+)", RegexOptions.IgnoreCase);
        return match.Groups[1].Value;
    }

    // Extraer los parámetros de la tabla para CREATE TABLE
    public (string tableName, Dictionary<string, (string type, int? length)> columns) ExtractCreateTableParameters(string sentence)
    {
        // Modificar la expresión regular para aceptar multilinea
        var match = Regex.Match(sentence, @"CREATE\s+TABLE\s+(\w+)\s*\((.+)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de CREATE TABLE.");
        }

        // Nombre de la tabla
        string tableName = match.Groups[1].Value;

        // Definición de columnas
        var columns = new Dictionary<string, (string type, int? length)>();
        string columnDefinitions = match.Groups[2].Value;

        // Match para capturar columnas en formato multilinea
        var columnMatches = Regex.Matches(columnDefinitions, @"(\w+)\s+(INTEGER|DOUBLE|VARCHAR\((\d+)\)|DATETIME)", RegexOptions.IgnoreCase);

        foreach (Match columnMatch in columnMatches)
        {
            string columnName = columnMatch.Groups[1].Value;
            string columnType = columnMatch.Groups[2].Value;
            int? length = null;

            if (columnMatch.Groups[3].Success)
            {
                length = int.Parse(columnMatch.Groups[3].Value);
            }

            columns.Add(columnName, (columnType, length));
        }

        return (tableName, columns);
    }

    // Extraer los parámetros de SELECT
    public (string column, string tableName, (string columnName, string compareOperator, string value)? whereClause, (string columnName, string order)? orderBy) ExtractSelectParameters(string sentence)
    {
        // Expresión regular para extraer la columna, tabla, where y order by
        var match = Regex.Match(sentence, @"SELECT\s+(\*|\w+)\s+FROM\s+(\w+)\s*(WHERE\s+(\w+)\s*(=|<|>|LIKE|NOT)\s*(\S+))?\s*(ORDER\s+BY\s+(\w+)\s*(ASC|DESC))?", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de SELECT.");
        }

        // Extraer columna
        string column = match.Groups[1].Value;

        // Nombre de la tabla
        string tableName = match.Groups[2].Value;

        // WHERE clause
        (string columnName, string compareOperator, string value)? whereClause = null;
        if (match.Groups[4].Success && match.Groups[5].Success && match.Groups[6].Success)
        {
            whereClause = (match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
        }

        // ORDER BY clause
        (string columnName, string order)? orderBy = null;
        if (match.Groups[8].Success && match.Groups[9].Success)
        {
            orderBy = (match.Groups[8].Value, match.Groups[9].Value);
        }

        return (column, tableName, whereClause, orderBy);
    }

    // Extraer nombre de tabla para DROP TABLE
    public string ExtractTableName(string sentence)
    {
        var match = Regex.Match(sentence, @"TABLE\s+(\w+)", RegexOptions.IgnoreCase);
        return match.Groups[1].Value;
    }

    // Extraer parámetros para INSERT INTO
    public (string tableName, List<(string value, string type)> values) ExtractInsertParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"INSERT\s+INTO\s+(\w+)\s+VALUES\s*\((.+)\)", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de INSERT.");
        }

        string tableName = match.Groups[1].Value;
        List<(string value, string type)> values = new List<(string value, string type)>();

        // Separar los valores por comas y agregarlos a nuestra estructura personalizada
        string[] splitValues = match.Groups[2].Value.Split(',');
        foreach (string value in splitValues)
        {
            string ValueDatos = value.Trim();
            string type = Insert.DetectType(ValueDatos);
            values.Add((ValueDatos, type));  // Agregamos cada valor, eliminando espacios
        }
        return (tableName, values);
    }

    // Extraer parámetros para UPDATE
    public (string tableName, (string columnName, string newValue) setClause, (string columnName, string compareOperator, string value)? whereClause) ExtractUpdateParameters(string sentence)
    {
        // Actualizamos la expresión regular para manejar los valores con comillas
        var match = Regex.Match(sentence, @"UPDATE\s+(\w+)\s+SET\s+(\w+)\s*=\s*""([^""]+)""\s*(WHERE\s+(\w+)\s*(==|<|>|LIKE|NOT)\s*(\S+))?;", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de UPDATE.");
        }

        // Nombre de la tabla
        string tableName = match.Groups[1].Value;

        // SET clause (nombre de la columna y el nuevo valor entre comillas)
        (string columnName, string newValue) setClause = (match.Groups[2].Value, match.Groups[3].Value);

        // WHERE clause (column-name, compare-operator, value), opcional
        (string columnName, string compareOperator, string value)? whereClause = null;
        if (match.Groups[5].Success && match.Groups[6].Success && match.Groups[7].Success)
        {
            whereClause = (match.Groups[5].Value, match.Groups[6].Value, match.Groups[7].Value);
        }

        return (tableName, setClause, whereClause);
    }

    // Extraer parámetros para DELETE
    public (string tableName, (string columnName, string compareOperator, string value)? whereClause) ExtractDeleteParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"DELETE\s+FROM\s+(\w+)\s*(WHERE\s+(\w+)\s*(==|<|>|LIKE|NOT)\s*(.+))?;", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de DELETE.");
        }

        // Nombre de la tabla
        string tableName = match.Groups[1].Value;

        // WHERE clause (column-name, compare-operator, value)
        (string columnName, string compareOperator, string value)? whereClause = null;
        if (match.Groups[3].Success && match.Groups[4].Success && match.Groups[5].Success)
        {
            whereClause = (match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value);
        }

        return (tableName, whereClause);
    }

    // Extraer parámetros para INDEX
    public (string indexName, string tableName, string columnName, string type) ExtractCreateIndexParameters(string sentence)
    {
        // Extraer el nombre del índice, el nombre de la tabla, la columna y el tipo de índice
        var match = Regex.Match(sentence, @"CREATE\s+INDEX\s+(\w+)\s+ON\s+(\w+)\s*\((\w+)\)\s+OF\s+TYPE\s+(BTREE|BST)", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception("Error en la sintaxis de CREATE INDEX.");
        }

        string indexName = match.Groups[1].Value;
        string tableName = match.Groups[2].Value;
        string columnName = match.Groups[3].Value;
        string type = match.Groups[4].Value;

        return (indexName, tableName, columnName, type);
    }

}

