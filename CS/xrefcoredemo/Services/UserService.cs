using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace xrefcoredemo.Services {
    public interface IUserService {
        int GetCurrentUserId();
        string GetCurrentUserName();
    }

    public class UserService : IUserService {
        readonly IHttpContextAccessor contextAccessor;

        public UserService(IHttpContextAccessor contextAccessor) {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public int GetCurrentUserId() {
            var sidStr = contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);        
            return Convert.ToInt32(sidStr.Value, CultureInfo.InvariantCulture);
        }

        public string GetCurrentUserName() {
            return contextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        }
    }
}
