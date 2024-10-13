# TinySQL Database System

TinySQL es un sistema de base de datos simple implementado en C#. Permite ejecutar varias operaciones SQL básicas, como crear tablas, insertar datos, realizar consultas `SELECT`, `UPDATE`, `DELETE`, y `CREATE INDEX`. Este proyecto simula un motor de base de datos utilizando archivos para almacenar y procesar datos, con soporte para el lenguaje SQL básico.

## Características principales

- **Comandos SQL soportados:**
  - `CREATE DATABASE <database-name>`
  - `SET DATABASE <database-name>`
  - `CREATE TABLE <table-name> ( <column-name> type nullability constraint )`
  - `INSERT INTO <table-name> ( <values> )`
  - `SELECT * | <columns> FROM <table-name> [WHERE where-statement] [ORDER BY <column> <asc | desc>]`
  - `UPDATE <table-name> SET <column-name> = <new-value> [WHERE where-statement]`
  - `DELETE FROM <table-name> [WHERE where-statement]`
  - `CREATE INDEX <index-name> ON <table-name>(<column-name>) OF TYPE <type>`
  
- **Almacenamiento basado en archivos:**
  - La información de las tablas se guarda como archivos en el sistema de archivos.
  - Los datos y estructuras de tablas se gestionan mediante archivos binarios.

- **Archivo de comandos:**
  - Los archivos de comandos tienen la extension .tinysql
  - Estos archivos separan los comandos linea por linea con ";"

## Estructura del proyecto

- **Entities**: Define las entidades del sistema de base de datos (tablas, columnas, etc.).
- **QueryProcessor**: Procesa y valida las consultas SQL.
- **StoreDataManager**: Gestiona el almacenamiento de datos en el sistema de archivos.
- **Operations**: Ejecuta las operaciones SQL (`CreateTable`, `InsertInto`, `Select`, etc.).

## Ejecución del proyecto

- Corre el servidor ApiInterface.

- Abre el archivo de PowerShell del cliente en PowerShell 7.

- Para comunicarse con el servidor usa el siguiente formato de comando:

```powershell
Execute-MyQuery –Query <Commands-file> -IP "127.0.0.1" -Port 11000

