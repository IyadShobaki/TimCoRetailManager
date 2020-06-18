﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{

    [Authorize]

    public class UserController : ApiController
    {    
        [HttpGet]
        public UserModel GetById()
        {
            //will get the id of the user who logged in
            string userId = RequestContext.Principal.Identity.GetUserId();

            //to get UserData - > add reference to TRMDataanager.Library
            UserData data = new UserData();

            return data.GetUsersById(userId).First(); 
        }
    }
}