using FrontToBack.DAL;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Services
{
    public class LayoutServices
    {
        public readonly AppDbContext _context;
        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        }
    }
}
