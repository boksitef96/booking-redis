using Booking.Models;
using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Booking.Controllers
{
    public class RedisController : Controller
    {
        private ApplicationDbContext _context;
        RedisClient redis = new RedisClient("localhost", 6379);
        JavaScriptSerializer jsonS = new JavaScriptSerializer();

        public RedisController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Redis
        public ActionResult Index()
        {
            return View();
        }

        public void initializeRedis()
        {
            List<Accomodation> accomodations;
            accomodations = _context.Accomodations.Where(a => a.Stars > 3).ToList();
            int index = 0;
            foreach (Accomodation acomodation in accomodations)
            {
                if (index > 9) break;
                redis.LPush("accomodationByStars", jsonS.Serialize(acomodation));
                index++;
            }
        }

        public List<Accomodation> GetAboveAvergeStars()
        {
            List<Accomodation> acc = new List<Accomodation>();
            var a = redis.LRange("accomodationByStars", 0, 9);
            foreach (var ac in a)
            { 
                acc.Add(jsonS.Deserialize<Accomodation>(ac));
                redis.LTrim("accomodationByStars", 0, 9);
            }

            return acc;
        }

        public void addAccomodationToList(string key,Accomodation accomodation)
        {
            redis.LPush(key, jsonS.Serialize(accomodation));
        }
    }
}