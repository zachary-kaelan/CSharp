using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZachLib;

namespace WellsFargoReportAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex RGX_MEMO1 = new Regex(
                @"^([A-Za-z0-9 ]*?) (\d{6}) (\d*|[A-Z ].*) (?:JOHN|NIC|Nic|John|\d{16})",
                RegexOptions.Singleline | RegexOptions.Compiled
            );
            Regex RGX_MEMO2 = new Regex(
                @"^([A-Za-z$0-9. ]*) ON ([0-9\/]*) (.*?) [A-Z]\d",
                RegexOptions.Singleline | RegexOptions.Compiled
            );

            Regex RGX_MEMO_SUBTYPE = new Regex(
                @"^((?:[A-Za-z][A-Za-z.'-]*(?: |$)){1,}|[^ ]* ?)(.*)?",
                RegexOptions.Singleline | RegexOptions.Compiled
            );

            var paymentsTemp = Utils.LoadCSV<PaymentTemp>(
                @"C:\Users\ZACH-GAMING\Downloads\Checking1.csv"
            ).Where(p => String.IsNullOrWhiteSpace(p.Deposit) && p.Withdrawal < 0);

            List<Payment> payments = new List<Payment>();
            foreach (var paymentTemp in paymentsTemp)
            {
                var match = RGX_MEMO1.Match(paymentTemp.Memo);
                if (!match.Success)
                    match = RGX_MEMO2.Match(paymentTemp.Memo);
                if (match.Success)
                {
                    payments.Add(
                        new Payment()
                        {
                            Type = match.Groups[1].Value,
                            Date = match.Groups[2].Value,
                            Memo = match.Groups[3].Value,
                            Amount = paymentTemp.Withdrawal
                        }
                    );
                }
            }

            List<PieChartPiece> pieChartTemp = new List<PieChartPiece>();

            var total = payments.Sum(p => p.Amount);
            var cutOff = total / 100;
            Console.WriteLine("Total:   $" + total.ToString());
            Console.WriteLine("Cut-Off: $" + cutOff.ToString("#.00"));
            var paymentTypes = payments.GroupBy(p => p.Type).OrderBy(g => g.Sum(p => p.Amount));
            
            foreach(var type in paymentTypes)
            {
                var typeTotal = type.Sum(p => p.Amount);
                if (typeTotal > cutOff)
                    continue;
                var typeCutOff = typeTotal / 100;

                if (type.Count() == 1)
                {
                    var single = type.First();
                    Console.WriteLine("{0} {1} -:- {2} - ${3}", type.Key.Trim(), single.Memo, single.Date, single.Amount);
                    continue;
                }

                Console.WriteLine("{0} - ${1}", type.Key, type.Sum(p => p.Amount));
                List<Payment> paymentsBySubtype = new List<Payment>();
                foreach(var payment in type.OrderBy(p => p.Amount))
                {
                    var match = RGX_MEMO_SUBTYPE.Match(payment.Memo);
                    if (match.Success)
                    {
                        if (match.Groups[1].Value.ToUpper().StartsWith("WAL-MART") || match.Groups[1].Value.StartsWith("WM "))
                        {
                            paymentsBySubtype.Add(
                                new Payment()
                                {
                                    Date = payment.Date,
                                    Amount = payment.Amount,
                                    Subtype = "WAL-MART",
                                    Memo = match.Groups[2].Value
                                }
                            );
                        }
                        else
                        {
                            paymentsBySubtype.Add(
                                new Payment()
                                {
                                    Date = payment.Date,
                                    Amount = payment.Amount,
                                    Subtype = match.Groups[1].Value,
                                    Memo = match.Groups[2].Value
                                }
                            );
                        }
                    }
                    else
                        Console.WriteLine("\t{0} -:- {1} - ${2}", payment.Memo, payment.Date, payment.Amount);
                }

                foreach(var subtype in paymentsBySubtype.GroupBy(p => p.Subtype).OrderBy(g => g.Sum(p => p.Amount)))
                {
                    var subtypeTotal = subtype.Sum(p => p.Amount);
                    if (subtypeTotal > typeCutOff)
                        continue;
                    var subtypeCutoff = subtypeTotal / 100;

                    pieChartTemp.Add(
                        new PieChartPiece()
                        {
                            Type = type.Key,
                            Subtype = subtype.Key,
                            Amount = Math.Abs(subtypeTotal)
                        }
                    );

                    if (subtype.Count() == 1)
                    {
                        var single = subtype.First();
                        Console.WriteLine("\t\t{0} {1} -:- {2} - ${3}", subtype.Key.Trim(), single.Memo, single.Date, single.Amount);
                        continue;
                    }

                    Console.WriteLine("\t\t{0} - ${1}", subtype.Key.Trim(), subtype.Sum(p => p.Amount));
                    foreach (var payment in subtype.OrderBy(p => p.Amount))
                    {
                        if (payment.Amount > subtypeCutoff)
                            continue;
                        if (String.IsNullOrWhiteSpace(payment.Memo))
                            Console.WriteLine("\t\t\t{0} - ${1}", payment.Date, payment.Amount);
                        else
                            Console.WriteLine("\t\t\t{0} -:- {1} - ${2}", payment.Memo.Trim(), payment.Date, payment.Amount);
                    }
                }

                Console.WriteLine();
            }


            var pieChartBySubtype = pieChartTemp.GroupBy(p => p.Subtype);
            var pieChartFinal = new List<CSVKeyValuePair<string, double>>();
            foreach(var subtype in pieChartBySubtype)
            {
                if (subtype.Count() == 1 && subtype.First().Type == paymentTypes.First().Key)
                    pieChartFinal.Add(new CSVKeyValuePair<string, double>(subtype.Key, subtype.First().Amount));
                else
                    pieChartFinal.Add(new CSVKeyValuePair<string, double>(subtype.Key, subtype.Sum(s => s.Amount)));
            }
            pieChartFinal.OrderByDescending(p => p.Value).SaveCSV(@"E:\PieChart.csv");

            Console.ReadLine();
        }
    }

    public class PaymentTemp
    {
        public DateTime Date { get; set; }
        public double Withdrawal { get; set; }
        public string Deposit { get; set; }
        public string Memo { get; set; }
    }

    public class Payment
    {
        public double Amount { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Date { get; set; }
        public string Memo { get; set; }
    }

    public class PieChartPiece
    {
        public string Type { get; set; }
        public string Subtype { get; set; }
        public double Amount { get; set; }
    }
}
