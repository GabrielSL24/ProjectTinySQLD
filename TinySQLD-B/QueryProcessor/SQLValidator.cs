using System.Text.RegularExpressions;

public class SQLValidator
{
    // Validar CREATE DATABASE
    public bool IsCreateDatabase(string sentence)
    {
        return Regex.IsMatch(sentence, @"^CREATE\s+DATABASE\s+\w+$", RegexOptions.IgnoreCase);
    }

    // Validar SET DATABASE
    public bool IsSetDatabase(string sentence)
    {
        return Regex.IsMatch(sentence, @"^SET\s+DATABASE\s+\w+$", RegexOptions.IgnoreCase);
    }

    // Validar CREATE TABLE
    public bool IsCreateTable(string sentence)
    {
        return Regex.IsMatch(sentence, @"CREATE\s+TABLE\s+\w+\s*\(.+\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }

    // Validar DROP TABLE
    public bool IsDropTable(string sentence)
    {
        return Regex.IsMatch(sentence, @"^DROP\s+TABLE\s+\w+$", RegexOptions.IgnoreCase);
    }

    // Validar SELECT
    public bool IsSelect(string sentence)
    {
        return Regex.IsMatch(sentence, @"^SELECT\s+(\*|\w+(\s*,\s*\w+)*)\s+FROM\s+\w+(\s+WHERE\s+\w+\s*(=|<|>|LIKE|NOT)\s*.+)?(\s+ORDER\s+BY\s+\w+\s*(ASC|DESC))?$", RegexOptions.IgnoreCase);
    }

    // Validar INSERT INTO
    public bool IsInsertInto(string sentence)
    {
        return Regex.IsMatch(sentence, @"^INSERT\s+INTO\s+\w+\s+VALUES\s*\(.+\)$", RegexOptions.IgnoreCase);
    }

    // Validar UPDATE
    public bool IsUpdate(string sentence)
    {
        return Regex.IsMatch(sentence, @"^UPDATE\s+\w+\s+SET\s+\w+\s*=\s*.+\s+WHERE\s+\w+\s*(==|<|>|LIKE|NOT)\s*.+$", RegexOptions.IgnoreCase);
    }

    // Validar DELETE
    public bool IsDelete(string sentence)
    {
        return Regex.IsMatch(sentence, @"^DELETE\s+FROM\s+\w+\s+WHERE\s+\w+\s*(==|<|>|LIKE|NOT)\s*.+$", RegexOptions.IgnoreCase);
    }

    // Validar INDEX
    public bool IsCreateIndex(string sentence)
    {
        return Regex.IsMatch(sentence, @"^CREATE\s+INDEX\s+\w+\s+ON\s+\w+\s*\(\w+\)\s+OF\s+TYPE\s+(BTREE|BST)$", RegexOptions.IgnoreCase);
    }

}

