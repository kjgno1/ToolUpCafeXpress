using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolUpCafePress;

namespace ToolUpCafeXpress
{
    class DatabaseContext : DbContext
    {
        public DatabaseContext() :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "db/app.db", ForeignKeys = true }.ConnectionString
            }, true)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ImageInfo> ImageInfoMaster { get; set; }

        public List<ImageInfo> getListNotUploaded()
        {
            return Database.SqlQuery<ImageInfo>("SELECT ID,NAME,URL,Descriptions,Tags,status FROM TBL_IMAGE_INFO WHERE status = 0").ToList();
        }


        public void UpdateStatus(String id, String status)
        {
            Database.ExecuteSqlCommand("Update TBL_IMAGE_INFO SET STATUS ='" + status + "' where id='" + id + "'");
        }
        
        
    }
}
