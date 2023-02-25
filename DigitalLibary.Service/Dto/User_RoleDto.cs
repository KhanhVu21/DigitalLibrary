using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class User_RoleDto
    {
        public User_RoleDto()
        {

        }
        public Guid Id { get; set; }
        public string IdRole { get; set; }
        public string IdUser { get; set; }
    }
}
