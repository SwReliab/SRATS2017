using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRATS2017AddIn.Models
{
    public class SRMList
    {
        public bool IsBasic { get; set; }
        public bool IsCPH { get; set; }
        public bool IsHEr { get; set; }
        public string CPHPhase { get; set; }
        public string HErPhase { get; set; }

        public List<int> CPH
        {
            get
            {
                List<int> tmp = new List<int>();
                string[] strlist = CPHPhase.Replace(" ", "").Split(',');
                foreach (string str in strlist)
                {
                    string[] nl = str.Split('-');
                    if (nl.Length == 2)
                    {
                        int lower = Int32.Parse(nl[0]);
                        int upper = Int32.Parse(nl[1]);
                        for (int i=lower; i<=upper; i++)
                        {
                            tmp.Add(i);
                        }
                    }
                    else if (nl.Length == 1)
                    {
                        tmp.Add(Int32.Parse(nl[0]));
                    }
                }
                return tmp;
            }
        }

        public List<int> HEr
        {
            get
            {
                List<int> tmp = new List<int>();
                string[] strlist = HErPhase.Replace(" ", "").Split(',');
                foreach (string str in strlist)
                {
                    string[] nl = str.Split('-');
                    if (nl.Length == 2)
                    {
                        int lower = Int32.Parse(nl[0]);
                        int upper = Int32.Parse(nl[1]);
                        for (int i = lower; i <= upper; i++)
                        {
                            tmp.Add(i);
                        }
                    }
                    else if (nl.Length == 1)
                    {
                        tmp.Add(Int32.Parse(nl[0]));
                    }
                }
                return tmp;
            }
        }
    }
}
