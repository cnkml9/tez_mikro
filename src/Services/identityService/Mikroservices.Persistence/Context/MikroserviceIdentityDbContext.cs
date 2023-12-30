using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mikroservices.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservices.Persistence.Context
{
    public class MikroserviceIdentityDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public MikroserviceIdentityDbContext(DbContextOptions options) : base(options)
        {

        }

     
    }
}
