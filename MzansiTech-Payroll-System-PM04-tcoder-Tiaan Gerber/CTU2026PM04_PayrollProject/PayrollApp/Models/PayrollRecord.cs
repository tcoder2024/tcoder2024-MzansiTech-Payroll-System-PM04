using System;

namespace PayrollApp.Models
{
    public class PayrollRecord
    {
        public string ContractorName { get; set; } = string.Empty;
        public double HoursWorked { get; set; }
        public int Dependents { get; set; }
        public double GrossPay { get; set; }
        public double UIFDeduction { get; set; }
        public double MembershipFee { get; set; }
        public double PAYE { get; set; }
        public double NetPay { get; set; }
        public DateTime CalculationDate { get; set; }

        public PayrollRecord()
        {
            CalculationDate = DateTime.Now;
        }

        public string ToCSVString()
        {
            return $"{ContractorName},{HoursWorked},{Dependents},{GrossPay},{UIFDeduction},{MembershipFee},{PAYE},{NetPay},{CalculationDate:yyyy-MM-dd HH:mm:ss}";
        }

        public static string GetCSVHeader()
        {
            return "ContractorName,HoursWorked,Dependents,GrossPay,UIFDeduction,MembershipFee,PAYE,NetPay,CalculationDate";
        }
    }
}