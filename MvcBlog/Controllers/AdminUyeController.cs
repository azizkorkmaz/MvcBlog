using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace MvcBlog.Controllers
{
    public class AdminUyeController : Controller
    {
        private MvcBlogDB db = new MvcBlogDB();

        // GET: AdminUye
        public ActionResult Index()
        {
            var uyes = db.Uyes.Include(u => u.Yetki);
            return View(uyes.OrderByDescending(u=>u.UyeId).ToList());
        }

        // GET: AdminUye/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uye uye = db.Uyes.Find(id);
            if (uye == null)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        // GET: AdminUye/Create
        public ActionResult Create()
        {
            ViewBag.YetkiID = new SelectList(db.Yetkis, "YetkiId", "YetkiAdi");
            return View();
        }

        // POST: AdminUye/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Uye uye, HttpPostedFileBase Foto )
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotobilgi = new FileInfo(Foto.FileName);
                    string yenifoto = Guid.NewGuid().ToString() + fotobilgi.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/UyeFoto/" + yenifoto);
                    uye.Foto = "/Uploads/UyeFoto/" + yenifoto;

                }
                db.Uyes.Add(uye);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.YetkiID = new SelectList(db.Yetkis, "YetkiId", "YetkiAdi", uye.YetkiID);
            return View(uye);
        }

        // GET: AdminUye/Edit/5
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).FirstOrDefault();
            if (uye== null)
            {
                return HttpNotFound();
            }
          
            
            ViewBag.YetkiID = new SelectList(db.Yetkis, "YetkiId", "YetkiAdi", uye.YetkiID);
            return View(uye);
        }

        // POST: AdminUye/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Uye uye)
        {
            try
            {
                var uyeDuzelt = db.Uyes.Where(m => m.UyeId == id).SingleOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uye.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uye.Foto));
                    }

                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotobilgi = new FileInfo(Foto.FileName);

                    string yenifoto = Guid.NewGuid().ToString() + fotobilgi.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/UyeFoto/" + yenifoto);
                    uyeDuzelt.Foto = "/Uploads/UyeFoto/" + yenifoto;
                    uyeDuzelt.KullaniciAdi = uye.KullaniciAdi;
                    uyeDuzelt.Email = uye.Email;
                    uyeDuzelt.Sifre = uye.Sifre;
                    uyeDuzelt.AdSoyad = uye.AdSoyad;
                    db.SaveChanges();

                }
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                ViewBag.UyeId = new SelectList(db.Uyes, "UyeId", "KullaniciAdi", uye.UyeId);
                return View(uye);
            }
        }

        // GET: AdminUye/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uye uye = db.Uyes.Find(id);
            if (uye == null)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        // POST: AdminUye/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var uyes = db.Uyes.Where(u => u.UyeId == id).FirstOrDefault();
            foreach (var i in uyes.Yorums.ToList())
            {
                db.Yorums.Remove(i);
            }
            foreach (var i in uyes.Makales.ToList())
            {
                db.Makales.Remove(i);
            }
            Uye uye = db.Uyes.Find(id);
            db.Uyes.Remove(uye);
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
