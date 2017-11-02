using MvcBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        // GET: AdminMakale
        MvcBlogDB db = new MvcBlogDB();
        public ActionResult Index()
        {
            var makaleler = db.Makales.ToList();
            return View(makaleler);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Makale makale = db.Makales.Find(id);
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)

            {
                // TODO: Add insert logic here

                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotobilgi = new FileInfo(Foto.FileName);
                    string yenifoto = Guid.NewGuid().ToString() + fotobilgi.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + yenifoto);
                    makale.Foto = "/Uploads/MakaleFoto/" + yenifoto;
                    
                }

                if (etiketler != null)
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { Adi = i };
                        db.Etikets.Add(yenietiket);
                        makale.Etikets.Add(yenietiket);
                    }

                }
                makale.UyeID = Convert.ToInt32(Session["uyeid"]);
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(makale);

        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi",makale.KategoriID);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                var makaleDuzelt = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (Foto!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makale.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makale.Foto));
                    }

                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotobilgi = new FileInfo(Foto.FileName);

                    string yenifoto = Guid.NewGuid().ToString() + fotobilgi.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + yenifoto);
                    makaleDuzelt.Foto = "/Uploads/MakaleFoto/" + yenifoto;
                    makaleDuzelt.Baslik = makale.Baslik;
                    makaleDuzelt.Icerik = makale.Icerik;
                    makaleDuzelt.KategoriID = makale.KategoriID;
                    db.SaveChanges();
                   
                }
                return RedirectToAction("Index");

            }
            catch (Exception )
            {
                ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriID);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.KategoriID == id).SingleOrDefault();
            //if (makale == null)
            //{
            //    return HttpNotFound();
            //}
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makale = db.Makales.Where(m => m.KategoriID == id).SingleOrDefault();
                //if (makale == null)
                //{
                //    return HttpNotFound();
                //}
                if (System.IO.File.Exists(Server.MapPath(makale.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makale.Foto));
                }

                foreach (var i in makale.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makale.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
