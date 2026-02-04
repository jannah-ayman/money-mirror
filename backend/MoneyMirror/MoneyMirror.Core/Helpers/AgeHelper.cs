using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Helpers
{
    /// <summary>
    /// Helper class for calculating age and age groups from Date of Birth.
    /// Centralizes age logic to prevent inconsistencies.
    /// </summary>
    public static class AgeHelper
    {
        /// <summary>
        /// Calculates the current age from date of birth.
        /// </summary>
        /// <param name="dob">Date of birth</param>
        /// <returns>Age in years</returns>
        public static int CalculateAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;

            // If birthday hasn't occurred this year yet, subtract 1
            if (dob.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// Determines the age group based on date of birth.
        /// </summary>
        /// <param name="dob">Date of birth</param>
        /// <returns>Age group enum</returns>
        public static ChildAgeGroup CalculateAgeGroup(DateTime dob)
        {
            int age = CalculateAge(dob);

            return age switch
            {
                < 6 => ChildAgeGroup.Younger_Than_6,
                >= 6 and <= 8 => ChildAgeGroup.Age_6_8,
                >= 9 and <= 11 => ChildAgeGroup.Age_9_11,
                >= 12 and <= 14 => ChildAgeGroup.Age_12_14,
                _ => ChildAgeGroup.Older_Than_14
            };
        }

        /// <summary>
        /// Gets a child-friendly display name for an age group.
        /// </summary>
        /// <param name="ageGroup">Age group enum</param>
        /// <returns>Display name</returns>
        public static string GetAgeGroupDisplayName(ChildAgeGroup ageGroup)
        {
            return ageGroup switch
            {
                ChildAgeGroup.Younger_Than_6 => "Under 6 years old",
                ChildAgeGroup.Age_6_8 => "6-8 years old",
                ChildAgeGroup.Age_9_11 => "9-11 years old",
                ChildAgeGroup.Age_12_14 => "12-14 years old",
                ChildAgeGroup.Older_Than_14 => "Over 14 years old",
                _ => "Unknown"
            };
        }
    }
}