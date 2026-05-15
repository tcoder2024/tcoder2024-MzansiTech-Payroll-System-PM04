using System;
using System.Collections.Generic;
using System.IO;
using PayrollApp.Models;

namespace PayrollApp.Services
{
    public class CSVStorage
    {
        private readonly string _storageDirectory;
        private readonly string _payrollFilePath;
        private List<PayrollRecord> _payrollHistory;

        public CSVStorage()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _storageDirectory = Path.Combine(documentsPath, "PayrollSystem");

            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            _payrollFilePath = Path.Combine(_storageDirectory, "payroll_history.csv");
            _payrollHistory = new List<PayrollRecord>();
            LoadPayrollHistory();
        }

        public List<PayrollRecord> GetAllRecords() => _payrollHistory;

        public void SaveRecord(PayrollRecord record)
        {
            ArgumentNullException.ThrowIfNull(record);
            _payrollHistory.Add(record);
            SaveToFile();
        }

        public void SaveToFile()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(_payrollFilePath, false);
                writer.WriteLine(PayrollRecord.GetCSVHeader());
                foreach (var record in _payrollHistory)
                {
                    writer.WriteLine(record.ToCSVString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save: {ex.Message}");
            }
        }

        private void LoadPayrollHistory()
        {
            if (!File.Exists(_payrollFilePath)) return;

            try
            {
                string[] lines = File.ReadAllLines(_payrollFilePath);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    if (parts.Length >= 9)
                    {
                        var record = new PayrollRecord
                        {
                            ContractorName = parts[0],
                            HoursWorked = double.Parse(parts[1]),
                            Dependents = int.Parse(parts[2]),
                            GrossPay = double.Parse(parts[3]),
                            UIFDeduction = double.Parse(parts[4]),
                            MembershipFee = double.Parse(parts[5]),
                            PAYE = double.Parse(parts[6]),
                            NetPay = double.Parse(parts[7]),
                            CalculationDate = DateTime.Parse(parts[8])
                        };
                        _payrollHistory.Add(record);
                    }
                }
            }
            catch (Exception)
            {
                // If file is corrupted, start with empty history
            }
        }

        public int GetRecordCount() => _payrollHistory.Count;

        public void ClearHistory()
        {
            _payrollHistory.Clear();
            SaveToFile();
        }
    }
}