using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CountdownSolver.Models
{
    public class LettersInput
    {
        [BindRequired]
        public string letters { get; set; }
    }
}
