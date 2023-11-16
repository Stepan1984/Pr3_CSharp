

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Pr2
{
    class Program
    {
        [DllImport("file64.dll")]
        public static extern IntPtr open(string path, bool read);
        [DllImport("file64.dll")]
        public static extern void close(IntPtr file);
        [DllImport("file64.dll")]
        public static extern bool read(IntPtr file, int num, StringBuilder word);
        [DllImport("file64.dll")]
        public static extern void write(IntPtr file, string text);
        [DllImport("file64.dll")]
        public static extern int fileLength(IntPtr file);

        public class File : IDisposable
        {
            public readonly IntPtr path;
            public readonly string sPath;
            private StringBuilder word;
            int length;
            public int Length { get { return length = fileLength(path); } }
            public string Word{ get { return word.ToString(); } }
            public File(string newPath, bool openType) // конструктор
            {
                try 
                {
                    sPath = "C:\\Users\\stepa\\Desktop\\CSharp\\Pr3\\text\\" + newPath;
                    path = open(sPath, openType);
                    word = new StringBuilder(255);
                }
                catch (Exception ex) 
                {
                    throw new Exception("Всё плохо, вырубай");
                    //видимо удалить объект
                }

            }
            ~File() // деструктор
            {
                
                try { close(path); } 
                catch (Exception ex) {}
                
            }

            public unsafe void Dispose() // dispose
            {
                try { close(path); }
                catch (Exception ex) {}
            }

            public bool Read(int num) //чтение слова из файла
            {
                try
                {
                    return read(path, num, word);
                }catch (Exception ex) 
                {
                    throw new Exception("Читать - никак");
                }
            }

            public void Write(string text) // запись строки в файл
            {
                try
                {
                    write(path, text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Писать - никак");
                }
            }
            
        }

        static void Main(string[] args)
        {
            
            int menu;
            string text;
            string stmp = "";
            //bool open = false;
            string filename = "";
            File first = null;
            File second = null;
            Dictionary<string, int> tenWords = new Dictionary<string, int>(); // словарь 
            
            do
            {
                Console.Clear();
                Console.WriteLine("1.Открыть файл");//
                Console.WriteLine("2.Оставить в файле 10 самых частых слов");//
                Console.WriteLine("3.Получить количество слов в файле");//
                Console.WriteLine("4.Заменить каждое четное слово файла на соответствующее из второго файла,\n   заменить каждое нечётное слово из второго файла на соответствующее из первого");
                Console.WriteLine("5.Выход");
                Int32.TryParse(Console.ReadLine(), out menu);
                Console.Clear();
                switch (menu)
                {
                    case 1:// открываем файл
                        first = OpenFile(GetFilename(), true);
                        Exit();
                        break;
                    case 2:
                        text = "";
                        if (first != null)
                        {
                            for (int i = 1; i <= first.Length; i++) // читаем и считаем в словарь 
                            {
                                first.Read(i);
                                if (tenWords.ContainsKey(first.Word))
                                    tenWords[first.Word]++;
                                else if (tenWords.Count() < 10)
                                    tenWords.Add(first.Word, 1);
                            }
                            if (tenWords.Count != 0)
                            {
                                filename = first.sPath;
                                first.Dispose();
                                first = null;
                                first = OpenFile(filename, false); // откываем для записи
                                if (first != null)
                                {
                                    Dictionary<string, int> sortedDict = tenWords.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); // сортируем словарь по значению
                                    sortedDict.Reverse(); // разворачиваем словарь
                                    while (first.Length < 10 || sortedDict.Count != 0) // пока в файле меньше 10 слов или список не пуст
                                    {
                                        stmp = sortedDict.Keys.ElementAt(1); // получаем элемент
                                        sortedDict.Remove(stmp); // удаляем запись из словаря
                                        text += stmp + " "; // добавляем слово в строку, которую потом запишем в файл
                                    }
                                    first.Write(text);
                                }
                            }
                            first.Dispose();
                            first = null;
                        }
                        Console.WriteLine("Работа выполнена");
                        Exit();
                        break;
                    case 3:
                        first = OpenFile(GetFilename(), true);
                        if (first != null) 
                        {
                            Console.WriteLine("Количество слов в файле: {0}", first.Length);
                        }
                        Exit();
                        break;
                    case 4: // удалить элемент по индексу
                        Console.WriteLine("Затычка");
                        Exit();
                        break;
                }
            }
            while (menu != 5);

        }

        public static string GetFilename()
        {
            string filename;
            do
            {
                Console.Clear();
                Console.WriteLine("Введите имя обрабатываемого файла:");
                filename = Console.ReadLine();
            } while (filename == null);
            return filename;
        }

        public static bool GetOpenType() 
        {
            bool type;
            string answ;
            do
            {
                Console.Clear();
                Console.WriteLine("Выберите тип открытия файла:");
                answ = Console.ReadLine();
            } while (answ != "r" && answ != "R" && answ != "W" && answ != "w");
            if(answ == "r" || answ == "R")
                type = true;
            else
                type = false;
            return type;
        }

        public static File OpenFile(string filename, bool type)
        {
            File file;
            try
            {
                file = new File(filename, type);
                //open = true;
                Console.WriteLine("Файл открыт");
                return file;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Console.WriteLine("Ошибка открытия файла");
                return null;
            }
        }


        public static int GetIndex(int length)
        {
            int index;
            do
            {
                Console.Clear();
                Console.WriteLine("Введите номер элемента в списке: ");
            } while (!Int32.TryParse(Console.ReadLine(), out index) || index < 1 || index > length);
            return --index;
        }

        public static void Exit()
        {
            Console.WriteLine("Для возврата в меню нажмите любую кнопку (кроме отключения питания устройства)");
            Console.ReadKey();
        }
    }
}
