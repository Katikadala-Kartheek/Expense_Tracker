# WebApp

[HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
        string AssignedRole=model.Email.ToLower()=="admin1@gmail.com" ? "Admin" : "User";

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, AssignedRole);

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, AssignedRole));

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Login", "Account");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
    return View(model);
}



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("user", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
});


builder.Services.AddAuthentication();

builder.Services.AddAuthorization();





[Authorize]
[Authorize(policy: "Admin")]
[Area("Admin")]



<div class="collapse navbar-collapse" id="navbarNav">
	<ul class="navbar-nav ms-auto">
		@{
			@using Microsoft.AspNetCore.Identity
			@inject UserManager<IdentityUser> UserManager

			var user = await UserManager.GetUserAsync(User);
		}

		@if (User.IsInRole("Admin"))
		{
			<li class="nav-item"><a class="nav-link" asp-area="Admin" asp-controller="AdminHome" asp-action="Index">Admin</a></li>
		}
		<li class="nav-item"><a class="nav-link" href="@Url.Action("Index", "Expense")">Dashboard</a></li>
		<li class="nav-item"><a class="nav-link" href="@Url.Action("Create", "Expense")">Add Expense</a></li>
		<li class="nav-item"><a class="nav-link" href="@Url.Action("Index", "Graph")">Reports</a></li>


		@if (user != null)
		{
			<div class="right">
				<a class="btn btn-outline-danger" href="@Url.Action("Logout", "Account")">Logout</a>
			</div>
		}
		else
		{
			<div class="right">
				<a class="btn btn-outline-light" href="@Url.Action("Login", "Account")">Login</a>
				<a class="btn btn-outline-primary" href="@Url.Action("Register", "Account")">SignUp</a>
			</div>
		}
</div>
	</ul>
</div>


