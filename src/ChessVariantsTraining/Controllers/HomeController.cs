﻿using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsTraining
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
