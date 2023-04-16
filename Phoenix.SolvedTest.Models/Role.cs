using Phoenix.Identity;
using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.Models
{
    public class Role : RoleBase
    {
        [MaxLength(100)]
        public string Description { get; set; }

    }
}
