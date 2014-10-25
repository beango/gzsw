using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public class UserFuncs
    {
        public int FUNCTION_ID { get; set; }

        public string FUNCTION_NAM { get; set; }

        public string FUNCTION_COD { get; set; }

        public Nullable<int> PAR_FUNCTION_ID { get; set; }

        public byte FUNCTION_TYP { get; set; }
    }

    public class UserOrgs
    {
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        public string PAR_ORG_ID { get; set; }

        public int ORG_LEVEL { get; set; }
    }
}
