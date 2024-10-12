using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataManager.Checks
{
    internal class CheckSystem
    {
        internal static void VerifyDocuments(string Name, string tableName)
        {
            const string DatabaseBasePath = @"C:\TinySql\";
            const string DataPath = $@"{DatabaseBasePath}\Data";
            var path = $@"{DataPath}\{Name}";
            var tablePath = $@"{path}\{tableName}.Table";

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    if (stream.Length == 0)
                    {
                        Console.WriteLine($"El archivo {tableName} está vacío. Procediendo a eliminarlo...");
                        stream.Close();
                        reader.Close();
                        
                        File.Delete(tablePath);
                        Console.WriteLine($"El archivo {tableName} ha sido eliminado");

                    }
                    else
                    {
                        Console.WriteLine($"El archivo {tableName} NO está vacío.");
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al verificar o eliminar el archivo {tableName} : {ex.Message}");
                }
            }
            
        }
    }
}
