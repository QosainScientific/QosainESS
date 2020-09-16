using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QosainESSDesktop
{
    public class DoubleBufferPanel:Panel
    {
        public DoubleBufferPanel()
        {
            DoubleBuffered = true;
        }
    }
}
