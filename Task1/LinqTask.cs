using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            // Вернуть всех заказчиков, у которых сумма всех заказов превышает указанный лимит.
            return customers.Where(c => c.Orders.Sum(o => o.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            // Вернуть для каждого заказчика список поставщиков, у которых есть тот же город, что и у заказчика.
            return customers.Select(c => (c, suppliers.Where(s => s.City == c.City)));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            // Альтернативное решение для Linq2, используя группировку по городу.
            return customers
                .GroupJoin(
                    suppliers,
                    customer => customer.City,
                    supplier => supplier.City,
                    (customer, matchingSuppliers) => (customer, matchingSuppliers)
                );
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            // Вернуть заказчиков, у которых есть заказы на сумму больше заданного лимита.
            return customers.Where(c => c.Orders.Any(o => o.Total > limit));
        }

        public static IEnumerable<(Customer customer, DateTime DateOfEntry)> Linq4(IEnumerable<Customer> customers)
        {
            return customers
                .Where(c => !string.IsNullOrWhiteSpace(c.PostalCode) && c.PostalCode.Any(char.IsLetter) ||
                            string.IsNullOrEmpty(c.Region) ||
                            !c.Phone.Contains("(") || !c.Phone.Contains(")"))
                .Select(c => (c, DateOfEntry: c.Orders.Any() ? c.Orders.Min(o => o.OrderDate) : DateTime.MinValue))
                .ToList();
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(IEnumerable<Customer> customers)
        {
            return customers
                .Where(c => c.Orders.Any()) // Отфильтровываем заказчиков с пустыми заказами
                .Select(c => (c, dateOfEntry: c.Orders.Min(o => o.OrderDate))); // Выбираем заказчика и дату первого заказа
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            // Вернуть список заказчиков, у которых указан номер региона и он не пустой.
            return customers.Where(c => !string.IsNullOrWhiteSpace(c.Region));
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            // Вернуть список продуктов, сгруппированных по категории и стоимости (UnitsInStock) с отсортированными ценами.
            return products
                .GroupBy(p => p.Category)
                .Select(g => new Linq7CategoryGroup
                {
                    Category = g.Key,
                    UnitsInStockGroup = g
                        .GroupBy(p => p.UnitsInStock)
                        .Select(g2 => new Linq7UnitsInStockGroup
                        {
                            UnitsInStock = g2.Key,
                            Prices = g2.Select(p => p.UnitPrice).OrderBy(price => price)
                        })
                });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            // Вернуть продукты, разделенные на три категории (дешевые, средние и дорогие) по их цене.
            var cheapProducts = products.Where(p => p.UnitPrice <= cheap);
            var middleProducts = products.Where(p => p.UnitPrice > cheap && p.UnitPrice <= middle);
            var expensiveProducts = products.Where(p => p.UnitPrice > middle && p.UnitPrice <= expensive);

            return new[]
            {
                (cheap, cheapProducts),
                (middle, middleProducts),
                (expensive, expensiveProducts)
            };
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
        IEnumerable<Customer> customers
)
        {
            return customers
                .Select(c => (
                    c.City,
                    Income: c.Orders.Sum(o => o.Total),
                    Intensity: c.Orders.Length
                ))
                .GroupBy(c => c.City)
                .Select(g => (
                    g.Key,
                    (int)Math.Round(g.Average(c => c.Income)),
                    (int)Math.Round(g.Average(c => c.Intensity))
                ));
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            // Вернуть строку, содержащую список уникальных стран, из которых поставщики,
            // отсортированных сначала по длине названия, а затем по алфавиту
            var uniqueCountries = suppliers
                .Select(s => s.Country)
                .Distinct()
                .OrderBy(c => c.Length) // Сортировка по длине названия страны
                .ThenBy(c => c); // Затем сортировка по алфавиту
            return string.Join("", uniqueCountries);
        }
    }
}