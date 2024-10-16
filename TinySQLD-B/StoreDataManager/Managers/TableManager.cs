﻿using System;
using System.IO;
using Entities;

namespace StoreDataManager
{
    public class TableManager
    {
        private const string DataPath = @"C:\TinySql\Data";
        private const string SystemTablesFile = @"C:\TinySql\Data\SystemCatalog\SystemTables.Table";
        private int idTB = 0;

        public OperationStatus CreateTable(string nameDB, string nameTable, params (object value, ColumnType type)[] fields)
        {
            // Verifica si hay una base de datos establecida
            if (string.IsNullOrEmpty(nameDB))
            {
                Console.WriteLine("No hay ninguna base de datos seleccionada.");
                return OperationStatus.Error;
            }

            // Crea la ruta de la tabla dentro de la base de datos seleccionada
            var tablePath = $@"{DataPath}\{nameDB}\{nameTable}.Table";

            // Asegúrate de que la carpeta de la base de datos exista
            if (!Directory.Exists($@"{DataPath}\{nameDB}"))
            {
                Console.WriteLine($"La base de datos '{nameDB}' no existe.");
                return OperationStatus.Error;
            }

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new(stream))
            {
                idTB += 1;
                foreach (var field in fields)
                {
                    WriteField(writer, field);
                }
            }

            // Añadir la tabla al archivo de tablas
            AddTableToSystem(nameTable);
            Console.WriteLine($"Tabla '{nameTable}' creada en la base de datos '{nameDB}'.");
            return OperationStatus.Success;
        }

        private void WriteField(BinaryWriter writer, (object value, ColumnType type) field)
        {
            switch (field.type)
            {
                case ColumnType.Integer:
                    if (field.value is int intValue)
                    {
                        writer.Write(intValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("El valor no coincide con el tipo Integer");
                    }
                    break;
                case ColumnType.DateTime:
                    if (field.value is DateTime dtValue)
                    {
                        writer.Write(dtValue.ToBinary());
                    }
                    else
                    {
                        throw new InvalidOperationException("El valor no coincide con el tipo DateTime.");
                    }
                    break;
                case ColumnType.Double:
                    if (field.value is double doubleValue)
                    {
                        writer.Write(doubleValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("El valor no coincide con el tipo Double.");
                    }
                    break;
                case ColumnType.Varchar:
                    if (field.value is string strValue)
                    {
                        writer.Write(strValue.ToCharArray());
                    }
                    else
                    {
                        throw new InvalidOperationException("El valor no coincide con el tipo Varchar.");
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Tipo no soportado: {field.type}");
            }
        }

        private void AddTableToSystem(string nameTable)
        {
            var TBpath = SystemTablesFile;
            using (FileStream stream = new FileStream(TBpath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                int id = idTB;
                string table = nameTable.PadRight(15);

                writer.Write(id);
                writer.Write(table.ToCharArray());
            }
        }
    }
}
