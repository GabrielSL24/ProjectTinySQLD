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
        //Función para eliminar la tabla
        internal static void RemoveTable(string pathDB,string path, string TableName)
        {
            //Define los tamaños y posiciones para los datos del archivo binario
            int sizeB = 23;                 //tamaño
            int idDtabaseOffset = 0;               //Posición actual
            int idTableOffset = 4;               //Longitud de ID (bytes)
            int nameOffset = 8;             //Posición inicial del nombre
            int nameLength = 15;            //Longitud del nombre (bytes)
            bool recordDelete = false;      
            string tempPath = Path.GetTempFileName();   //Crea un archivo temporal
            try
            {
                //Abre el archivo original para lectura y archivo temporal para escritura
                using (FileStream originalStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(originalStream))
                using (FileStream tempStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer =  new BinaryWriter(tempStream))
                {
                    //Recorre el archivo original hasta llegar al final
                    while (originalStream.Position < originalStream.Length)         
                    {
                        long recordPosition = originalStream.Position;     
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

                            if (!recordString.Equals(TableName.Trim(), StringComparison.Ordinal))
                            {
                                writer.Write(recordData);
                            }
                            else
                            {
                                //Si coinciden los nombres, se intenta eliminar el registro
                                Console.WriteLine($"Registro {TableName} encontrando en posición {recordPosition} y será eliminado");
                                Console.WriteLine($"ID de la base de datos: {idDatabase},, ID de a tabla: {idTable}");
                                if (CheckDatabases.RemoveDatabases(pathDB, idDatabase, TableName))
                                {
                                    recordDelete = true;
                                }
                                else
                                {
                                    writer.Write(recordData);
                                    Console.WriteLine("El archivo no está vacio.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"El registro en posición {recordPosition} es demasiado pequeño o mal formado");
                        }
                    }
                }
                //Si se eliminó un registro, reemplaza el archivo original con el temporal
                if (recordDelete)
                {
                    File.Delete(path);
                    File.Move(tempPath, path);
                    Console.WriteLine($"El registro {TableName} ha sido eliminado fisicamente del archivo");
                }
                else
                {
                    Console.WriteLine("No se eliminó ningún registro.");
                    File.Delete(tempPath);
                }
                
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
