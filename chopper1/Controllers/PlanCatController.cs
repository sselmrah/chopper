using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using chopper1.Models;

namespace chopper1.Controllers
{
    public class PlanCatController : Controller
    {
        private PlanCatEntities db = new PlanCatEntities();

        // GET: PlanCat
        public ActionResult Index()
        {            
            return View(db.TCBCList4Themes.ToList());
        }

        public ActionResult Browse(string title)
        {
            
            var model = db.TCBCList4Themes.Where(a => a.Title.Contains(title));            
            return this.View(model);
        }

        // GET: PlanCat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TCBCList4Themes tCBCList4Themes = db.TCBCList4Themes.Find(id);
            if (tCBCList4Themes == null)
            {
                return HttpNotFound();
            }
            return View(tCBCList4Themes);
        }

        // GET: PlanCat/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlanCat/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ChannelCode,BCDate,ProducerCode,SellerCode,BeginHours,BeginMinutes,BeginSeconds,BeginTime,DaysCount,TimingHours,TimingMinutes,TimingSeconds,Timing,Title,Contents,Wildcard,AutorRights,MainID,PremiereDate,Bit_Repetition,TimingFrames,BeginFrames,RR,RM,DR,DM,BeginTimeText,TimingText,report,PremiereText,CelebrationText,CycleTitle,archiv,CelebrationDay,NormedBegin,DSTI,IsForeign,RRstr,RMstr,DRstr,DMstr,DSTIstr,AgeLimit")] TCBCList4Themes tCBCList4Themes)
        {
            if (ModelState.IsValid)
            {
                db.TCBCList4Themes.Add(tCBCList4Themes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tCBCList4Themes);
        }

        // GET: PlanCat/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TCBCList4Themes tCBCList4Themes = db.TCBCList4Themes.Find(id);
            if (tCBCList4Themes == null)
            {
                return HttpNotFound();
            }
            return View(tCBCList4Themes);
        }

        // POST: PlanCat/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ChannelCode,BCDate,ProducerCode,SellerCode,BeginHours,BeginMinutes,BeginSeconds,BeginTime,DaysCount,TimingHours,TimingMinutes,TimingSeconds,Timing,Title,Contents,Wildcard,AutorRights,MainID,PremiereDate,Bit_Repetition,TimingFrames,BeginFrames,RR,RM,DR,DM,BeginTimeText,TimingText,report,PremiereText,CelebrationText,CycleTitle,archiv,CelebrationDay,NormedBegin,DSTI,IsForeign,RRstr,RMstr,DRstr,DMstr,DSTIstr,AgeLimit")] TCBCList4Themes tCBCList4Themes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tCBCList4Themes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tCBCList4Themes);
        }

        // GET: PlanCat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TCBCList4Themes tCBCList4Themes = db.TCBCList4Themes.Find(id);
            if (tCBCList4Themes == null)
            {
                return HttpNotFound();
            }
            return View(tCBCList4Themes);
        }

        // POST: PlanCat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TCBCList4Themes tCBCList4Themes = db.TCBCList4Themes.Find(id);
            db.TCBCList4Themes.Remove(tCBCList4Themes);
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
