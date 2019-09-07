using GDN1.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
namespace GDN1.Controllers
{
	[Authorize]
    public class NavettesController : Controller
    {
        private Entities db = new Entities();
		

        // GET: Navettes
        public ActionResult Index()
        {
			
			var navettes = db.Navettes.Include(n => n.AspNetUser).Include(n => n.Lieu).Include(n => n.Lieu1);
            return View(navettes.Where(c=>c.AspNetUser.UserName==User.Identity.Name).ToList());
        }
		public ActionResult Index1(decimal? prix,DateTime? dated,DateTime? datef,string gamme,bool? wifi,bool? clim,bool? tv)
		{
			

			var appartenir = db.AspNetUsers.Where(c => c.UserName == User.Identity.Name).FirstOrDefault().Appartenirs.OrderByDescending(c=>c.Date).FirstOrDefault();
			var navettes = db.Navettes.Include(n => n.AspNetUser).Include(n => n.Lieu).Include(n => n.Lieu1);
			if (prix != null)
			{
				navettes=navettes.Where(x => x.Prix == prix);
			}
			if (dated != null)
			{
				navettes = navettes.Where(x => x.DateD <= dated);
			}
			if (datef != null)
			{
				navettes = navettes.Where(x => x.DateA >= datef);
			}
			if (gamme != null&&gamme!="-")
			{
				navettes = navettes.Where(x => x.Gamme == gamme);
			}
			if (wifi != null)
			{
				navettes = navettes.Where(x => x.WIFI == wifi);
			}
			if (clim != null)
			{
				navettes = navettes.Where(x => x.Climatisation == clim);
			}
			if (tv != null)
			{
				navettes = navettes.Where(x => x.TV == tv);
			}
			if(appartenir!=null)
				return View(navettes.Where(c => c.Id != appartenir.Id_Navette).ToList());
			return View(navettes.ToList());

		}
		public ActionResult Achat(int id)
		{
			var Navette = db.Navettes.Where(c => c.Id == id).FirstOrDefault();
			var Client = db.AspNetUsers.Where(c => c.UserName == User.Identity.Name).FirstOrDefault();
			if (Navette.Disponibilé == true && Navette.Appartenirs.Count < Navette.Capacité && (Client.Appartenirs.Count==0||Client.Appartenirs.Max(c=>c.Date)>DateTime.Now.AddMonths(1)))
			{
				
				Appartenir appartenir = new Appartenir() { Id_Navette = id, Id_Client = Client.Id, Date = DateTime.Now };
				db.Appartenirs.Add(appartenir);
				TempData["Msg"] = "Achat Effectué";
				try
				{
					db.SaveChanges();
				}
				catch (Exception e) {
					TempData["Msg"] = e.Message;
				}
				
			}
			TempData["Msg"] = TempData["Msg"] ?? "Erreur : Achat";
			return RedirectToAction("Index1");
		}
		// GET: Navettes/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Navette navette = db.Navettes.Find(id);
            if (navette == null)
            {
                return HttpNotFound();
            }
            return View(navette);
        }

        // GET: Navettes/Create
        public ActionResult Create()
        {
            ViewBag.Id_Entreprise = new SelectList(db.AspNetUsers, "Id", "Email");
			ViewBag.Gamme = new SelectList(new ArrayList { "Haut de Gamme","Economique","Bas de Gamme"});

			ViewBag.Arrivée = new SelectList(db.Lieux, "Id", "Zone");
			ViewBag.Départ = new SelectList(db.Lieux, "Id", "Zone");
			return View();
        }

        // POST: Navettes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Navette navette)
        {
			AspNetUser user = db.AspNetUsers.Where(c => c.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
				
				navette.Id_Entreprise =user.Id;
				navette.Disponibilé = true;
				navette.Départ = null;
				navette.Arrivée = null;
				navette.Date = DateTime.Now;
				db.Navettes.Add(navette);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Entreprise = new SelectList(db.AspNetUsers, "Id", "Email", navette.Id_Entreprise);
			ViewBag.Gamme = new SelectList(new ArrayList { "Haut de Gamme", "Economique", "Bas de Gamme" });
			ViewBag.Arrivée = new SelectList(db.Lieux, "Id", "Zone", navette.Arrivée);
			ViewBag.Départ = new SelectList(db.Lieux, "Id", "Zone", navette.Départ);
			return View(navette);
        }

        // GET: Navettes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Navette navette = db.Navettes.Find(id);
            if (navette == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Entreprise = new SelectList(db.AspNetUsers, "Id", "Email", navette.Id_Entreprise);
            ViewBag.Arrivée = new SelectList(db.Lieux, "Id", "Zone", navette.Arrivée);
            ViewBag.Départ = new SelectList(db.Lieux, "Id", "Zone", navette.Départ);
            return View(navette);
        }

        // POST: Navettes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Capacité,Prix,Disponibilé,Gamme,WIFI,TV,Climatisation,Départ,Arrivée,DateD,DateA,Id_Entreprise")] Navette navette)
        {
            if (ModelState.IsValid)
            {
                db.Entry(navette).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Entreprise = new SelectList(db.AspNetUsers, "Id", "Email", navette.Id_Entreprise);
            ViewBag.Arrivée = new SelectList(db.Lieux, "Id", "Zone", navette.Arrivée);
            ViewBag.Départ = new SelectList(db.Lieux, "Id", "Zone", navette.Départ);
            return View(navette);
        }

        // GET: Navettes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
           }
            Navette navette = db.Navettes.Find(id);
            if (navette == null)
            {
                return HttpNotFound();
            }
            return View(navette);
        }

        // POST: Navettes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Navette navette = db.Navettes.Find(id);
			//navette.AspNetUser.Navettes = null;
			db.Appartenirs.RemoveRange(navette.Appartenirs);
			db.SaveChanges();
			db.Navettes.Remove(navette);
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
