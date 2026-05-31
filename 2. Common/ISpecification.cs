using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class ISpecification
    {
        public bool m_bPass = false;
        public double m_dMaximum = 999.9999D;
        public double m_dMinimum = 0.0D;

        public ISpecification(double dMinimum, double dMaximum, bool bPass = true)
        {
            m_dMinimum = dMinimum;
            m_dMaximum = dMaximum;
        }
        
        public bool IsInSpec(double dMeasure)
        {
            if (dMeasure > m_dMinimum
                && dMeasure < m_dMaximum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
