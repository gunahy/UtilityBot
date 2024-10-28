using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityBot.Models
{
    public class UserSession
    {
        public enum Mode
        {
            None,
            CountCharacters,
            SumNumbers
        }
        public Mode SelectedMode { get; set; } = Mode.None;

        public void SetMode(Mode mode)
        {
            SelectedMode = mode;
        }
        public void Reset()
        {
            SelectedMode = Mode.None;
        }
    }
}
