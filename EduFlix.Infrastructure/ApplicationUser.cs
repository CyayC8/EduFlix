using Microsoft.AspNetCore.Identity;

namespace EduFlix.Infrastructure;

// Eigen user-klasse (later uitbreidbaar met bv. een displaynaam).
// Identity is een infrastructuur-detail, vandaar dat dit hier leeft.
public class ApplicationUser : IdentityUser
{
}
