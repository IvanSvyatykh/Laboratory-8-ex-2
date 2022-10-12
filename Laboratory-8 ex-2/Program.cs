using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Labarotory
{
    public class Bills
    {
        // Создаем класс для хранения информации о произведенных операциях
        public long date;
        public int sum = 0;
        public string transaction;
    }
    class CompareDate : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            var firstBill = (Bills)obj1;
            var secondBill = (Bills)obj2;
            return firstBill.date.CompareTo(secondBill.date);
        }
    }
    public class Programm
    {
        public static void Sort(Array bills, IComparer comparer)
        {
            for (int i = bills.Length - 1; i > 0; i--)
                for (int j = 1; j <= i; j++)
                {
                    object obj1 = bills.GetValue(j - 1);
                    object obj2 = bills.GetValue(j);
                    if (comparer.Compare(obj1, obj2) < 0)
                    {
                        object temporary = bills.GetValue(j);
                        bills.SetValue(bills.GetValue(j - 1), j);
                        bills.SetValue(temporary, j - 1);
                    }
                }
        }
        public static string ChangeDate(string date)
        {
            date = date.Replace(":", "");
            date = date.Replace("-", "");
            date = date.Replace(" ", "");
            return date;
        }
        public static void Distribution(string line, Bills[] bills, int count)
        {
            /*Метод записывает информацию из строки в Bills*/
            /*Удаляем '-' , ':' , ' ', для того чтобы записать год, месяц, дату, время в одно число
             для того чтобы было удобно сортировать элементы класса в зависимости от date*/
            line = ChangeDate(line);
            string[] lines = line.Split('|');
            bills[count] = new Bills();
            if (lines.Length == 3)
            {
                bills[count].date = long.Parse(lines[0]);
                bills[count].sum = int.Parse(lines[1]);
                bills[count].transaction = lines[2];
            }
            else
            {
                // Исключающая ветка созданна для хранения информации в случае revert
                bills[count].date = long.Parse(lines[0]);
                bills[count].transaction = lines[1];
            }
        }
        public static object CountSum(string date, int sum, int length, Bills[] bills)
        {
            bool firstTime = false;
            /*Создаём флаг firstTime который позволяет нам отслеживать было ли встреченно 
                 время введенное пользователем, если да, то флаг ползволяет на реализовать выход 
                 из цикла после встречи новго времени  */
            int lastNumber = 0;
            while (sum > 0)
            {
                // При сумме отрицательной цикл завершается и программа выводит сообщение об ошибке
                if (bills[length - 1].date == long.Parse(date)) firstTime = true;
                else if (firstTime && bills[length - 1].date != long.Parse(date)) break;
                if (bills[length - 1].transaction == "in")
                {
                    sum += bills[length - 1].sum;
                    lastNumber = bills[length - 1].sum;
                }
                if (bills[length - 1].transaction == "out")
                {
                    sum -= bills[length - 1].sum;
                    lastNumber = bills[length - 1].sum * (-1);
                }
                if (bills[length - 1].transaction == "revert") sum -= lastNumber;
                length--;
            }
            if (sum < 0) return "Exception the amount less than 0";
            else return "amount is " + sum;
        }
        static void Main()
        {
            Console.WriteLine("Put your date, pleease use \"- between years , months , days :");
            string date = Console.ReadLine();
            date = ChangeDate(date);
            int length = File.ReadAllLines("C:\\Users\\User\\Desktop\\Лабы ulearn\\Laboratory-8 ex-2\\example2.txt").Length - 1;
            Bills[] bills = new Bills[length];
            int count = 0;
            int sum = 0;
            foreach (string line in File.ReadLines("C:\\Users\\User\\Desktop\\Лабы ulearn\\Laboratory-8 ex-2\\example2.txt"))
            {
                if (sum == 0)
                {
                    /*Так как первая строка в файле содержит начальную сумму,
                    то мы не передаем её в Distribution*/
                    sum = int.Parse(line);
                    continue;
                }
                Distribution(line, bills, count);
                count++;
            }
            Sort(bills, new CompareDate());
            Console.WriteLine(CountSum(date, sum, length, bills));
        }
    }
}