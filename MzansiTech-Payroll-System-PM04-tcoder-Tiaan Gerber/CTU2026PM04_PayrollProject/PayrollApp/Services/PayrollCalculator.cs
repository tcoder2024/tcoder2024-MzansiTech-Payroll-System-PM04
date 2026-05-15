using PayrollApp.Models;
using System;

namespace PayrollApp.Services
{
    public class PayrollCalculator
    {
        private const double HOURLY_RATE = 950.00;
        private const double UIF_RATE = 0.01;
        private const double MEMBERSHIP_RATE = 0.13;
        private const double PAYE_BASE_RATE = 0.25;
        private const double DEPENDENT_RELIEF_RATE = 0.0575;

        public static double CalculateGrossPay(double hoursWorked)
        {
            if (hoursWorked < 0)
                throw new ArgumentException("Hours worked cannot be negative");

            return hoursWorked * HOURLY_RATE;
        }

        public static double CalculateUIF(double grossPay)
        {
            if (grossPay < 0)
                throw new ArgumentException("Gross pay cannot be negative");

            return grossPay * UIF_RATE;
        }

        public static double CalculateMembershipFee(double grossPay)
        {
            if (grossPay < 0)
                throw new ArgumentException("Gross pay cannot be negative");

            return grossPay * MEMBERSHIP_RATE;
        }

        public static double CalculatePAYE(double grossPay, int dependents)
        {
            if (grossPay < 0)
                throw new ArgumentException("Gross pay cannot be negative");

            if (dependents < 0 || dependents > 10)
                throw new ArgumentException("Dependents must be between 0 and 10");

            double dependentRelief = grossPay * DEPENDENT_RELIEF_RATE * dependents;
            double taxableIncome = grossPay - dependentRelief;

            if (taxableIncome < 0) taxableIncome = 0;

            return taxableIncome * PAYE_BASE_RATE;
        }
        // Add this method to PayrollCalculator class if not already there
        public static void CalculateAll(PayrollRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            record.GrossPay = CalculateGrossPay(record.HoursWorked);
            record.UIFDeduction = CalculateUIF(record.GrossPay);
            record.MembershipFee = CalculateMembershipFee(record.GrossPay);
            record.PAYE = CalculatePAYE(record.GrossPay, record.Dependents);
            record.NetPay = CalculateNetPay(record.GrossPay, record.UIFDeduction, record.MembershipFee, record.PAYE);
        }

        public static double CalculateNetPay(double grossPay, double uif, double membership, double paye)
        {
            if (grossPay < 0)
                throw new ArgumentException("Gross pay cannot be negative");

            double netPay = grossPay - uif - membership - paye;
            return netPay < 0 ? 0 : netPay;
        }

        // Helper methods for testing
        public static double GetHourlyRate() => HOURLY_RATE;
        public static double GetUIFRate() => UIF_RATE;
        public static double GetMembershipRate() => MEMBERSHIP_RATE;
        public static double GetPAYERate() => PAYE_BASE_RATE;
    }
}