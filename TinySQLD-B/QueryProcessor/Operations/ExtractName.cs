namespace QueryProcessor.Operations
{
    internal class ExtractName
    {
        internal static string Extract(string CommandSentence)
        {
            string errorint = "Error, es mayor a 3 elementos";
            if (CommandSentence.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = CommandSentence.Split(' ');
                if (parts.Length >= 3)
                {
                    string NameDB = parts[2];
                    return NameDB;
                }
                return errorint;
            }
            if (CommandSentence.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = CommandSentence.Split(' ');
                if (parts.Length >= 3)
                {
                    string NameDB = parts[2];
                    return NameDB;
                }
                return errorint;
            }
            else
            {
                return "Error, no se especificaba a una Database";
            }
        }
    }
}
