using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ripply.Components
{
    public interface ILinkSite
    {
        void Create(string store, string href, bool isItem);
    }
}
