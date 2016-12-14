using System;

namespace Jimmy
{
    [Serializable]
    public class CandidateValue
    {
        public double Numerical { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.Text))
                return this.Text;
            else
                return Numerical.ToString();
        }
    }
}
