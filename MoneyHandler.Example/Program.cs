using System;
using MoneyHandler.Extensions;

namespace MoneyHandler.Example
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(String.Format("2 Dolars = {0}", new Money(2, Currency.USD)));
            Console.WriteLine(String.Format("2 Dolars In Euro = {0}", 2m.Dollars().ToEuros()));
            Console.WriteLine(String.Format("2 Dolars In Yeans = {0}", 2m.Dollars().ToJapaneseYens()));
            Console.WriteLine(String.Format("2 Dolars + 3 Euro = {0}", (2m.Dollars() + 3m.Euros()).ToString("{0} {4}")));
        }
    }
}
