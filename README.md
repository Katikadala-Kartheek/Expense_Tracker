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
