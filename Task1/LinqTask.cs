﻿using System;
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
                .Where(c => c.Orders.Any(o => o.OrderDate != DateTime.MinValue))
                .Select(c => (c, DateOfEntry: c.Orders.Min(o => o.OrderDate)))
                .ToList();
        }

        public static IEnumerable<(Customer customer, DateTime DateOfEntry)> Linq5(IEnumerable<Customer> customers)
        {
            return customers
                .Where(c => c.Orders.Any())
                .Select(c => (c, DateOfEntry: c.Orders.Min(o => o.OrderDate)))
                .OrderBy(c => c.DateOfEntry) // Сортируем по дате первого заказа в возрастающем порядке
                .ToList();
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            var result = customers
                .Where(c => !string.IsNullOrEmpty(c.PostalCode) && c.PostalCode.Any(ch => !char.IsDigit(ch)) ||
                            string.IsNullOrEmpty(c.Region) ||
                            !c.Phone.Contains("(") || !c.Phone.Contains(")"))
                .ToList();

            return result;
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
            var result = products.GroupBy(p =>
            {
                if (p.UnitPrice <= cheap)
                    return cheap;
                if (p.UnitPrice <= middle)
                    return middle;
                return expensive;
            }).Select(g => (category: g.Key, products: g.AsEnumerable()));

            return result;
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
            return uniqueCountries.Aggregate((current, next) => current + next);
        }
    }
}