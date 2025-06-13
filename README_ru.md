
## SeeSharpLogger

![Static Badge](https://img.shields.io/badge/language-C%23-%2305a630?style=for-the-badge) ![Static Badge](https://img.shields.io/badge/.NET-6.0%2C%207.0%2C%208.0-%23c62ce8?style=for-the-badge)

Система консольного логирования с цветным выводом, поддержкой записи в файл и гибкой конфигурацией.

## 🚀 Приступая к работе

Изучите [примеры](https://github.com/antoha1834/SeeSharpLogger/tree/main/examples) и [документацию](https://github.com/antoha1834/SeeSharpLogger/tree/main/docs), чтобы начать.

## 📦 Установка

**Через NuGet:**
```
dotnet add package SeeSharpLogger
```
**Вручную:**
- Загрузите **.dll** с последнего [релиза](https://github.com/antoha1834/SeeSharpLogger/releases/latest)

## 💡 Возможности

- Несколько уровней логирования: `Unimportant`, `Log`, `Info`, `Success`, `Error`, `Warning`
- Цветной вывод в консоль
- Запись журнала в файл
- Собственная настройка префиксов и цветов

## 🧪 Пример использования

```csharp
// Named source
Log mainLog = new Log("Main");

mainLog.WriteLine("App started successfully", LogState.Success);
mainLog.WriteLine("This is a warning", LogState.Warning);
mainLog.WriteLine("Something went wrong", LogState.Error);

// Static calls
Log.WriteLine("Hi from static call!", "Someone", LogState.Info);

// Change the color of LogState (globally)
Log.SetStateColor(LogState.Info, ConsoleColor.Blue);
Log.WriteLine("Now the color for LogState.Info is blue!", "Update", LogState.Info);
// Change the prefix of LogState (globally)
Log.SetStatePrefix(LogState.Error, "[ERROR] ");
mainLog.WriteLine("Now the errors have become even more scary", LogState.Error);
```

### 🖨️ Вывод

```
[Timestamp] [+] [Main] App started successfully  
[Timestamp] [!] [Main] This is a warning  
[Timestamp] [X] [Main] Something went wrong  
[Timestamp] [i] [Someone] Hi from static call!  
[Timestamp] [i] [Update] Now the color for LogState.Info is blue!  
[Timestamp] [ERROR] [Main] Now the errors have become even more scary
```
