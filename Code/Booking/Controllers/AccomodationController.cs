﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Booking.Models;
using Booking.Controllers;
using CSRedis;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Booking.Helpers;

namespace Booking.Controllers
{
    public class AccomodationController : Controller
    {
        private ApplicationDbContext _context;
        public RedisController redisDB;

        public AccomodationController()
        {
            _context = new ApplicationDbContext();
            redisDB = new RedisController();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Accomodation
        public ActionResult Index()
        {
            return View();
        }
        
        [Authorize]
        [Route("add-new-accomodation", Name = "add_new_accomodation")]
        public ActionResult AddNewAccomodation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAccomodation(Accomodation accomodation)
        {
            var currentUserName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = _context.Users.Where(x => x.Email == currentUserName).FirstOrDefault();
            accomodation.User = user;
            accomodation.AvailableRooms = 0;
            accomodation.CreationDate = DateTime.Now;
            accomodation.LastUpdate = DateTime.Now;
            accomodation.Rating = 0;
            _context.Accomodations.Add(accomodation);
            if (accomodation.Stars>3)
            {
                redisDB.addAccomodationToList("accomodationByStars", accomodation);
            }
            //redis.Set("accomodation:"+accomodation.Id.ToString()+":id", jsonS.Serialize(accomodation));
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("show-all-accomoations", Name ="show_all_accomodations")]
        public ActionResult ShowAllAccomodations()
        {
            //var accomodations = _context.Accomodations.ToList();
            List<Accomodation> accomodations = redisDB.GetAboveAvergeStars();
            return View(accomodations);
        }

        [HttpGet]
        [Authorize]
        [Route("show-user-accomoations", Name = "show_user_accomodations")]
        public ActionResult ShowAllAccomodationsByUser()
        {
            var currentUserName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = _context.Users.Where(x => x.Email == currentUserName).FirstOrDefault();
           
            var accomodations = _context.Accomodations.Where(a => a.User.Id == user.Id).ToList();
            return View(accomodations);
        }

        [Authorize]
        public ActionResult DeleteAccomodation(int id)
        {
            var accomodation = _context.Accomodations.Where(a => a.Id == id).FirstOrDefault();

            _context.Accomodations.Remove(accomodation);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

    }
}