using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class ChildrenController(FamilyContext context) : ControllerBase
    {
        private readonly FamilyContext _context = context;
        private static int lastExportedId = 0;

        [HttpGet("childrenList")]
        public JsonResult GetChildren()
        {
            return new JsonResult(new { children = _context.Children.OrderByDescending(item => item.Id) });
        }

        [HttpDelete("deleteAll")]
        public JsonResult DeleteAll()
        {
            _context.Children.RemoveRange(_context.Children);
            _context.SaveChanges();
            return new JsonResult(new { children = _context.Children });
        }

        [HttpDelete("deleteById/{id:int}")]
        public JsonResult Delete(int id)
        {
            var toRemove = _context.Children.Find(id);
            if (toRemove != null)
            {
                _context.Children.Remove(toRemove);
                _context.SaveChanges();
            }
            return new JsonResult(new { children = _context.Children });
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitChild([FromBody] Child child)
        {
            child.Name = CapitalizeName(child.Name ?? "");
            if (!IsValidChild(child))
            {
                return new JsonResult(new { message = "Invalid data submitted", status =  "error" });
            }

            if (child.Name == "Bugs Bunny" && child.FavoriteAnimals != null && child.FavoriteAnimals.Contains("bunny"))
            {
                await _context.Children.AddAsync(child);
                await _context.SaveChangesAsync();
                return new JsonResult(new { message = "You are funny!\n\rData submitted successfully", status = "success" });
            }

            await _context.Children.AddAsync(child);
            await _context.SaveChangesAsync();
            return new JsonResult(new { message = "Data submitted successfully", children = _context.Children, status = "success" });
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportCsv([FromBody] ExportRequest request)
        {
            var children = request.ExportType == "all"
                ? await _context.Children.ToListAsync<Child>()
                : await _context.Children.Where(c => c.Id > lastExportedId).ToListAsync<Child>();

            if (children.Count > 0)
            {
                lastExportedId = children.Max(c => c.Id);
            }

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            var csvWriter = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture));

            csvWriter.WriteRecords(children);
            await writer.FlushAsync();
            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "children_data.csv");
        }

        private static string CapitalizeName(string name)
        {
            return string.Join(" ", name.Split(' ').Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));
        }

        private static bool IsValidChild(Child child)
        {
            if (string.IsNullOrWhiteSpace(child.Name) ||
                child.Name.Length < 2 ||
                string.IsNullOrWhiteSpace(child.Email) ||
                string.IsNullOrWhiteSpace(child.Gender) ||
                (child.FavoriteAnimals == null ||
                child.FavoriteAnimals.Length == 0) ||
                !MyRegex().IsMatch(child.Email))
            {
                return false;
            }

            var nameParts = child.Name.Split(' ');
            if (nameParts.Any(n => n.Length < 2))
            {
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        private static partial Regex MyRegex();
    }

    public class ExportRequest
    {
        public string? ExportType { get; set; }
    }

}
