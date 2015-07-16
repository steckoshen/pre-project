using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Reflection;
using MvcPaging;

namespace WebApplication1.Controllers
{
    public class TablePagingController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();
        string FilterCity, SortColumnName, SortType;
        private const int DefaultPageSize = 10;
        private int page = 0;

        // GET: Ddl
        public ActionResult Index(int page=0)
        {
            PrepareDropDownList();

            ViewData.Model = new List<Customer>();
            
            ViewBag.FilterCity = this.FilterCity;
            ViewBag.SortColumnName = this.SortColumnName;
            ViewBag.SortType = this.SortType;

            return View();
        }


        [HttpPost]
        public ActionResult Index(string city = "all", string columnName="CompanyName", string sort = "asc")
        {
            ViewBag.Message = "Paging Gridview";

            PrepareDropDownList();
            var query = db.Customers.Select(x => x);

            if (!city.Equals("all")) {
                 query = query.Where("City == @0", city ?? "Lodon");
            }

            query = query.OrderBy(string.Format("{0} {1}", columnName, sort));
            ViewData.Model = query.ToList();

            return View();
        }

        private void PrepareDropDownList() {

            GetCityDDL();
            GetColumnNameDDL();
            GetSortingRuleDDL();
        }

        /// <summary>
        /// Prepare for DropDownList: City
        /// </summary>
        private void GetCityDDL() {

            SelectList selectlist = new SelectList(this.GetCityList(), "Value", "Text");
            ViewBag.CityList = selectlist;

        }

        private void GetColumnNameDDL() {
            
            SelectList selectlist = new SelectList(this.GetColumnNameList(), "Value", "Text");
            ViewBag.ColumnNameList = selectlist;

        }

        private void GetSortingRuleDDL() {
            List<SelectListItem> sortList = new List<SelectListItem>();
            sortList.Add(new SelectListItem() { Text = "ASC", Value = "asc" });
            sortList.Add(new SelectListItem() { Text = "DESC", Value = "desc" });
            SelectList selectlist = new SelectList(sortList, "Value", "Text");
            ViewBag.SortList = selectlist;
        }

        /// <summary>
        /// Get CityList in Customers
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetCityList()
        {
            var cities = db.Customers.Select(x => x.City).OrderBy(x => x).Distinct();

            List<SelectListItem> cityList = new List<SelectListItem>();
            cityList.Add(new SelectListItem() { Text = "All City", Value = "all" });
            foreach (var city in cities) {
                cityList.Add(new SelectListItem() { Value = city, Text = city });
            }

            return cityList;
        }

        /// <summary>
        /// Get ColumnName List by LingObj
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetColumnNameList() {

            var linqObj = new Customer();

            List<SelectListItem> columnNameList = new List<SelectListItem>();
            foreach (PropertyInfo pI in linqObj.GetType().GetProperties().Distinct())
            {
                columnNameList.Add(new SelectListItem() { Value = pI.Name, Text = pI.Name });
            }

            return columnNameList;
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
