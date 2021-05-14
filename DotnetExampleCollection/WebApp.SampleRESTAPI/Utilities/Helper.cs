using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.SampleRESTAPI.Utilities
{
    public static class Helper
    {
        public static int GetAge(this DateTimeOffset dateTimeOffset)
        {
            var currentDate = DateTimeOffset.UtcNow;
            var age = currentDate.Year - dateTimeOffset.Year;

            if(currentDate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }

    }
}
