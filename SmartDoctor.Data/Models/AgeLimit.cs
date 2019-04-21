using SmartDoctor.Data.Enums;
using System;

namespace SmartDoctor.Data.Models
{
    public class AgeLimit
    {
        public byte Age { get; }
        public AgeLimitsEnum Limit { get; }
        public AgeLimit(byte age)
        {
            if (age <= 0)
                throw new Exception($"Age value must be positive");
            if (age < 4)
                throw new Exception($"Not supported age");
            else if (age >= 4 && age <= 7)
                Limit = AgeLimitsEnum.FirstChildhood;
            else if (age >= 8 && age <= 12)
                Limit = AgeLimitsEnum.SecondChildhood;
            else if (age >= 13 && age <= 16)
                Limit = AgeLimitsEnum.Teenage;
            else if (age >= 17 && age <= 21)
                Limit = AgeLimitsEnum.Youth;
            else if (age >= 22 && age <= 35)
                Limit = AgeLimitsEnum.FirstMatureAge;
            else if (age >= 36 && age <= 60)
                Limit = AgeLimitsEnum.SecondMatureAge;
            else if (age >= 61 && age <= 74)
                Limit = AgeLimitsEnum.Elderly;
            else if (age >= 75 && age <= 90)
                Limit = AgeLimitsEnum.Senile;
            else if (age >= 91)
                Limit = AgeLimitsEnum.LongLiver;
            Age = age;
        }
    }
}
