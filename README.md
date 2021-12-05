~~Файловый менеджер
 "Курсовой проект по курсу "Введение в C#"

~~Интерфейс программы
Интерфейс разделен на три части: 1 - дерево с файлами и директориями, 2 - информация о директории или файле, 3 - командная строка

~~Использование программы
При первом запуске в "ТЕКУЩАЯ ДИРЕКТОРИЯ" и в "ИНФОРМАЦИЯ О ДИРЕКТОРИИ" отображается системная папка, а также информация о ней.
Изначально пользователю доступно взаимодействие именно с этой частью интерфейса, при помощи стрелок вверх и вниз, можно передвигаться по каталогу.
По нажатию на клавишу Enter активной становиться командная строка, и можно начинать вводить команды, при нажатии на клавишу Enter команда выполняется.
После выполнения команд, для возвращения к командной строке, следует нажать клавишу Enter.
В файл errors_name_exception.txt - сохраняются обработанные исключения. В файл settings.dat - сохраняются найстройки программы.

~~Команды

 - `cd nameDirectory` - вывод дерева с файлами каталогами, nameDirectory - абсолютный путь к каталогу
						(путь необходимо прописывать с учетом регистра);

 - `cp * nameDirectory` - копирование текущей директории (т.е. к которой пользователь обращался командой ls), nameDirectory - абсолютный путь к каталогу, в который необходимо скопировать директорию;
 - `cp *f nameDirectory` - копирование текущего файла (т.е  к которой пользователь обращался командой file), nameDirectory - абсолютный путь к каталогу, в который необходимо скопировать файл;
 - `cp nameDirectory1 -> nameDirectory2` - копирование каталога в другой каталог, nameDirectory1, nameDirectory2 - абсолютные пути к каталогам;
 - `cp nameFile -> nameDirectory` - копирование файла в каталог nameFile, nameDirectory - абсолютные пути rm *- удалить текущую директорию;
 - `rm *` - удалить текущий каталог (т.е. к которой пользователь обращался командой ls);
 - `rm *f` - удалить текущий файл (т.е  к которой пользователь обращался командой file);
 - `rm nameFile` - удалить файл, nameFile - абсолютный путь к файлу (для удаления файла, необходимо обратиться к другому файлу);
 - `rm nameDir` - удалить каталог, nameDir - абсолютный путь к файлу;
 - `file nameFile` - вывод информации о файле, nameFile - абсолютный путь к файлу;
 - `file * nameFile` - вывод информации о файле из текущей директории, nameFile - относительный путь к файлу;
 - `ls nameFile.extension` - создание файла nameFile.extension - абсолютный путь к файлу;