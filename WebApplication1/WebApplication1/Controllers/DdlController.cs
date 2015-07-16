using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DdlController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: Ddl
        public ActionResult Index()
        {

            ViewBag.Message = "DropDownList testing";
            SelectList selectlist = new SelectList(this.GetCustomers(), "CustomerID", "ContactName");
            ViewBag.Selectlist = selectlist;
            //return View(db.Products.ToList());
            return View();
        }

        [HttpPost]
        public JsonResult Orders(string customerID)
        {

            List<KeyValuePair<int, string>> items = new List<KeyValuePair<int, string>>();

            if (!string.IsNullOrEmpty(customerID))
            {
                var orders = GetOrders(customerID);

                foreach (var order in orders)
                {
                    items.Add(new KeyValuePair<int, string>(int.Parse(order.OrderID.ToString()),
                        string.Format("{0} ({1:yyyy-MM-dd})", order.OrderID, order.OrderDate)));
                }
            }
            JsonResult j = this.Json(items);
            return j;
        }

        [HttpPost]
        public JsonResult Products(string orderID) 
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            int id =0;

            if (!string.IsNullOrEmpty(orderID) && int.TryParse(orderID, out id)) 
            {
                    var products = GetProducts(id);
                    if (products.Count() > 0)
                    {
                        foreach (var product in products)
                        {
                            items.Add(new KeyValuePair<string, string>(
                                product.ProductID.ToString(), product.ProductName));
                        }
                    }
                
            }

            return this.Json(items);
        }

        public ActionResult ProductInfo(string productID) 
        {
            int id = 0;
            if (!string.IsNullOrEmpty(productID) && int.TryParse(productID, out id)) {
                var productinfo = GetProductInfo(id);

                ViewData.Model = productinfo;
            }
            return PartialView("_ProductInfoPartialView");
        }

        private IEnumerable<Customer> GetCustomers()
        {
            var query = db.Customers.OrderBy(x => x.CustomerID);
            return query.ToList();
        }

        private IEnumerable<Order> GetOrders(string customerID)
        {
            var query = db.Orders.Where(x => x.CustomerID == customerID).OrderBy(x => x.OrderDate);
            return query.ToList();
        }

        private IEnumerable<Product> GetProducts(int orderID)
        {
            var query = db.Order_Details.Where(x => x.OrderID == orderID).Select(x => x.Product);
            return query.ToList();
        }

        private Product GetProductInfo(int productID) {
            
            return db.Products.FirstOrDefault(x => x.ProductID == productID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
