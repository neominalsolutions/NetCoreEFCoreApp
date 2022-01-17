using Microsoft.EntityFrameworkCore;
using NetCoreEFCoreApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Persistences.EFCore.Contexts
{
    public class AppDbContext: DbContext
    {
        // IOC ile uygulamının belirli bir ayara göre instance alınması için yazıyoruz. Yani biz startup dosyasında buranın Mysql, SqlServer, Postgres vs ile çalıştığını söyleceğiz. Bu arakadaş da ona göre instance alacak. opt => dediğimiz şey options.
        // uygulama otomatik instance alırken ise burayı kullanıyor. Startup dosyasına AddDbContext olarak AppDbContext tanımladıktan sonra her bir istek de sistem otomatik olarak buranın instance'ını alıcak.
        public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
        {
            
        }

        // Kendimiz AppDbContext'ten instance alırken burayı kullanacağız.
        public AppDbContext()
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // uygulama ilk ayağa kalkarken hangi db provider ile çalışacağınkı buradan söyleriz.

            optionsBuilder.UseLazyLoadingProxies(); // bu kısımda tüm ef core genelinde lazy loading aktif hale getirdik.

            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=TestEFCoreDb;Trusted_Connection=true;");

        
        }
    }
}
