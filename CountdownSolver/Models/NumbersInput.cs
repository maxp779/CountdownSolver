using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CountdownSolver.Models
{
    public class NumbersInput
    {
        [BindRequired]
        public string number1 { get; set; }
        [BindRequired]
        public string number2 { get; set; }
        [BindRequired]
        public string number3 { get; set; }
        [BindRequired]
        public string number4 { get; set; }
        [BindRequired]
        public string number5 { get; set; }
        [BindRequired]
        public string number6 { get; set; }
        [BindRequired]
        public string target { get; set; }
        public string algorithm { get; set; }
    }
}
