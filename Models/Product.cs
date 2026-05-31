using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.Models
{
    public class Product
    {
        /// <summary>
        /// This class was created for testing purposes only,
        /// the data may be inaccurate and meaningless.
        /// </summary>
        /// 

        public int Id { get; set; }
        public string Item { get; set; }
        public int Stock { get; set; }
        public double UnitPrice { get; set; }

        public Product()
        {

        }

        public List<Product> GetProductsList()
        {
            var list = new List<Product>();
            list.Add(new Product { Id = 1, Item = "Baby Food", Stock = 989, UnitPrice = 19.98 });
            list.Add(new Product { Id = 2, Item = "Beverages", Stock = 1589, UnitPrice = 21.90 });
            list.Add(new Product { Id = 3, Item = "Cereal", Stock = 1515, UnitPrice = 9.56 });
            list.Add(new Product { Id = 4, Item = "Clothes", Stock = 478, UnitPrice = 54.65});
            list.Add(new Product { Id = 5, Item = "Cosmetics", Stock = 3659, UnitPrice = 19.98 });
            list.Add(new Product { Id = 6, Item = "Fruits", Stock = 456, UnitPrice = 3.5 });
            list.Add(new Product { Id = 7, Item = "House Hold", Stock = 2548, UnitPrice = 11.55 });
            list.Add(new Product { Id = 8, Item = "Meat", Stock = 325, UnitPrice = 21.9 });
            list.Add(new Product { Id = 9, Item = "Office Supplies", Stock = 6548, UnitPrice = 8.69 });
            list.Add(new Product { Id = 10, Item = "Personal Care", Stock = 2987, UnitPrice = 17.45 });
            list.Add(new Product { Id = 11, Item = "Snacks", Stock = 278, UnitPrice = 3.9 });
            list.Add(new Product { Id = 12, Item = "Vegetables", Stock = 198, UnitPrice = 2.8 });

            return list;
        }

    }
}
