
using LearningRedisCaching.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace LearningRedisCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        //private readonly CultureInfo culture = new("en-US");
        private readonly ApplicationDbContext db;
        private readonly IConfiguration _configuration;
        //private static readonly object _lockObj = new();
        //private readonly IDistributedCache _cache;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<AnalyticsController> logger;

        public AnalyticsController(ApplicationDbContext db, IMemoryCache memoryCache, ILogger<AnalyticsController> logger
            , IConfiguration configuration)
        {
            this.db = db;
            this.logger = logger;
            _configuration = configuration;
            //_cache = cache;
            this.memoryCache = memoryCache;
        }



        [HttpPost]
        [Route("/Create-ArticleMatrix")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CreateArticleMatrix([FromBody] List<ArticleMatrix> matrixs)
        {
            logger.LogInformation("API call to Create Article Matrix");
            foreach (var matrix in matrixs)
            {
                var matrixCheck = db.ArticleMatrix.SingleOrDefault(x => x.Title == matrix.Title);

                if (matrixCheck != null)
                {
                }
                else
                {
                    db.ArticleMatrix.Add(matrix);
                }

            }
            db.SaveChanges();


            return Ok();

        }

        [HttpGet]
        [Route("/cache/{author}")]
        public ActionResult GetArticleMatrix(string author)
        {
            logger.LogInformation("API to getArticle Matrix");

            if(!memoryCache.TryGetValue(author, out List<ArticleMatrix> matrixList))
            {
                logger.LogInformation("Cache is not saved yet ===============> BEGINING SAVING SEQUENCE");
                matrixList = db.ArticleMatrix.Where(matrix => matrix.Author == author).ToList();

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {

                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                memoryCache.Set(author, matrixList, cacheExpirationOptions);
            }
            
            return Ok(matrixList);
        }

     

    }
}
