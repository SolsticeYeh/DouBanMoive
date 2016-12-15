using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace movie
{
    public class service
    {
        public static SQLiteAsyncConnection db { get; set; }
        public static int time = 0;
        public static double st = 0;
        public static int status = 0;
        public static int error = 0;
        public static int loadStatus = 0;
        public static async void check()
        {
            db = new SQLiteAsyncConnection("Date.db");
            await db.CreateTableAsync<LoveModel>();
            //var query = await(db.Table<LoveModel>().Where(v => v._Id >= 1)).ToListAsync();
        }
    }
    public class LoveModel
    {
        [PrimaryKey, AutoIncrement]
        public int _Id { get; set; }
        public string MidImage { get; set; }
        public string title { get; set; }
        public string original_title { get; set; }
        public string year { get; set; }
        public string BigImage { get; set; }
        public double rating { get; set; }
        public string summary { get; set; }
        public string genres { get; set; }
        public string DirectorsName { get; set; }
        public string DirectorsPic { get; set; }
        public string CastsName { get; set; }
        public string CastsPic { get; set; }
        public string id { get; set; }
    }
    public class LikeModel
    {
        [PrimaryKey, AutoIncrement]
        public int _Id { get; set; }
        public string MidImage { get; set; }
        public string title { get; set; }
        public string original_title { get; set; }
        public string year { get; set; }
        public string BigImage { get; set; }
        public double rating { get; set; }
        public string summary { get; set; }
        public string[] genres { get; set; }
        public List<People> Directors { get; set; }
        public List<People> Casts { get; set; }
        public string id { get; set; }
    }
    public class People
    {
        public string name { get; set; }
        public string Pic { get; set; }
    }
}
