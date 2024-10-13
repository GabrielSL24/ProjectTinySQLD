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

        internal static bool RemoveDatabases(string path ,int id, string TableName)
        {
            //Tamaño en bytes para el ID (4 bytes) y nombre de la tabla (15 bytes)
            int idLength = 4;
            int nameLength = 15;

            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    //Mientras la posición actual en el archivo sea menor que su longitud total, sigue leyendo.
                    while (stream.Position < stream.Length)
                    {
                        int currentID = reader.ReadInt32();
                        byte[] nameBytes = reader.ReadBytes(nameLength);
                        string tableName = Encoding.ASCII.GetString(nameBytes).Trim();  //Convierte los bytes leídos a un string
                        Console.WriteLine($"ID leido: {currentID}, Nombre de la base de datos: {tableName}");

                        if (currentID == id)
                        {
                            Console.WriteLine($"ID {id} encontrado. Nombre de la base de datos: {tableName}");
                            //Verifica si la tabla esta vacía
                            if (CheckSystem.VerifyDocuments(tableName, TableName))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                            //return;
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
            return false;
        }
    }
}
