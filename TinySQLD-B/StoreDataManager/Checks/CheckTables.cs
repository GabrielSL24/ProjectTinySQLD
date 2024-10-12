using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StoreDataManager.Checks
{
    internal class CheckTables
    {
        internal static bool CheckTB(string path, string name)
        {
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    int nameSize = 15;
                    byte[] nameBuffer = new byte[nameSize];

                    while (stream.Position < stream.Length)
                    {
                        int currentID = reader.ReadInt32();
                        Console.WriteLine($"ID leído {currentID}");

                        int bytesRead = reader.Read(nameBuffer, 0, nameSize);

                        if (bytesRead < nameSize)
                        {
                            Console.WriteLine("No hay suficiente bytes para leer el nombre");
                            return false;
                        }

                        string currentName = Encoding.UTF8.GetString(nameBuffer).Trim();
                        Console.WriteLine($"Nombre leído: {currentName}");

                        if (currentName.Equals(name.Trim(), StringComparison.Ordinal)) ;
                        {
                            if (reader.ReadInt32() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                stream.Close();
                                return false;
                            }
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

        internal static void RemoveTable(string pathDB,string path, string TableName)
        {
            int sizeB = 19;
            int idOffset = 0;
            int idLength = 4;
            int nameOffset = 4;
            int nameLength = 15;
            string tempPath = Path.GetTempFileName();
            try
            {
                using (FileStream originalStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(originalStream))
                using (FileStream tempStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer =  new BinaryWriter(tempStream))
                {
                    while (originalStream.Position < originalStream.Length)
                    {
                        long recordPosition = originalStream.Position;
                        byte[] recordData = reader.ReadBytes(sizeB);

                        if (recordData.Length >= nameOffset + nameLength)
                        {
                            int id = BitConverter.ToInt32(recordData, idOffset);

                            Console.WriteLine($"Bytes del ID (hex): {BitConverter.ToString(recordData, idOffset, idLength)}");
                            Console.WriteLine($"ID convertido: {id}");


                            string recordString = Encoding.ASCII.GetString(recordData, nameOffset, nameLength).Trim();
                            recordString = new string(recordString.Where(c => !char.IsControl(c)).ToArray());

                            if (!recordString.Equals(TableName.Trim(), StringComparison.Ordinal))
                            {
                                writer.Write(recordData);
                            }
                            else
                            {
                                Console.WriteLine($"Registro {TableName} encontrando en posición {recordPosition} y será eliminado");
                                Console.WriteLine($"El id es: {id}");
                                CheckDatabases.CheckDB(pathDB, id, TableName);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"El registro en posición {recordPosition} es demasiado pequeño o mal formado");
                        }
                    }
                }
                File.Delete(path);
                File.Move(tempPath, path);
                Console.WriteLine($"El registro {TableName} ha sido eliminado fisicamente del archivo");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer el archivo: " + ex.Message);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
    }
}
