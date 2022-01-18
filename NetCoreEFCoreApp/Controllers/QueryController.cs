using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreEFCoreApp.Domain.Models;
using NetCoreEFCoreApp.Persistences.EFCore.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Controllers
{
    public class QueryController : Controller
    {
        private AppDbContext _db;
        // repositoryleri kullanarak araya interface koyacağız böylelikle dependency inversion prensibini uygulayacağız. DIP

        public QueryController(AppDbContext db) // Dependency Injection (DI consturctor üzerinden bir nesneye başka nesnenin instance gönderilmesine DI diyoruz.)
        {
            //_db = new AppDbContext();
            _db = db;
        }

        public IActionResult Index()
        {

    

            var  products =  _db.Products.ToList(); // lambda expression linq
            var products2 = (from p in _db.Products select p).ToList(); // raw linq
            // karmaşık group by ve join işlemleri varsa yukarıdaki gibi linqtoSql olarak kullanabiliriz.
            // lazy loading products ile birlikte categories geliyor.
            // Include ile navigation property üzerinden bağlamış olduk.
            // Eager Loading her zaman lazy loading göre daha performanslı bir yöntemdir.
            var productIncludeWithCategory = _db.Products.Include(x => x.Category).ToList();
            // install-package Microsoft.EntityFrameworkCore.Proxies eğer lazy loading aktif hale getirmek istersek bu versiyonda yukarıdaki paketi kurmamız gerekiyor. 


            // fiyatı 43 ile 78 arasında olanlar

            var products3 = _db.Products.Where(x => x.Price >= 43 && x.Price <= 78).ToList();
            // ürünleri fiyatına göre artandan azalana sıralama
            var products4 = _db.Products.OrderByDescending(x => x.Price).ToList();
            // isiminde ürün geçen products
            var products5 = _db.Products.Where(x => x.Name.Contains("ürün")).ToList();
            // 2.yöntem
            var products6 = _db.Products.Where(x => EF.Functions.Like(x.Name, "%ürün%")).ToList();
            // ürün ismi ü ile başlayanlar
            var products7 = _db.Products.Where(x => EF.Functions.Like(x.Name, "%ü"));
            var products8 = _db.Products.Where(x => x.Name.StartsWith("ü")).ToList();
            // ismi s ile biten ürünler
            var products9 = _db.Products.Where(x => EF.Functions.Like(x.Name, "s%")).ToList();
            var products10 = _db.Products.Where(x => x.Name.EndsWith("s")).ToList();

            // ürün id sine göre getirme Find ve FirstOrDefault kaydı bulamazsa null döndürür. Bulursa nesneyi attached eder. yani veri tabanına bağlı bir hale getirir.
            var product11 = _db.Products.Find("42AF1309-FFC6-4443-9209-1A2CA2C9C66E");
            // firstorDefault ile getirme
            var products12 = _db.Products.FirstOrDefault(x => x.Id == "42AF1309-FFC6-4443-9209-1A2CA2C9C66E");
            // tek bir kayı deöndürür yukarıdakilerden farklı olarak bulamaz ise hata döner.exception düşer. o yüzden kullanmıyoruz.
            var products13 = _db.Products.SingleOrDefault(x => x.Id == "42AF1309-FFC6-4443-9209-1A2CA2C9C66E");

            // ilk 5 ürünü çekme // ürün fiyatına göre asc olarak sıralanmış ilk 5 adet ürün. Take ile çalırken öncesinde orderBy sorgusu atalım
            var products14 = _db.Products.OrderBy(x => x.Price).Take(5).ToList();
            // sayfalam işlemleri için
            var products15 = _db.Products.OrderBy(x => x.Price).Skip(2).Take(2).ToList();
            // skip methodu ile kayıt atlatma işlemleri yani sqldeki offset işlemleri yaparız.
            // iki farklı alana göre kayıtlarımızı sırlamak için thenBy methodunu kullanırız.
            // veri tabanında aynı isimde çalışan varsa soy isimlerine göre azalandan artana sıralama yapmak için kullanılabilir.
            var products16 = _db.Products.OrderBy(x => x.Name).ThenBy(x => x.Price).ToList();
            // veri tabanında kayıt var mı yok mu diye sorgulamak için any methodu kullanılır
            var products17 = _db.Products.Any(x => x.Name.Contains("a")); // name alanında a geçen bir kayıt var mı. Any sonuç olarak true yada false değer döndürür.
            // select ile bir tablodaki belirli alanları çekebiliriz.
            var products18 = _db.Products.Select(x => x.Name).ToList(); // sadece name alanlarını çektik. 
            // birden fazla alanı çekmek istersek bu durumda ise new keyword ile anonim bir class içerisine alırız.
            var product19 = _db.Products.Select(x => new
            {
                Name = x.Name,
                Price = x.Price
            }).ToList();
            // select many ile ise bireçok ilişkili tablolarda koleksiyon içerisinde bir işlem yapabiliriz.

            // kategor'nin altındaki ürünlere bağlanıp fiyatı 50 tl üstünde olan ürünlerin filtrelenmesini sağlar.
            var products20 = _db.Categories
            .Include(x => x.Products)
            .SelectMany(x => x.Products)
            .Where(x => x.Price > 50)
            .ToList();

            /* SQL Query
             * SELECT [p].[Id], [p].[CategoryId], [p].[Name], [p].[Price], [p].[Stock]
FROM [Categories] AS [c]
INNER JOIN [Products] AS [p] ON [c].[Id] = [p].[CategoryId]
WHERE [p].[Price] > 50.0
             * 
             */
            // stoğuna göre ürünleri gruplama
            var products21 = _db.Products.GroupBy(x => x.Stock).Select(a => new
             {
                Count = a.Count(), // 2
                Name = a.Key // 32
 
             }).ToList();

            // Lambda join çok kullanmıyoruz. bunu yerine complex join işlemlerinde ya linq raw kullanıyoruz.
            var query = _db.Products
        .Join(
            _db.Categories,
            product => product.Category.Id,
            category => category.Id,
            (category, product) => new
            {
                CategoryName = category.Name,
                ProductName = product.Name
            }
        ).ToList();

            // IncludeYöntemi
            var query2 = _db.Products.Include(x => x.Category).Select(a => new
            {
                CategoryName = a.Category.Name,
                ProductName = a.Name
            }).ToList();

            // Linq ile sorgulama yöntemi
            var query3 = (from product in _db.Products
                          join category in _db.Categories on product.Category.Id equals category.Id
                          select new
                          {
                              CategoryName = category.Name,
                              ProductName = product.Name
                          }).ToList();


            // sum, count, avarage, max, min aggregate functions

            var totalUnitPrice = _db.Products.Sum(x => x.Price);
            // fiyatı 50 den büyük olanların sayısını getir
            var totalCount = _db.Products.Where(x=> x.Price > 50).Count();

            var avgs = _db.Products.Average(x => x.Price); // ortalama 1 ürüne ait birim maliyeti bulur
            var totalavgs = _db.Products.Average(x => x.Price * x.Stock); // stoktaki ürünlerin ortalama maliyeti
            var products23 = _db.Products.Where(x=> x.Price > _db.Products.Average(x => x.Price)).ToList();
            // ortalama ürün birim fiyatın üstündeki ürünlerin listesi ort ürün birim fiyat 30=> sql de subquery ile yaparız.
            var products24 = _db.Products.FirstOrDefault(x => x.Price == _db.Products.Max(x => x.Price));
            // ürün giyatı maksimum fiyat olan ürünü getir.

            // kategori getir fakat ürünleri fiyatına göre artandan azalana sıralı getir.

            var categories10 = _db.Categories.Include(x => x.Products).Select(x=> new Category { 
                Id = x.Id,
                Name = x.Name,
                Products = x.Products.OrderByDescending(y => y.Price).ToList()

            }).ToList();

            /* SQL Query
             * SELECT [c].[Id], [c].[Name], [t].[Id], [t].[CategoryId], [t].[Name], [t].[Price], [t].[Stock]
FROM [Categories] AS [c]
LEFT JOIN (
    SELECT [p].[Id], [p].[CategoryId], [p].[Name], [p].[Price], [p].[Stock]
    FROM [Products] AS [p]
) AS [t] ON [c].[Id] = [t].[CategoryId]
ORDER BY [c].[Id], [t].[Price] DESC, [t].[Id]
             * 
             */

            // veri çekilince Include ile tüm navigation peroperty olan koleksiyon alanları içerisinde ramde dönüp tek tek sıralayabilirz.
            // bu aşağıdaki yöntemi kullanmayalım.
            categories10.ForEach(a =>
            {

                a.Products.OrderByDescending(y => y.Price);
            });











            return View();
        }
    }
}
