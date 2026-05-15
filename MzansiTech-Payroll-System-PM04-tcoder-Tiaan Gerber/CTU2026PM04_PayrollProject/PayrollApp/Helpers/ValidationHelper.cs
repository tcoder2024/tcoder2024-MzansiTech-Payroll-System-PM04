using System;

namespace PayrollApp.Helpers
{
    public class ValidationHelper
    {
        public static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        public static bool IsValidHours(double hours)
        {
            return hours >= 0 && hours <= 744; // Max 31 days * 24 hours
        }

        public static bool IsValidDependents(int dependents)
        {
            return dependents >= 0 && dependents <= 10;
        }

        public static bool TryParseHours(string input, out double hours)
        {
            return double.TryParse(input, out hours);
        }

        public static string GetErrorMessage(string fieldName, string reason)
        {
            return $"{fieldName}: {reason}";
        }
    }
}