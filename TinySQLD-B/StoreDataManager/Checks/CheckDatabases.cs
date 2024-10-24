using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
                        string DatabaseName = Encoding.ASCII.GetString(nameBytes).Trim();  //Convierte los bytes leídos a un string
                        Console.WriteLine($"ID leido: {currentID}, Nombre de la base de datos: {DatabaseName}");

                        if (currentID == id)
                        {
                            Console.WriteLine($"ID {id} encontrado. Nombre de la base de datos: {DatabaseName}");
                            //Verifica si la tabla esta vacía
                            if (CheckSystem.VerifyDocuments(DatabaseName, TableName))
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

        //Función que retorna el nombre de la base de Datos
        internal static string CheckNameDatabase(int idDB, string path)
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
                        string databaseName = Encoding.ASCII.GetString(nameBytes).Trim();  //Convierte los bytes leídos a un string
                        Console.WriteLine($"ID leido: {currentID}, Nombre de la base de datos: {databaseName}");

                        //Busca si el ID enviado es igual al currentID para retornar el nombre de la base de datos
                        if (currentID == idDB)
                        {
                            Console.WriteLine($"ID {idDB} encontrado. Nombre de la base de datos: {databaseName}");
                            return databaseName;
                        }
                    }
                    Console.WriteLine($"ID {idDB} no encontrado en el archivo");
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
            return null;
        }

        //Función que retorna el id de una Database
        internal static int CheckIdDatabase(string nameTable, string path)
        {
            int sizeB = 23;                 //tamaño
            int idDtabaseOffset = 0;               //Posición actual
            int idTableOffset = 4;               //Longitud de ID (bytes)
            int nameOffset = 8;             //Posición inicial del nombre
            int nameLength = 15;
            int currentID = 0;

            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                //Recorre el archivo original hasta llegar al final
                while (stream.Position < stream.Length)
                {
                    long recordPosition = stream.Position;
                    byte[] recordData = reader.ReadBytes(sizeB);

                    //Verifica si el tamaño del registro leído es suficiente para contener ID y nombre
                    if (recordData.Length >= nameOffset + nameLength)
                    {
                        int idDatabase = BitConverter.ToInt32(recordData, idDtabaseOffset);
                        int idTB = BitConverter.ToInt32(recordData, idTableOffset);
                        Console.WriteLine($"ID de la base de datos: {idDatabase}, ID de la tabla: {idTB}");

                        //Extrae el nombre del resgistro (15 bytes desde la posición 4)
                        string recordString = Encoding.ASCII.GetString(recordData, nameOffset, nameLength).Trim();
                        //Elimina caracteres de control del nombre extraído
                        recordString = new string(recordString.Where(c => !char.IsControl(c)).ToArray());

                        if (recordString.Equals(nameTable, StringComparison.Ordinal))
                        {
                            currentID = idDatabase;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"El registro en posición {recordPosition} es demasiado pequeño o mal formado");
                    }
                }
            }
            return currentID;
        }
    }
}
