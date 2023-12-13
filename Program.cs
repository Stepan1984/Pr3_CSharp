

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
        public static extern int length(IntPtr file);

        public class File : IDisposable
        {
            public readonly IntPtr path;
            public readonly string sPath;
            private StringBuilder word;
            int length;
            public int Length { get { return length = length(path); } }
            public string Word{ get { return word.ToString(); } }
            public File(string newPath, bool openType) // конструктор
            {
                try
                {
                    sPath = /*"C:\\Users\\stepa\\Desktop\\CSharp\\Pr3\\text\\" + */  newPath;
                    path = open(sPath, openType);
                    word = new StringBuilder(255);
                }
                catch (SEHException ex) 
                {
                    //
                    //
                    //Console.WriteLine(ex.ToString());
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
            string text2;
            string stmp = "";
            //bool open = false;
            string filename = "";
            string filename2 = "";
            int prevLength;
            int prevLength2;
            int minLength;
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
                        if (first != null)
                            first.Dispose();
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
                                        
                                    tenWords = tenWords.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); // сортируем словарь по значению
                                    tenWords.Reverse(); // разворачиваем словарь
                                    prevLength = tenWords.Count - 10;
                                    while (tenWords.Count != 0 && tenWords.Count != prevLength) // пока в файле меньше 10 слов или список не пуст
                                    {
                                        stmp = tenWords.Keys.ElementAt(0); // получаем элемент
                                        tenWords.Remove(stmp); // удаляем запись из словаря
                                        text += stmp + " "; // добавляем слово в строку, которую потом запишем в файл
                                    }
                                    text = text.Trim();
                                    first.Write(text);
                                    filename = first.sPath;
                                    first.Dispose();
                                    first = OpenFile(filename, true);
                                }
                            }
                            Console.WriteLine("Работа выполнена");
                        }
                        else
                            Console.WriteLine("Откройте файл");
                        
                        Exit();
                        break;
                    case 3:
                        if (first != null)
                        {
                            Console.WriteLine("Количество слов в файле: {0}", first.Length);
                        }
                        else
                            Console.WriteLine("Откройте файл");
                        Exit();
                        break;
                    case 4: // удалить элемент по индексу
                        if (first != null) 
                        {
                            if (second == null) 
                            {
                                Console.WriteLine("Второй файл");
                                second = OpenFile(GetFilename(), true);
                            }
                            if(second!= null) 
                            {
                                text = text2 = "";
                                filename = first.sPath;
                                prevLength = first.Length;
                                filename2 = second.sPath;
                                prevLength2 = second.Length;
                                minLength = prevLength > prevLength2? prevLength2 : prevLength;
                                for(int i = 1; i <= prevLength;i++)
                                {
                                    if (i%2==0) // если чётное слово , то надо его заменить на соответствующее чётное из второго файла
                                    {
                                        if (i > prevLength2) // если индекс слова больше чем длина второго файла
                                        {
                                            second.Read(i - prevLength2 * (i / prevLength2));// 290 - 30* 290/30
                                            text += second.Word  + " ";
                                        }
                                        else 
                                        {
                                            second.Read(i);
                                            text += second.Word + " ";
                                        }
                                            
                                    }
                                    else 
                                    {
                                        first.Read(i);
                                        text += first.Word + " ";
                                    }
                                }

                                for (int i = 1; i <= prevLength2; i++)
                                {
                                    if (i % 2 == 0) // если чётное 
                                    {
                                        second.Read(i);
                                        text2 += second.Word + " ";
                                    }
                                    else // если нечётное
                                    {
                                        
                                        if (i > prevLength)
                                        {
                                            first.Read(i - prevLength * (i / prevLength));
                                            text2 += first.Word + " ";
                                        }
                                        else
                                        {
                                            first.Read(i);
                                            text2 += first.Word + " ";
                                        }
                                    }
                                }

                                first.Dispose();
                                second.Dispose();
                                first = OpenFile(filename, false);
                                second = OpenFile(filename2, false);
                                text = text.Trim();
                                text2 = text2.Trim();
                                first.Write(text);
                                second.Write(text2);
                                first.Dispose();
                                second.Dispose();
                                second = null;
                                first = OpenFile(filename, true);
                            }
                        }
                        else 
                        {
                            Console.Write("Откройте файл\n");
                        }
                        Exit();
                        break;
                }
            }
            while (menu != 5);

            if(first != null)
            first.Dispose();
            if (second != null)
                second.Dispose();

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
