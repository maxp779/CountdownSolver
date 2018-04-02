﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CountdownSolver.Logic.LettersGame;
using CountdownSolver.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CountdownSolver.Controllers
{
    [Route("countdownsolver/[controller]")]
    public class CountdownLettersController : Controller
    {

        [HttpGet]
        public JsonResult Get([FromQuery] LettersInput lettersGameInput)
        {
            ICollection<string> output = this.RunLettersGame(lettersGameInput);
            return Json(output);
        }

        [HttpPost]
        public JsonResult Post([FromBody] LettersInput lettersGameInput)
        {
            ICollection<string> output = this.RunLettersGame(lettersGameInput);
            return Json(output);
        }

        private ICollection<string> RunLettersGame (LettersInput lettersGameInput)
        {
            CountdownWordsFinder wordFinder = new CountdownWordsFinder();
            ICollection<string> wordsFound = wordFinder.FindAllWords(lettersGameInput.letters);
            return wordsFound;
        }

        // GET: api/values
        //[HttpGet]
        //public JsonResult Get()
        //{
        //    ICollection<string> wordsFound = new List<string>();
        //    return Json(wordsFound);
        //}

        //// GET api/values/5
        //[HttpGet("{inputCharacters}")]
        //public JsonResult Get(string inputCharacters)
        //{ 
        //    CountdownWordsFinder wordFinder = new CountdownWordsFinder();
        //    ICollection<string> wordsFound = wordFinder.FindAllWords(inputCharacters);
        //    return Json(wordsFound);
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
