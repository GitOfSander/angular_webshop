using Microsoft.EntityFrameworkCore;
using Site.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class Setting
    {
        SiteContext _context;

        public Setting(SiteContext context)
        {
            _context = context;
        }

        public string GetSettingValueByKey(string key, string linkedToType, int linkedToId)
        {
            Settings _setting = _context.Settings.FirstOrDefault(Setting => Setting.Key == key && Setting.LinkedToType == linkedToType && Setting.LinkedToId == linkedToId);

            return _setting != null ? _setting.Value : "";
        }

        public async Task<Settings> IncrementSettingValueByKeyAsync(string key, string linkedToType, int linkedToId)
        {
            return await _context.Settings.FromSql("Increase_Setting_Value @Key, @LinkedToType, @LinkedToId", new SqlParameter("Key", key), 
                                                                                                              new SqlParameter("LinkedToType", linkedToType), 
                                                                                                              new SqlParameter("LinkedToId", linkedToId)).FirstOrDefaultAsync();
        }
    }
}
