using StoreDataManager;
using Entities;
using QueryProcessor.Parser;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(string TableName, List<(string value, string type)> values)
        {
            return Store.GetInstance().InsertInto(TableName, values);
        }

        internal static string DetectType(string value)
        {
            //Detectar si el valor es número entero
            if (int.TryParse(value, out _))
            {
                return "Integer";
            }
            //Detectar si el valor es número decimal
            else if (double.TryParse(value, out _))
            {
                return "Double";
            }
            //Detectar si el valor es un string
            else if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                string identifyValue = value.Trim('\"');
                if (Regex.IsMatch(identifyValue, @"^\d{4}-\d{2}-\d{2}(T\d{2}:\d{2}:\d{2})?$"))
                {
                    return "DateTime";
                }
                else
                {
                    return "Varchar";
                }
            }
            else
            {
                return "unkown";
            }

        }
    }
}
