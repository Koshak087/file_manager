using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileManager
{

    class Program
    {
        // поля для вывода каталога
        public static int CountLevel { get; set; }

        public static string DirNow { get; set; }
        public static string FileNow { get; set; }
        public static List<string> FileDir { get; set; }
        // размер директории
        public static long SizeDir { get; set; }
        // проверка на директорию возвращает True - если это директорию False - если это файл
        public static bool IsDirectory(FileSystemInfo fsItem)
        {
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
        // расчет размера каталога
        public static long DirSize(DirectoryInfo di)
        {
            long size = 0;
            FileInfo[] Inf = null;
            try
            {
                Inf = di.GetFiles();
            }
            catch (DirectoryNotFoundException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return 0;
            }
            catch (System.UnauthorizedAccessException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return 0;
            }
            foreach (FileInfo fi in Inf)
            {
                size += fi.Length;
            }
            DirectoryInfo[] dis = di.GetDirectories();
            foreach (DirectoryInfo d in dis)
            {
                size += DirSize(d);
            }
            return size;
        }


        // Вывод страницы
        public static void OutputPage(int minCountFile, int maxCountFile, DirectoryInfo file)
        {
            //выводим надпапку
            int j = minCountFile;
            while (FileDir[j].Contains("║   ╠══"))
            {
                j--;
            }
            if (minCountFile != j)
            {
                Console.WriteLine(FileDir[j]);
            }
            for (int i = minCountFile; i < FileDir.Count && i < maxCountFile; i++)
            {
                Console.WriteLine(FileDir[i]);
            }
        }


        public static void Paging(DirectoryInfo file, int index)
        {
            int minCountFile = 0;
            int maxCountFile = 10;
            Console.SetCursorPosition(0, 2);
            for (int i = minCountFile; i < FileDir.Count && i < maxCountFile; i++)
            {
                Console.WriteLine(FileDir[i]);
            }
            if (index == 0)
            {
                OutputInfoDir(file);
            }
            Console.SetCursorPosition(0, 25);
            while (true)
            {
                var consoleKey = Console.ReadKey();

                switch (consoleKey.Key)
                {
                    case ConsoleKey.DownArrow:
                        minCountFile += 10;
                        maxCountFile += 10;
                        if (minCountFile < FileDir.Count)
                        {
                            ClearLine(3, 12);
                            Console.SetCursorPosition(0, 2);
                            OutputPage(minCountFile, maxCountFile, file);
                            Console.SetCursorPosition(0, 25);
                        }
                        else
                        {
                            minCountFile -= 10;
                            maxCountFile -= 10;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        minCountFile -= 10;
                        maxCountFile -= 10;

                        if (minCountFile >= 0)
                        {
                            ClearLine(3, 12);
                            Console.SetCursorPosition(0, 2);
                            OutputPage(minCountFile, maxCountFile, file);
                            Console.SetCursorPosition(0, 25);
                        }
                        else
                        {
                            minCountFile += 10;
                            maxCountFile += 10;
                        }
                        break;
                    case ConsoleKey.Enter:
                        ClearLine(25, 5);
                        Console.SetCursorPosition(0, 25);
                        return;
                }
            }
        }
        //очистить строку в консоли
        public static void ClearLine(int line, int count)
        {
            for (int i = line; i < line + count; i++)
            {
                Console.MoveBufferArea(0, i, Console.BufferWidth, 1, Console.BufferWidth, i, ' ', Console.ForegroundColor, Console.BackgroundColor);
            }
        }

        //Вывод информации о файле
        public static void OutputInfoFile(string filename)
        {
            var fileInfo = new FileInfo(filename);
            Console.SetCursorPosition(0, 16);
            Console.WriteLine($"Имя файла: {fileInfo.FullName}");
            Console.WriteLine($"Расширение: {fileInfo.Extension}");
            Console.WriteLine($"Размер (в Mb): {fileInfo.Length / 1024 / 1024}");
            Console.WriteLine($"Атрибуты: {fileInfo.Attributes}");
            Console.WriteLine($"Время создания: {fileInfo.CreationTime}");
            Console.WriteLine($"Время изменения: {fileInfo.LastWriteTime}");
            if (fileInfo.IsReadOnly == true)
            {
                Console.WriteLine($"Доступен только для чтения: да");
            }
            else
            {
                Console.WriteLine($"Доступен только для чтения: нет");
            }
        }

        //Вывод информации о директории
        public static void OutputInfoDir(DirectoryInfo di)
        {
            Console.SetCursorPosition(0, 15);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("ИНФОРМАЦИЯ О ДИРЕКТОРИИ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═════════════════════════════════════════════════");
            Console.WriteLine($"Имя каталога: {di.FullName}");
            Console.WriteLine($"Размер (в Mb): {DirSize(di) / 1024 / 1024}");
            Console.WriteLine($"Атрибуты: {di.Attributes}");
            Console.WriteLine($"Время создания: {di.CreationTime}");
            Console.WriteLine($"Время изменения: {di.LastWriteTime}");
            Console.SetCursorPosition(0, 24);
            Console.Write("═══════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("КОМАНДНАЯ СТРОКА");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═════════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 25);
        }


        // Создание страницы из всех папок и подпапок директории
        public static void ShowFileDir(string filename, string prefix = " ")
        {
            var di = new DirectoryInfo(filename);
            List<FileSystemInfo> fItem = null;
            try
            {
                fItem = di.GetFileSystemInfos().ToList();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return;
            }
            catch (DirectoryNotFoundException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return;
            }

            for (int i = 0; i < fItem.Count; i++)
            {
                FileDir.Add($"{prefix}╠══ {fItem[i].Name}");

                if (IsDirectory(fItem[i]) && CountLevel != 1)
                {
                    CountLevel = 1;
                    ShowFileDir(fItem[i].FullName, prefix + "║   ");
                }
            }
            CountLevel = 0;
        }

        // копирование директории
        public static void DirCopy(string sourcePath, string destinationPath)
        {
            var di = new DirectoryInfo(sourcePath);

            Directory.CreateDirectory(destinationPath + "\\" + di.Name);
            List<FileSystemInfo> fItem = null;
            try
            {
                fItem = di.GetFileSystemInfos().ToList();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return;
            }
            catch (DirectoryNotFoundException ex)
            {
                File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                return;
            }

            for (int i = 0; i < fItem.Count; i++)
            {

                if (Directory.Exists(fItem[i].FullName))
                {
                    DirCopy(fItem[i].FullName, destinationPath + "\\" + di.Name);
                }
                else
                {
                    File.Create(destinationPath + "\\" + di.Name + "\\" + fItem[i].Name);
                    try
                    {
                        File.Copy(fItem[i].FullName, destinationPath + "\\" + di.Name + "\\" + fItem[i].Name, true);
                    }
                    catch (System.IO.IOException ex)
                    {
                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                    }
                }
            }

        }

        static void Main(string[] args)
        {
            Console.Title = "Файловый менеджер";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("═══════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("ТЕКУЩАЯ ДИРЕКТОРИЯ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("══════════════════════════════════════════════════");

            string filename = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            BinaryFormatter formatter = new BinaryFormatter();
            // считываем директорию к которой последний раз обращались
            using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
            {
                try
                {
                    string str = ((string)formatter.Deserialize(fs));
                    if (str != null && Directory.Exists(str.Trim()))
                    {
                        filename = String.Empty;
                        filename = str;
                    }
                }
                catch
                {

                }
            }

            DirNow = filename;
            using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, DirNow);
            }

            var file = new DirectoryInfo(filename);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(file);
            Console.ForegroundColor = ConsoleColor.White;
            FileDir = new List<string>();
            ShowFileDir(filename);
            Paging(file, 0);
            while (true)
            {
                int cmd = 0;
                string com = Console.ReadLine();
                if (com.Contains("cd"))
                    cmd = 1;
                else if (com.Contains("cp"))
                    cmd = 2;
                else if (com.Contains("rm"))
                    cmd = 3;
                else if (com.Contains("file"))
                    cmd = 4;
                else if (com.Contains("ls"))
                    cmd = 5;
                else
                {
                    ClearLine(25, 5);
                    Console.SetCursorPosition(0, 25);
                    Console.WriteLine("Команда не найдена");
                    Console.SetCursorPosition(0, 26);
                }


                switch (cmd)
                {
                    case 1:
                        {
                            com.Trim();
                            com = com.Remove(0, 2);
                            if (Directory.Exists(com.Trim()))
                            {
                                DirNow = null;
                                DirNow = com.Trim();
                                // записываем директорию к которой последний раз обратились
                                using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
                                {
                                    formatter.Serialize(fs, DirNow);
                                }

                                var di = new DirectoryInfo(com.Trim());

                                ClearLine(1, 14);
                                ClearLine(25, 5);
                                Console.SetCursorPosition(0, 1);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(di);
                                Console.ForegroundColor = ConsoleColor.White;
                                FileDir.Clear();
                                FileDir = new List<string>();
                                ShowFileDir(com.Trim());
                                Paging(di, 0);
                            }
                            else
                            {
                                ClearLine(25, 5);
                                Console.SetCursorPosition(0, 25);
                                Console.WriteLine("Директория не существует");
                                Console.SetCursorPosition(0, 26);
                            }

                        };
                        break;

                    case 2:
                        {
                            com = com.Trim();
                            com = com.Remove(0, 2);
                            com = com.Trim();

                            if (com.Length > 1 && com.Substring(0, 2) == "*f")
                            {
                                if (File.Exists(FileNow))
                                {
                                    string destDir = com.Remove(0, 2).Trim();
                                    string sourceDir = FileNow;
                                    var path = new FileInfo(sourceDir).Name;
                                    try
                                    {
                                        File.Copy(sourceDir, destDir + "\\" + path);
                                    }
                                    catch (System.IO.IOException ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Не удалось скопировать файл");
                                        Console.SetCursorPosition(0, 26);
                                    }
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine($"Файл скопирован и находится по пути: {destDir}");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файл не существует");
                                    Console.SetCursorPosition(0, 26);
                                }
                            }
                            else if (com.Length > 1 && com[0] == '*')
                            {
                                if (Directory.Exists(DirNow))
                                {
                                    string sourceDir = DirNow;
                                    var path = new DirectoryInfo(sourceDir).Name;
                                    string destDir = com.Remove(0, 1).Trim();

                                    DirCopy(sourceDir, destDir);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine($"Директория скопирована и находится по пути: {destDir + "\\" + path}");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Директория не существует");
                                    Console.SetCursorPosition(0, 26);
                                }
                            }
                            else if (com.Contains("->"))
                            {
                                int index = com.IndexOf("->");
                                string sourceDir = com.Substring(0, index);
                                string destDir = com.Substring(index + 3);
                                if (Directory.Exists(sourceDir))
                                {
                                    var path = new DirectoryInfo(sourceDir).Name;
                                    DirCopy(sourceDir, destDir);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine($"Директория скопирована и находится по пути: {destDir + "\\" + path}");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else if (File.Exists(sourceDir))
                                {
                                    var path = new FileInfo(sourceDir).Name;
                                    try
                                    {
                                        File.Copy(sourceDir, destDir + "\\" + path);
                                    }
                                    catch (System.IO.IOException ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Не удалось скопировать файл");
                                        Console.SetCursorPosition(0, 26);
                                    }
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine($"Файл скопирован и находится по пути: {destDir}");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine($"Директория или файл не найдены по адресу: {sourceDir}");
                                    Console.SetCursorPosition(0, 26);
                                }
                            }
                            else
                            {
                                ClearLine(25, 5);
                                Console.SetCursorPosition(0, 25);
                                Console.WriteLine("Команда не найдена");
                                Console.SetCursorPosition(0, 26);
                            }


                        };
                        break;
                    case 3:
                        {
                            com = com.Trim();
                            com = com.Remove(0, 2);
                            com = com.Trim();

                            if (com[0].ToString() + com[1].ToString() == "*f")
                            {
                                com = com.Remove(0, 2);
                                if (File.Exists(FileNow))
                                {
                                    try
                                    {
                                        File.Delete(FileNow);
                                    }
                                    catch (Exception ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Не удалось удалить файл");
                                        Console.SetCursorPosition(0, 26);
                                        break;
                                    }
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файл удален");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файла по предыдущему пути не существует");
                                    Console.SetCursorPosition(0, 26);
                                }
                            }
                            else if (com[0] == '*')
                            {
                                com = com.Remove(0, 1);
                                if (Directory.Exists(DirNow))
                                {
                                    try
                                    {
                                        Directory.Delete(DirNow, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Не удалось удалить директорию");
                                        Console.SetCursorPosition(0, 26);
                                        break;
                                    }
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Директория удален");
                                    Console.SetCursorPosition(0, 26);
                                }
                                else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Директории по предыдущему пути не существует");
                                    Console.SetCursorPosition(0, 26);

                                }
                            }
                            else
                            {
                                try
                                {
                                    if (Directory.Exists(com))
                                {
                                    Directory.Delete(com, true);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Директория удален");
                                    Console.SetCursorPosition(0, 26);
                                }
                                    else if (File.Exists(com))
                                {
                                    File.Delete(com);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файл удален");
                                    Console.SetCursorPosition(0, 26);
                                }
                                    else
                                {
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файла или директории по такому пути не существует");
                                    Console.SetCursorPosition(0, 26);
                                }
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                    File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Объект используется другим приложением");
                                    Console.SetCursorPosition(0, 26);
                                    break;
                                }
                            }

                        };
                        break;

                    case 4:
                        {
                            com = com.Trim();
                            com = com.Remove(0, 4);
                            com = com.Trim();
                            if (com != "")
                            {
                                if (com[0] == '*')
                                {
                                    com = com.Remove(0, 1);
                                    try { if (File.Exists(DirNow + "\\" + com.Trim()))
                                    {
                                        var di = new DirectoryInfo(DirNow);
                                        ClearLine(16, 7);
                                        FileNow = String.Empty;
                                        FileNow = DirNow + "\\" + com.Trim();
                                        // записываем директорию к которой последний раз обратились
                                        using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
                                        {
                                            formatter.Serialize(fs, DirNow);
                                        }

                                        OutputInfoFile(DirNow + "\\" + com.Trim());
                                        Paging(di, 1);
                                    }
                                    else
                                    {
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Файл не найден");
                                        Console.SetCursorPosition(0, 26);
                                    }
                                    }
                                    catch (Exception ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Непредвиденная ошибка");
                                        Console.SetCursorPosition(0, 26);
                                        break;
                                    }

                                }
                                else
                                {
                                    try
                                    {
                                        if (File.Exists(com.Trim()))
                                        {
                                            ClearLine(16, 7);
                                            OutputInfoFile(com.Trim());
                                            FileNow = String.Empty;
                                            FileNow = com.Trim();
                                            // записываем директорию к которой последний раз обратились
                                            using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
                                            {
                                                formatter.Serialize(fs, DirNow);
                                            }

                                            com = com.Substring(0, com.LastIndexOf('\\'));
                                            var di = new DirectoryInfo(com);
                                            ClearLine(1, 14);
                                            ClearLine(25, 5);
                                            Console.SetCursorPosition(0, 1);
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine(di);
                                            Console.ForegroundColor = ConsoleColor.White;
                                            FileDir.Clear();
                                            FileDir = new List<string>();
                                            DirNow = com;
                                            ShowFileDir(com);
                                            Paging(di, 1);
                                        }
                                        else
                                        {
                                            ClearLine(25, 5);
                                            Console.SetCursorPosition(0, 25);
                                            Console.WriteLine("Файл не найден");
                                            Console.SetCursorPosition(0, 26);
                                        }
                                    }
                                    
                                    catch (Exception ex)
                                    {
                                        File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                        File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                        ClearLine(25, 5);
                                        Console.SetCursorPosition(0, 25);
                                        Console.WriteLine("Непредвиденная ошибка");
                                        Console.SetCursorPosition(0, 26);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ClearLine(25, 5);
                                Console.SetCursorPosition(0, 25);
                                Console.WriteLine("Укажите файл");
                                Console.SetCursorPosition(0, 26);
                            };
                        };
                        break;

                    case 5:
                        {
                            com = com.Remove(0, 2);
                            com = com.Trim();
                            if (File.Exists(com))
                            {
                                try
                                {
                                    File.Create(com);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                    File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Файл с таким именем существует");
                                    Console.SetCursorPosition(0, 26);
                                    break;
                                }
                            }
                            else
                            {
                                try
                                {
                                    File.Create(com);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText("errors_name_exception.txt", $"{DateTime.Now} Сообщение об ошибке: {ex.Message}");
                                    File.AppendAllText("errors_name_exception.txt", Environment.NewLine);
                                    ClearLine(25, 5);
                                    Console.SetCursorPosition(0, 25);
                                    Console.WriteLine("Не удалось создать файл");
                                    Console.SetCursorPosition(0, 26);
                                    break;
                                }
                                ClearLine(25, 5);
                                Console.SetCursorPosition(0, 25);
                                Console.WriteLine("Файл создан");
                                Console.SetCursorPosition(0, 26);
                            }

                        }
                        break;

                }
            }
        }
    }
}
