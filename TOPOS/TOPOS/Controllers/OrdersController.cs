﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TOPOS.Data;
using TOPOS.Models;
using TOPOS.Models.Enums;

namespace TOPOS.Controllers
{
    public class OrdersController : Controller
    {
        private TOPOSContext db = new TOPOSContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customers);
            return View(orders.ToList());
        }

        public ActionResult Track()
        {
            var userId = (long) Session["LoginId"];
            var orders = db.Orders.Include(o => o.Customers).Where(o => o.CustomersId == userId);

            return View("Index", orders.ToList());
        }

        public ActionResult Orderdetails(long Id)
        {
            return RedirectToAction("View", "OrderDetails", new { id = Id });
        }

        public ActionResult Complete(long id)
        {
            var checkOrder = db.Orders.Find(id);

            if(checkOrder is null)
                return View(db.Orders.Include(o => o.Customers).ToList());

            checkOrder.StatusId = (long)StatusType.End;
            db.SaveChanges();

            return View("Index", db.Orders.Include(o => o.Customers).ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomersId = new SelectList(db.Customers, "Id", "NameSurname");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,StatusId,CustomersId")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.CustomersId = new SelectList(db.Customers, "Id", "NameSurname", orders.CustomersId);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }

            ViewBag.StatusId = new SelectList(new List<SelectListItem>
                                              {
                                                    new SelectListItem { Text = "Waiting", Value = ((long)StatusType.Waiting).ToString()},
                                                    new SelectListItem { Text = "InQueue", Value =  ((long)StatusType.InQueue).ToString()},
                                                    new SelectListItem { Text = "Preparing", Value =  ((long)StatusType.Preparing).ToString()},
                                                    new SelectListItem { Text = "OnTheWay", Value =  ((long)StatusType.OnTheWay).ToString()},
                                              }, "Value", "Text");

            ViewBag.CustomersId = new SelectList(db.Customers, "Id", "NameSurname", orders.CustomersId);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,StatusId,CustomersId")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomersId = new SelectList(db.Customers, "Id", "NameSurname", orders.CustomersId);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Orders orders = db.Orders.Find(id);
            db.Orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index");
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
