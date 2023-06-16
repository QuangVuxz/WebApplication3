using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public class WebApplication3Context : DbContext
    {
        public WebApplication3Context (DbContextOptions<WebApplication3Context> options)
            : base(options)
        {
        }

        public DbSet<WebApplication3.Models.UserInformationModel> UserInformationModel { get; set; } = default!;

        public DbSet<WebApplication3.Models.WorkLogModel>? WorkLogModel { get; set; }
    }
}
