using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreDataManager.Checks
{
    internal class CheckColumns
    {
        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.Table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.Table";
        private const string SystemColumnFile = $@"{SystemCatalogPath}\SystemColumns.Table";

        //Función para leer las columnas de SystemColumns
        internal static List<string> LoadSystemColums(int idTable)
        {
            var columnTypes = new List<string>();

            using (FileStream stream = new FileStream(SystemColumnFile, FileMode.OpenOrCreate))
            using (BinaryReader reader = new (stream)) 
            {
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int idTB = reader.ReadInt32();
                        string columnName = new string(reader.ReadChars(15)).Trim();
                        string columnType = new string(reader.ReadChars(10)).Trim();
                        int extra = reader.ReadInt32();

                        if (idTB == idTable)
                        {
                            columnTypes.Add(columnType);
                        }
                    }
                }
                return columnTypes; 
            }
        }
        //Función para verificar los tipos de Datos
        internal static bool Check(int id, List<(string value, string type)> values)
        {
            //Cargar las columnas y sus tipos desde SystemColumns
            List<string> columnTypes = LoadSystemColums(id);

            //Verificar que el número de columnas coincida
            if (columnTypes.Count != values.Count)
            {
                throw new Exception("El número de columnas no coincide");
            }

            //Itera de manera que va verificando si son iguales los tipos de datos enviados y los de la tabla
            for (int i = 0; i < columnTypes.Count; i++)
            {
                string expectedType = columnTypes[i];
                string providedType = values[i].type;

                if (!IsMatchingType(expectedType, providedType))
                {
                    throw new Exception($"El valor '{values[i].value}' no coincide con el tipo esperado '{expectedType}' en la columna {i + 1}.");
                }
            }
            return true;
        }

        //Función que compara el tipo de la columna con el tipo de Dato
        private static bool IsMatchingType(string type, string providedType)
        {
            return type.Equals(providedType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
