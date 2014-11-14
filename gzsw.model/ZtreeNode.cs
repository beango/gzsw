using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public class ZtreeNode
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isParent { get; set; }
        public bool @checked { get; set; }
        public bool open { get; set; }
        public bool nocheck { get; set; }
        public bool chkDisabled { get; set; }
        public bool highlight { get; set; }
        public IList<ZtreeNode> children { get; set; }
    }

    public class ZtreeNode_ORG 
    {
        public byte? Org_LV { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool hasauth { get; set; }
        public bool isParent { get; set; }
        public bool @checked { get; set; }
        public bool open { get; set; }
        public bool nocheck { get; set; }
        public bool chkDisabled { get; set; }
        public bool highlight { get; set; }
        public IList<ZtreeNode_ORG> children { get; set; }
    }

    public class ZtreeNodeItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isParent { get; set; }

        public string pId { get; set; }

        public bool open { get; set; }
        public bool nocheck { get; set; }

        public bool enable { get; set; }

        public bool highlight { get; set; }
    }
}
