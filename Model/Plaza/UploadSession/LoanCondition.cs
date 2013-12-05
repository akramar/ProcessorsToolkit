using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model.Plaza.UploadSession
{
    public class LoanCondition
    {
        public enum PriorToTypes { D, F }

        public string UniqueId { get; private set; }
        public string CheckboxName { get; set; }
        public PriorToTypes PriorTo { get; set; }
        public int Number { get; set; }
        public string NumberStr { get { return Number.ToString("D3"); } }
        public string Description { get; set; }
        public string ConditionId { get; set; }
        public string DisplayNameShort
        {
            get
            {
                //make this count about 5 words instead, split then join while under char limit
                var shortDesc = Description.Substring(0, Math.Min(Description.Length, 70));
                return NumberStr + " - " + shortDesc + "...";
            }
        }

        public LoanCondition()
        { UniqueId = Guid.NewGuid().ToString("N"); }
    }
}
