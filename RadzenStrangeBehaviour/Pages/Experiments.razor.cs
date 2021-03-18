using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RadzenStrangeBehaviour.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RadzenStrangeBehaviour.Pages
{
    public partial class Experiments
    {
        [Inject] private UserManager<IdentityUser> _userManager { get; set; }
        [Inject] private IHttpContextAccessor _httpCtx { get; set; }        
        [Inject] private ApplicationDbContext _ctx { get; set; }
        string UserId => _httpCtx.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public class Emp
        {
            public string Name { get; set; }
            public string LastName { get; set; }
        }

        private int count = 0;
        private List<Emp> employees = new();

        private List<Emp> query = new List<Emp>(){
            new Emp{Name = "Andrej", LastName = "A"},
            new Emp{Name = "Adrian", LastName = "A"},
            new Emp{Name = "Alex", LastName = "E"},
            new Emp{Name = "Brane", LastName = "C"},
            new Emp{Name = "Bogdan", LastName = "D"},
            new Emp{Name = "Boio", LastName = "Z"},
            new Emp{Name = "Cene", LastName = "Y"},
            new Emp{Name = "Doris", LastName = "X"}};

        protected override async Task OnInitializedAsync()
        {
            await LoadData(new LoadDataArgs() { Skip = 0, Top = 5 });
        }

        private async Task LoadData(LoadDataArgs args)
        {
            var userId = UserId;
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var isAdmin = await _userManager.IsInRoleAsync(user, "admin"); // when you start filtering the app hangs here
            //var isAdmin = AsyncHelper.RunSync<bool>(() =>    _userManager.IsInRoleAsync(user, "admin") );

            //var someRole = await _ctx.Roles.FirstAsync().ConfigureAwait;
            //var someRole = await _ctx.Roles.FirstAsync().ConfigureAwait;
            var q = query.AsQueryable();
            if (!string.IsNullOrEmpty(args.Filter))
            {
                q = q.Where(x => x.Name.ToLower().Contains(args.Filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                q = q.OrderBy(args.OrderBy);
            }

            count = q.Count();
            employees = q.Skip(args.Skip.Value).Take(args.Top.Value).ToList();
            await InvokeAsync(StateHasChanged);
        }
    }
}
