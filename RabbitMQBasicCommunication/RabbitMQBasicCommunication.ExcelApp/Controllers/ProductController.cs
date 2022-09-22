using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQBasicCommunication.ExcelApp.Models;
using RabbitMQBasicCommunication.ExcelApp.Services;

namespace RabbitMQBasicCommunication.ExcelApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext appDbContext;
        private readonly RabbitMQPublisher rabbitMQPublisher;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext appDbContext, RabbitMQPublisher rabbitMQPublisher)
        {
            this.userManager = userManager;
            this.appDbContext = appDbContext;
            this.rabbitMQPublisher = rabbitMQPublisher;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid()}";

            UserFile userfile = new UserFile
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = Enums.FileStatus.Creating
            };

            await appDbContext.UserFiles.AddAsync(userfile);
            await appDbContext.SaveChangesAsync();

            rabbitMQPublisher.Publish(new Shared.CreateExcelMessage { FileId = userfile.Id });

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(ProductController.Files));

        }

        public async Task<IActionResult> Files()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            return View(await appDbContext.UserFiles.Where(x => x.UserId == user.Id).OrderByDescending(x=> x.Id).ToListAsync());
        }
    }
}
