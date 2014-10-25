using System.Collections.Generic;
using PetaPoco;

namespace gzsw.model
{
    public partial class SYS_MENU
    {
        [Ignore]
        public IList<SYS_MENU> Par_MenuList { get; set; }

        [Ignore]
        public SYS_FUNCTION Func { get; set; }
    }
}
