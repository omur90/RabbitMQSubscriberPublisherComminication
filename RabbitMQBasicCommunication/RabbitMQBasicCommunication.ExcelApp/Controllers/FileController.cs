
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQBasicCommunication.ExcelApp.Hubs;
using RabbitMQBasicCommunication.ExcelApp.Models;

namespace RabbitMQBasicCommunication.ExcelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IHubContext<MyHub> _hubContext;

        public FileController(AppDbContext appDbContext, IHubContext<MyHub> hubContext)
        {
            this.appDbContext = appDbContext;
            this._hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await appDbContext.UserFiles.FirstAsync(x => x.Id == fileId);

            var filePath = userFile.FileName + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = Enums.FileStatus.Completed;

            await appDbContext.SaveChangesAsync();

            await _hubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile");

            return Ok();
        }
    }
}
