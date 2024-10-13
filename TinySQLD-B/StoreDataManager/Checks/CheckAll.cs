using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataManager.Checks
{
    internal class CheckAll
    {
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
    }
}
