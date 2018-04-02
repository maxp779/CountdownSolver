using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CountdownSolver.Logic.NumbersGame;
using CountdownSolver.Models;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CountdownSolver.Controllers
{
    [Route("countdownsolver/[controller]")]
    public class CountdownNumbersController : Controller
    {
        [HttpGet]
        public JsonResult Get([FromQuery] NumbersInput numbersGameInput)
        {
            ICollection<string> output = this.RunNumbersGame(numbersGameInput);
            return Json(output);
        }

        [HttpPost]
        public JsonResult Post([FromBody] NumbersInput numbersGameInput)
        {
            ICollection<string> output = this.RunNumbersGame(numbersGameInput);
            return Json(output);
        }

        private ICollection<string> RunNumbersGame(NumbersInput numbersGameInput)
        {
            ICollection<string> output;
            if (numbersGameInput.algorithm == "recursive")
            {
                List<int> intNumbersInput = new List<int>();
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number1));
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number2));
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number3));
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number4));
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number5));
                intNumbersInput.Add(Convert.ToInt32(numbersGameInput.number6));
                CountdownNumbersCalculatorRecursive calculator = new CountdownNumbersCalculatorRecursive();
                output = calculator.calculate(intNumbersInput, Convert.ToInt32(numbersGameInput.target));
            }
            else
            {
                List<string> stringNumbersInput = new List<string>();
                stringNumbersInput.Add(numbersGameInput.number1);
                stringNumbersInput.Add(numbersGameInput.number2);
                stringNumbersInput.Add(numbersGameInput.number3);
                stringNumbersInput.Add(numbersGameInput.number4);
                stringNumbersInput.Add(numbersGameInput.number5);
                stringNumbersInput.Add(numbersGameInput.number6);
                CountdownNumbersCalculatorBruteForce calculator = new CountdownNumbersCalculatorBruteForce(stringNumbersInput, numbersGameInput.target);
                output = calculator.Calculate();
            }
            return output;
        }

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
