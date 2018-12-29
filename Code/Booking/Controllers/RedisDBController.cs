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
    public class RedisDBController : Controller
    {
        private ApplicationDbContext _context;
        RedisClient redis = new RedisClient("localhost", 6379);
        JavaScriptSerializer jsonS = new JavaScriptSerializer();

        public RedisDBController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: RedisDB
        public ActionResult Index()
        {
            return View();
        }

        public void AddReservation(Reservation reservation)
        {
            string key = reservation.DateStart.ToString();
            var response = redis.Get(key);
            if(response!=null)
            {
                List<Reservation> reservations = jsonS.Deserialize<List<Reservation>>(response);
                reservations.Add(reservation);
                redis.Set(key, jsonS.Serialize(reservation));

            }
            else
            {
                List<Reservation> res = new List<Reservation>();
                res.Add(reservation);
                redis.Set(key, jsonS.Serialize(res));
            }
        }
    }
}