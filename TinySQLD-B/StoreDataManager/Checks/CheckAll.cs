using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataManager.Checks
{
    internal class CheckAll
    {
        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.Table";

        //Función para verificar si una Database existe.
        internal static bool Check(string path, string name)
        {

            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    int nameSize = 15;                   //tamaño fijo del nombre
                    byte[] nameBuffer = new byte[nameSize]; //Buffer donde se almacenerá el nombre leído
                    //Mientras no llegue al final del archivo sigue leyendo
                    while (stream.Position < stream.Length)
                    {
                        int currentID = reader.ReadInt32();
                        Console.WriteLine($"ID leído {currentID}");
                        //Lee los siguientes bytes despues del ID (15 bytes)
                        int bytesRead = reader.Read(nameBuffer, 0, nameSize);

                        if (bytesRead < nameSize)
                        {
                            Console.WriteLine("No hay suficiente bytes para leer el nombre");
                            return false;
                        }
                        //Convierte los bytes leídos del nombre en una cadena de texto UTF-8, quitando espacios en blancos
                        string currentName = Encoding.UTF8.GetString(nameBuffer).Trim();
                        Console.WriteLine($"Nombre leído: {currentName}");

                        if (currentName.Equals(name.Trim(), StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
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

        //Función que retorna el ID de una tabla con el ID de una Database
        internal static int CheckID(int IdDatabase, string path)
        {
            int BytesSize = 23;             //Tamaño del registro (ID database, ID table, Name table)
            int currentID = 0;
            int IDSize1 = 0;                //Posición inicial
            int IDSize2 = 4;                //Longitud de bytes luego de leer el primer ID

            using (FileStream stream = File.Open(path, FileMode .OpenOrCreate))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (stream.Position  < stream.Length)
                {
                    //RecordData es el registro en sí, y luego se guardan en variables desde las posiciones respectivas
                    byte[] recordData = reader.ReadBytes(BytesSize);
                    int IdDB = BitConverter.ToInt32(recordData, IDSize1);               //Desde la posición 0 lee 4 bytes
                    int IdTable = BitConverter.ToInt32(recordData, IDSize2);            //Desde después de el primer ID lee el segundo

                    if (IdDatabase == IdDB)
                    {
                        currentID = IdTable;
                    }
                }
            }
            return currentID;
        }

        //Función que retorna el ID de una Tabla con el Nombre
        internal static int CheckID(string NameTable)
        {
            int sizeB = 23;                 //tamaño
            int idDtabaseOffset = 0;               //Posición actual
            int idTableOffset = 4;               //Longitud de ID (bytes)
            int nameOffset = 8;             //Posición inicial del nombre
            int nameLength = 15;
            int currentID = 0;

            using (FileStream stream = File.Open(SystemTablesFile, FileMode.OpenOrCreate))
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
                        int idTable = BitConverter.ToInt32(recordData, idTableOffset);
                        Console.WriteLine($"ID de la base de datos: {idDatabase}, ID de la tabla: {idTable}");

                        //Extrae el nombre del resgistro (15 bytes desde la posición 4)
                        string recordString = Encoding.ASCII.GetString(recordData, nameOffset, nameLength).Trim();
                        //Elimina caracteres de control del nombre extraído
                        recordString = new string(recordString.Where(c => !char.IsControl(c)).ToArray());

                        if (recordString.Equals(NameTable, StringComparison.Ordinal))
                        {
                            currentID = idTable;
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
