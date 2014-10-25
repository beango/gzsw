using PetaPoco;

namespace gzsw.model
{
    public partial class SYS_ROLE
    {
        [Ignore]
        public SYS_ORGANIZE RoleORG { get; set; }
    }
}
