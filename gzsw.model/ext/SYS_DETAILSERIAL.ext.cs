using PetaPoco;

namespace gzsw.model
{
    public partial class SYS_DETAILSERIAL
    {
        [Ignore]
        public SYS_QUEUESERIAL SYS_QUEUESERIAL { get; set; }

        [Ignore]
        public SYS_DLSERIAL SYS_DLSERIAL { get; set; }
    }
}
