using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class SYS_FUNCTION
    {
        [Ignore]
        public IList<SYS_FUNCTION> Par_FuncList { get; set; }
    }
}
