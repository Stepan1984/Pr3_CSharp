

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

        static List<string> allMonths = new List<string> { "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
        const double EPS = 1e-6;
        public struct Plant
        {
            string name;
            int month;
            int seedsAmount;
            double price;
            public Plant(string n, int m, int s, double p)
            {
                name = n;
                month = m;
                seedsAmount = s;
                price = p;
            }
            public override string ToString()
            {
                return string.Format("{0,8}|{1,9}|{2,12:d}|{3,6:F} ", name, allMonths[month], seedsAmount, price); // 8|9|12|6
            }

            public string GetName() { return name; }
            public int GetMonth() { return month; }
            public int GetSeedsAmount() { return seedsAmount; }
            public double GetPrice() { return price; }

            public void SetName(string newName) { if (newName != null) name = newName; }
            public void SetMonth(int newMonth) { if (newMonth > 0 && newMonth < 13) month = newMonth; }
            public void SetSeedsAmount(int newAmount) { if (newAmount > 0) seedsAmount = newAmount; }
            public void SetPrice(double newPrice) { if (newPrice > 0) price = newPrice; }

        }

        public class File : IDisposable
        {
            IntPtr path;
            File(string newPath, bool openType) 
            {
                try 
                {
                    open(newPath, openType);
                }
                catch (Exception ex) 
                {
                    throw new Exception("370 01111824");
                    //видимо удалить объект
                }
                
            }
            ~File() 
            {
                
                try { close(iPath); } 
                catch (Exception ex) {}
                
            }

            public unsafe void Dispose() 
            {
                try { close(&iPath); }
                catch (Exception ex) { }
            }

            public bool Read() { }
            
        }
        static void Main(string[] args)
        {

            
            
            

            string filename;
            Stack<Plant> stack = new Stack<Plant>(); // создаём стэк
            do
            {
                filename = GetFilename();
            } while (ReadFile(filename, ref stack) < 0); // читаем данные из файла
            int menu;
            int length;
            do
            {
                Console.Clear();
                Console.WriteLine("1.Отобразить содержимое стека");
                Console.WriteLine("2.Записать данные в обратном порядке");
                Console.WriteLine("3.Добавить новый элемент");
                Console.WriteLine("4.Удалить элемент по индексу");
                Console.WriteLine("5.Удалить первый элемент (из вершины стека)");
                Console.WriteLine("6.Корректировать элемент");
                Console.WriteLine("7.Что высаживать весной");
                Console.WriteLine("8.Корректировать цену");
                Console.WriteLine("9.Выход");
                Int32.TryParse(Console.ReadLine(), out menu);
                Console.Clear();
                switch (menu)
                {
                    case 1:
                        //Console.WriteLine("Содержимое стека:");
                        Show(ref stack);
                        break;
                    case 2:
                        //Console.WriteLine("Записали в обратном порядке");
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
                        //Console.WriteLine("Добавляем элемент в начало (в вершину стека)");
                        stack.Push(CreateNewPlant());
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
                                Stack<Plant> tmpStack = new Stack<Plant>();
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
                    case 5://удалить первый элемент
                        Console.WriteLine(stack.Pop());
                        Console.WriteLine("Элемент успешно удалён");
                        Exit();
                        break;
                    case 6: // корректировка эелмента
                        length = stack.Count;
                        if (length > 0)
                        {
                            int index = GetIndex(length);
                            Stack<Plant> tmpStack = new Stack<Plant>();
                            int j = 0;
                            while (j++ != index)
                                tmpStack.Push(stack.Pop());
                            stack.Push(ChangeElement(stack.Pop()));
                            while (tmpStack.Count > 0)
                                stack.Push(tmpStack.Pop());

                            Console.WriteLine("Элемент успешно изменён");
                        }
                        else
                        {
                            Console.WriteLine("Стек пустой");
                        }
                        Exit();
                        break;
                    case 7: // с марта по май
                        Console.WriteLine("Высаживаем весной");
                        Console.WriteLine("{0,3}|{1,-12}", "№", "название");
                        int i = 0;
                        foreach (Plant item in stack)
                        {
                            if (item.GetMonth() > 1 && item.GetMonth() < 5)
                                Console.WriteLine("{0,3}|{1,-12}", ++i, item.GetName());
                        }
                        Exit();
                        break;
                    case 8:// корректировка цены
                        length = stack.Count;
                        if (length > 0)
                        {
                            int index = GetIndex(length);
                            Stack<Plant> tmpStack = new Stack<Plant>();
                            int j = 0;
                            while (j++ != index)
                                tmpStack.Push(stack.Pop());
                            stack.Push(ChangePrice(stack.Pop()));
                            while (tmpStack.Count > 0)
                                stack.Push(tmpStack.Pop());
                        }
                        else
                        {
                            Console.WriteLine("Стек пустой");
                        }
                        Exit();
                        break;
                }
            }
            while (menu != 9);

            WriteFile(filename, ref stack);

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
