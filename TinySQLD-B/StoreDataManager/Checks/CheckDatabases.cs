using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StoreDataManager.Checks
{
    internal class CheckDatabases
    {
        internal static void CheckDB(string path ,int id, string TableName)
        {
            int idLength = 4;
            int nameLength = 15;

            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    while (stream.Position < stream.Length)
                    {
                        int currentID = reader.ReadInt32();
                        byte[] nameBytes = reader.ReadBytes(nameLength);
                        string tableName = Encoding.ASCII.GetString(nameBytes).Trim();

                        Console.WriteLine($"ID leido: {currentID}, Nombre de la base de datos: {tableName}");

                        if (currentID == id)
                        {
                            Console.WriteLine($"ID {id} encontrado. Nombre de la base de datos: {tableName}");
                            CheckSystem.VerifyDocuments(tableName, TableName);
                            return;
                        }
                    }
                    Console.WriteLine($"ID {id} no encontrado en el archivo");
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("Fin del archivo");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error leyendo archivo: {ex.Message}");
                }
            }
        }
    }
}
