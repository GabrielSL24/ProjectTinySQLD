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
        internal static bool Check(string path, string name)
        {
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                try
                {
                    while (stream.Position < stream.Length)
                    {
                        int skip = reader.ReadInt32();
                        Console.WriteLine($"Bytes para saltar: {skip}");

                        string currentName = reader.ReadString().Trim();
                        Console.WriteLine($"Nombre leido: {currentName}");

                        if (currentName.Equals(name.Trim(), StringComparison.Ordinal))
                        {
                            int tableContent = reader.ReadInt32();
                            if (tableContent > 0)
                            {
                                return true;
                            }
                            else
                            {
                                stream.Close();
                                return false;
                            }
                        }
                        reader.BaseStream.Seek(skip, SeekOrigin.Current);

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

        internal static void RemoveTable(string path, string TableName)
        {
            int sizeB = 19;
            int nameOffset = 5;
            int nameLength = 14;
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
                            string recordString = Encoding.UTF8.GetString(recordData, nameOffset, nameLength).Trim();
                            if (!recordString.Equals(TableName.Trim(), StringComparison.Ordinal))
                            {
                                writer.Write(recordData);
                            }
                            else
                            {
                                Console.WriteLine($"Registro {TableName} encontrando en posición {recordPosition} y será eliminado");
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
