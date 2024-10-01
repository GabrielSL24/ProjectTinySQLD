using System.Text.RegularExpressions;

public class ExtractParameters
{
    // Extraer el nombre de la base de datos (para CREATE DATABASE y SET DATABASE)
    public string ExtractDatabaseName(string sentence)
    {
        var match = Regex.Match(sentence, @"DATABASE\s+(\w+)", RegexOptions.IgnoreCase);
        return match.Groups[1].Value;
    }

    //#TODO: Cambiar metodo de extraccion 
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
    public (List<string> columns, string tableName, (string columnName, string compareOperator, string value) whereClause, (string columnName, string order) orderBy) ExtractSelectParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"SELECT\s+(.+)\s+FROM\s+(\w+)\s*(WHERE\s+(\w+)\s*(=|<|>|LIKE|NOT)\s*(.+))?\s*(ORDER\s+BY\s+(\w+)\s*(ASC|DESC))?", RegexOptions.IgnoreCase);

        // Columnas a seleccionar
        List<string> columns = new List<string>(match.Groups[1].Value.Split(','));

        // Nombre de la tabla
        string tableName = match.Groups[2].Value;

        // WHERE statement (column-name, compare-operator, value)
        (string columnName, string compareOperator, string value) whereClause = (null, null, null);
        if (match.Groups[4].Success && match.Groups[5].Success && match.Groups[6].Success)
        {
            whereClause = (match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
        }

        // ORDER BY statement (column-name, asc/desc)
        (string columnName, string order) orderBy = (null, null);
        if (match.Groups[8].Success && match.Groups[9].Success)
        {
            orderBy = (match.Groups[8].Value, match.Groups[9].Value);
        }

        return (columns, tableName, whereClause, orderBy);
    }


    // Extraer nombre de tabla para DROP TABLE
    public string ExtractTableName(string sentence)
    {
        var match = Regex.Match(sentence, @"TABLE\s+(\w+)", RegexOptions.IgnoreCase);
        return match.Groups[1].Value;
    }

    // Extraer parámetros para INSERT INTO
    public (string tableName, List<string> values) ExtractInsertParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"INSERT\s+INTO\s+(\w+)\s+\((.+)\)", RegexOptions.IgnoreCase);
        string tableName = match.Groups[1].Value;
        List<string> values = new List<string>(match.Groups[2].Value.Split(','));

        return (tableName, values);
    }

    // Extraer parámetros para UPDATE
    public (string tableName, (string columnName, string newValue) setClause, (string columnName, string compareOperator, string value) whereClause) ExtractUpdateParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"UPDATE\s+(\w+)\s+SET\s+(\w+)\s*=\s*(.+)\s+WHERE\s+(\w+)\s*(==|<|>|LIKE|NOT)\s*(.+);", RegexOptions.IgnoreCase);

        // Nombre de la tabla
        string tableName = match.Groups[1].Value;

        // SET clause
        (string columnName, string newValue) setClause = (match.Groups[2].Value, match.Groups[3].Value);

        // WHERE clause (column-name, compare-operator, value)
        (string columnName, string compareOperator, string value) whereClause = (match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);

        return (tableName, setClause, whereClause);
    }

    // Extraer parámetros para DELETE
    public (string tableName, (string columnName, string compareOperator, string value) whereClause) ExtractDeleteParameters(string sentence)
    {
        var match = Regex.Match(sentence, @"DELETE\s+FROM\s+(\w+)\s+WHERE\s+(\w+)\s*(==|<|>|LIKE|NOT)\s*(.+);", RegexOptions.IgnoreCase);

        // Nombre de la tabla
        string tableName = match.Groups[1].Value;

        // WHERE clause (column-name, compare-operator, value)
        (string columnName, string compareOperator, string value) whereClause = (match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);

        return (tableName, whereClause);
    }
}

