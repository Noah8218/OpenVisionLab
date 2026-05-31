using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.Models
{
    public class SalesAnalysis
    {
        /// <summary>
        /// This class was created for testing purposes only,
        /// the data may be inaccurate and meaningless.
        /// </summary>
        /// 
        private SalesOrder order;
        public struct SalesRevenue
        {
            public string DateOrTime { get; set; }
            public double Revenue { get; set; }
        }
        public struct SalesByCategory
        {
            public string Item { get; set; }
            public double UnitSold { get; set; }
        }
        public struct BestProduct
        {
            public string Item { get; set; }
            public double UnitSold { get; set; }
        }
        public struct NumberSales
        {
            public string DateOrTime { get; set; }
            public double UnitSold { get; set; }
        }

        public int NumberCustomers { get; private set; }
        public int TotalNumberSales { get; private set; }
        public double Revenue { get; private set; }
        public double Profit { get; private set; }
        public double Cost { get; private set; }

        public List<SalesRevenue> SalesRevenueList { get; private set; }
        public List<SalesByCategory> SalesByCategoryList { get; private set; }
        public List<BestProduct> Top5ProductsList { get; private set; }
        public List<NumberSales> NumberSalesList { get; private set; }

        public SalesAnalysis()
        {
            order = new SalesOrder();
            GetTotalNumberSales();
            GetRevenue();
            GetCost();
            GetProfit();
            GetCustomers();
            GetSalesRevenue();
            GetNumberSales();
            GetSalesByCategory();
            GetTop5Products();

        }

        private void GetCustomers()
        {
            NumberCustomers = new Customer().GetCustomerList().Count;
        }
        private void GetTotalNumberSales()
        {
            TotalNumberSales = order.GetSalesList().Count;
        }
        private void GetRevenue()
        {
            Revenue = Math.Round(order.GetSalesList().Sum(sale => sale.TotalRevenue), 2);
        }
        private void GetCost()
        {
            Cost = Math.Round(Revenue - (Revenue * 0.3), 2); //This is an assumption that the profit is 30% of the cost
        }
        private void GetProfit()
        {
            Profit = Math.Round(Revenue - Cost, 2);
        }

        private void GetSalesRevenue()
        {
            SalesRevenueList = (from salesList in order.GetSalesList()
                                group salesList by salesList.OrderDate.ToString("MMMM")
                                    into sales
                                    select new SalesRevenue
                                    {
                                        DateOrTime = sales.Key,
                                        Revenue = Math.Round(sales.Sum(item => item.TotalRevenue), 2)
                                    }).ToList();
        }
        private void GetNumberSales()
        {
            NumberSalesList = (from salesList in order.GetSalesList()
                               group salesList by salesList.OrderDate.ToString("MMMM")
                                   into sales
                                   select new NumberSales
                                   {
                                       DateOrTime = sales.Key,
                                       UnitSold = sales.Sum(item => item.UnitSold)
                                   }).ToList();
        }
        private void GetSalesByCategory()
        {
            SalesByCategoryList = (from salesList in order.GetSalesList()
                                   group salesList.UnitSold by salesList.ItemType
                                       into sales
                                       orderby sales.Key
                                       select new SalesByCategory
                                       {
                                           Item = sales.Key,
                                           UnitSold = sales.Sum()
                                       }).ToList();

        }
        private void GetTop5Products()
        {
            Top5ProductsList = (from salesList in order.GetSalesList()
                                group salesList.UnitSold by salesList.ItemType
                                    into sales
                                    orderby sales.Sum() descending
                                    select new BestProduct
                                    {
                                        Item = sales.Key,
                                        UnitSold = sales.Sum()
                                    }).Take(5).ToList();

        }



    }
}
