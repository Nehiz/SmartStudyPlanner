# Authentication & Database Setup Guide

## Overview
The authentication system uses **ASP.NET Core Identity** with a custom `User` model extending `IdentityUser`. This provides out-of-the-box account management with secure password hashing, lockout policies, and user claims.

## Key Components

### 1. Models (in `Models/` folder)
- **User.cs** - Extends IdentityUser with FirstName, LastName, CreatedAt, and navigation properties to Tasks and Subjects
- **Subject.cs** - Represents a study course/subject owned by a user
- **StudyTask.cs** - Represents a task/assignment within a subject
- **InputModels/** - Form input validation models for Register/Login forms

### 2. Database Context (`Data/ApplicationDbContext.cs`)
- Inherits from `IdentityDbContext<User>` (includes built-in Identity tables)
- Configured with proper relationships and cascade delete behavior
- Auto-creates indexes on UserId and SubjectId for performance

### 3. Authentication Pages (`Components/Pages/Account/`)
- **Register.razor** - User registration page
- **Login.razor** - User login page
- Both use `[SupplyParameterFromForm]` for static SSR form binding in .NET 8

### 4. Middleware (`Program.cs`)
```csharp
// Key setup:
- AddDbContext<ApplicationDbContext>() with SQLite
- AddIdentity<User, IdentityRole>() with configured password rules
- AddCascadingAuthenticationState() for authentication state availability
- app.UseAuthentication() and app.UseAuthorization()
```

## Password Requirements
For development/learning purposes, passwords require:
- Minimum 6 characters
- No special characters, digits, or case requirements

**To change:** Edit `Program.cs` lines 23-30 in the Identity configuration.

## Database Migrations
SQLite database auto-migrates on app startup via `db.Database.Migrate()` in Program.cs (development only).

### Create New Migrations
When updating models, run:
```bash
cd SmartStudyPlanner
dotnet ef migrations add MigrationName
```

This creates a timestamped migration file in `Migrations/`. The migration auto-applies on next app run.

## How to Add Authentication to Components
To use the current authenticated user in a component or service:

```csharp
@inject AuthenticationStateProvider AuthenticationStateProvider
@inject UserManager<User> UserManager

// In @code:
private async Task GetCurrentUser()
{
    var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;
    if (user.Identity?.IsAuthenticated ?? false)
    {
        var appUser = await UserManager.FindByNameAsync(user.Identity.Name);
        // Use appUser here
    }
}
```

## Important Implementation Notes

### Cascade Deletes
When a user is deleted, all their Tasks and Subjects are automatically deleted (configured in DbContext). This is fine for learning but should be reviewed for production (consider soft deletes or archiving).

### Antiforgery
Antiforgery protection is **disabled** for Razor components (`.DisableAntiforgery()`) due to static SSR complexity in .NET 8. This is acceptable for learning but should be re-enabled in production.

### Static SSR
Register/Login pages use static server-side rendering (no `@rendermode InteractiveServer`). This is intentional for proper authentication context handling.

## Common Tasks

### Get Current User in a Service
```csharp
public class MyService
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MyService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return null;
        return await _userManager.FindByNameAsync(username);
    }
}
```

### Check User in Component
```razor
@if (context.User.Identity?.IsAuthenticated ?? false)
{
    <p>Welcome, @context.User.Identity.Name</p>
}
else
{
    <p>Please <a href="/Account/Login">log in</a></p>
}
```

## Next Steps for Your Team

1. **Efehi**: Build the Services/Repository layer using these models
2. **Camila**: Create `/planner` dashboard that consumes Efehi's services
3. **Stanford**: Polish UI/CSS for the authenticated experience

## Troubleshooting

- **"User not found"**: Make sure `UserManager.CreateAsync(user, password)` succeeded
- **Claims not showing**: Ensure `CascadingAuthenticationState` is registered
- **Password validation fails**: Check password rules in Program.cs Identity configuration
