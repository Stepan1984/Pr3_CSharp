

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
            readonly IntPtr path;
            private StringBuilder word;
            int length;
            public int Length { get { return length; } }
            public string Word{ get { return word.ToString(); } }
            public File(string newPath, bool openType) // конструктор
            {
                try 
                {
                    path = open(newPath, openType);
                    word = new StringBuilder(255);
                }
                catch (Exception ex) 
                {
                    throw ex;
                    //видимо удалить объект
                }
                length = fileLength(path);

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
                    throw ex;
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
                    throw ex;
                }
            }
            
        }
        static void Main(string[] args)
        {
            List<File> files = new List<File>();
            int menu;
            do
            {
                Console.Clear();
                Console.WriteLine("1.Открыть файл");//
                Console.WriteLine("2.Оставить в файле 10 самых частых слов");
                Console.WriteLine("3.Получить количество слов в файле");//
                Console.WriteLine("4.Заменить каждое четное слово файла на соответствующее из второго файла,\n   заменить каждое нечётное слово из второго файла на соответствующее из первого");
                Console.WriteLine("5.Выход");
                Int32.TryParse(Console.ReadLine(), out menu);
                Console.Clear();
                switch (menu)
                {
                    case 1:
                        try
                        {
                            files.Push(new File(GetFilename()));
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine("Ошибка открытия файлов");
                            fileList.ForEach(file => file.Dispose());
                            break;
                        }

                        break;
                    case 2:
                        length = stack.Count;
                        if (length > 0)
                        {
                            Stack<Plant> rev = new Stack<Plant>();
                            while (stack.Count != 0)
                                rev.Push(stack.Pop());
                            stack.Clear();
                            stack = rev;
                            Console.WriteLine("Записали в обратном порядке");
                        }
                        else Console.WriteLine("Стек пустой");
                        Exit();
                        break;
                    case 3:
                        fileList.ForEach(file => Console.WriteLine(file.GetLength()));
                        Exit();
                        break;
                    case 4: // удалить элемент по индексу
                        length = stack.Count;
                        if (length > 0)
                        {
                            int index = GetIndex(length);
                            if (index == 0)
                                stack.Pop();
                            else
                            {
                                
                                int j = 0;
                                while (j++ != index)
                                    tmpStack.Push(stack.Pop());
                                Console.WriteLine(stack.Pop());
                                while (tmpStack.Count > 0)
                                    stack.Push(tmpStack.Pop());
                            }
                            Console.WriteLine("Элемент успешно удалён");
                        }
                        else
                        {
                            Console.WriteLine("Стек пустой");
                        }
                        Exit();
                        break;
                }
            }
            while (menu != 5);

        }

        public static int LoadLib()
        {
            
            return 0;
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

        public static int ReadFile(string filename, ref Stack<Plant> stack)
        {
            try
            {
                using (FileStream fstream = new FileStream(filename, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fstream))
                    {
                        string name;
                        int month;
                        int seedsAmount;
                        double price;
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            name = reader.ReadString();
                            month = reader.ReadInt32();
                            seedsAmount = reader.ReadInt32();
                            price = reader.ReadDouble();
                            Plant item = new Plant(name, month, seedsAmount, price);
                            stack.Push(item);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Такого файла нет, но мы его создали");
            }
            catch (System.ArgumentException)
            {
                return -1;
            }
            Exit();
            return 0;
        }

        public static void WriteFile(string filename, ref Stack<Plant> stack)
        {
            using (FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter writer = new BinaryWriter(fstream))
                {
                    foreach (Plant plant in stack)
                    {
                        writer.Write(plant.GetName());
                        writer.Write(plant.GetMonth());
                        writer.Write(plant.GetSeedsAmount());
                        writer.Write(plant.GetPrice());
                    }
                }
            }
            Console.WriteLine("Стек успешно сохранён");
            Exit();
        }

        public static void Show(ref Stack<Plant> stack)
        {
            //ConsoleKeyInfo action;
            Console.Clear();
            Console.WriteLine(" № |Название|  месяц  |кол-во семян| цена "); // 3|8|9|12|6
            int i = 0;

            foreach (var item in stack)
            {
                Console.WriteLine("{0,3}|{1}", ++i, item);
            }
            Console.WriteLine("Enter - для возврата в меню");
            //action = 

            while ((Console.ReadKey()).Key != ConsoleKey.Enter) ;
        }
        public static Plant CreateNewPlant()
        {
            string name;
            int seedsAmount, month;
            double price;

            do
            {
                Console.Clear();
                Console.WriteLine("Введите название растения:");
                name = Console.ReadLine();
            } while (name == null || name.Length == 0);

            do
            {
                Console.Clear();
                Console.WriteLine("Введите месяц посадки (номер месяца ):");
            } while (!Int32.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12);

            do
            {
                Console.Clear();
                Console.WriteLine("Введите количество семян в упаковке:");
            } while (!Int32.TryParse(Console.ReadLine(), out seedsAmount) || seedsAmount < 1);

            do
            {
                Console.Clear();
                Console.WriteLine("Введите цену упаковки:");
            } while (!Double.TryParse(Console.ReadLine(), out price) || price < EPS);

            return new Plant(name, --month, seedsAmount, price);
        }

        public static Plant ChangeElement(Plant plant)
        {
            int menu;
            string name;
            int seedsAmount, month;
            double price;
            do
            {
                Console.Clear();
                Console.WriteLine("1.Изменить название");
                Console.WriteLine("2.Изменить месяц посадки");
                Console.WriteLine("3.Изменить количество семян в упаковке");
                Console.WriteLine("4.Изменить цену");
                Console.WriteLine("5.Сохранить и вернуться в меню");
                Int32.TryParse(Console.ReadLine(), out menu);
                Console.Clear();
                switch (menu)
                {
                    case 1:
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Введите новое название растения:");
                            name = Console.ReadLine();
                        } while (name == null || name.Length == 0);
                        plant.SetName(name);
                        Exit();
                        break;
                    case 2:
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Введите новый месяц посадки (номер месяца):");
                        } while (!Int32.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12);
                        plant.SetMonth(--month);
                        Exit();
                        break;
                    case 3:
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Введите новое количество семян в упаковке:");
                        } while (!Int32.TryParse(Console.ReadLine(), out seedsAmount) || seedsAmount < 1);
                        plant.SetSeedsAmount(seedsAmount);
                        Exit();
                        break;
                    case 4:
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Введите новую цену упаковки:");
                        } while (!Double.TryParse(Console.ReadLine(), out price) || price < EPS);
                        plant.SetPrice(price);
                        Exit();
                        break;
                }
            } while (menu != 5);
            return plant;
        }

        public static Plant ChangePrice(Plant plant)
        {
            double price;
            do
            {
                Console.Clear();
                Console.WriteLine("Введите новую цену упаковки:");
            } while (!Double.TryParse(Console.ReadLine(), out price) || price < EPS);
            plant.SetPrice(price);
            return plant;
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
