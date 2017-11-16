using MvcBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace MvcBlog.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        MvcBlogDB db = new MvcBlogDB();
        public ActionResult Index(int Page=1)
        {
            var makale = db.Makales.OrderByDescending(m => m.MakaleId).ToPagedList(Page, 5);
            
            return View(makale);
        }
        public ActionResult BlogAra(string Ara=null)
        {
            var aranan = db.Makales.Where(m=>m.Baslik.Contains(Ara)).ToList();
            return View(aranan.OrderByDescending(m=>m.Tarih));
        }

        public ActionResult SonYorumlar()
        {
            var yorum = db.Yorums.OrderByDescending(y => y.YorumId).Take(5);
            return View(yorum);
        }

        public ActionResult PopulerMakaleler( )
        {
            var makale = db.Makales.OrderByDescending(m => m.Okunma).Take(5);
            return View(makale);
        }

        public ActionResult KategoriMakale(int id)
        {
            var makaleler = db.Makales.Where(k => k.Kategori.KategoriId == id).ToList();
            return View(makaleler);
        }

        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult Projeler()
        {
            return View();
        }
        public ActionResult KategoriPartial()
        {

            return View(db.Kategoris.ToList());
        }
        public JsonResult YorumYap(string yorum,int makaleid)
        {
            var uyeid = Session["uyeid"];
            if (yorum!=null)
            {
                db.Yorums.Add(new Yorum { UyeID = Convert.ToInt32(uyeid), MakaleID = makaleid, Icerik = yorum, Tarih = DateTime.Now});
                db.SaveChanges();
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult YorumSil(int id)
        {
            var uyeid = Session["uyeid"];
            var yorum = db.Yorums.Where(y => y.YorumId == id).SingleOrDefault();
            var makale = db.Makales.Where(m => m.MakaleId == yorum.MakaleID).SingleOrDefault();
            if (yorum.UyeID==Convert.ToInt32(uyeid))
            {
                db.Yorums.Remove(yorum);
                db.SaveChanges();
                return RedirectToAction("MakaleDetay", "Home", new { id = makale.MakaleId });
            }
            else
            {
                return HttpNotFound();
            }
            
        }

        public ActionResult OkumaArttir(int makaleid)
        {
            var makale = db.Makales.Where(m => m.MakaleId == makaleid).SingleOrDefault();
            makale.Okunma += 1;
            db.SaveChanges();
            return View();
        }
    }
}