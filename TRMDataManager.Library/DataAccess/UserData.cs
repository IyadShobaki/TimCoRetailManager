using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            _config = config;
        }
        public List<UserModel> GetUsersById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var p = new { Id = Id };  //anonymous objetc with no name type 

           var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");

            return output;

        }
    }
}
