using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace StoreDataManager.Checks
{
    internal class CheckSystem
    {
        internal static bool VerifyDocuments(string Name, string tableName)
        {
            //Definición de rutas base y específicas para la ubicación del archivo
            const string DatabaseBasePath = @"C:\TinySql\";
            const string DataPath = $@"{DatabaseBasePath}\Data";
            var path = $@"{DataPath}\{Name}";
            var tablePath = $@"{path}\{tableName}.Table";

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    //Si el archivo esta vació (Longitud 0), se procede a eliminarlo
                    if (stream.Length == 0)
                    {
                        //Cierra el stream y el lector antes de eliminar el archivo
                        Console.WriteLine($"El archivo {tableName} está vacío. Procediendo a eliminarlo...");
                        stream.Close();
                        reader.Close();
                        
                        File.Delete(tablePath);
                        Console.WriteLine($"El archivo {tableName} ha sido eliminado");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"El archivo {tableName} NO está vacío.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al verificar o eliminar el archivo {tableName} : {ex.Message}");
                }
            }
            return false;
            
        }
    }
}
