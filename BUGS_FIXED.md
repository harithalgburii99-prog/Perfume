# Bug Report & Fix Summary — Royal Perfume Store

## 🔴 Critical (Security / Will Crash in Production)

| # | File | Bug | Fix |
|---|------|-----|-----|
| 1 | `Web.config` | **Hardcoded DB credentials** (`user id`, `pwd`) committed to source control | Replace with placeholder; use Azure App Settings or env vars for production |
| 2 | `Web.config` | `<compilation debug="true">` left on | Changed to `debug="false"` |
| 3 | `Web.config` | `<customErrors mode="Off">` — full stack traces shown to end users | Changed to `RemoteOnly` |
| 4 | `Web.config` | `<httpErrors errorMode="Detailed">` — server paths exposed | Changed to `Custom` with proper error pages |
| 5 | `Web.config` | No security headers (X-Frame-Options, XSS protection, etc.) | Added standard security headers |
| 6 | All forms | **No `@Html.AntiForgeryToken()`** on Login, Register, Create forms → **CSRF vulnerable** | Added to every POST form |
| 7 | `Perfumes/Index`, `Dashboard` | **Delete via GET link** — can be triggered by a third-party image/link → **CSRF** | Changed to POST form with anti-forgery token |
| 8 | `Cart/Index` | **Delete cart item via GET** → same CSRF issue | Changed to POST form |
| 9 | DB / Models | **Password stored in plain-text** (NVARCHAR 100 column) | Noted: must hash with BCrypt/PBKDF2 in controller before saving; column widened to 256 |

---

## 🟠 High (Runtime Crashes)

| # | File | Bug | Fix |
|---|------|-----|-----|
| 10 | `Perfumes/Index`, `Home/Index` | `item.Price.ToString("C")` on a **nullable `decimal?`** → `NullReferenceException` | Added `.HasValue` guard |
| 11 | `Home/Index`, `Perfumes/Index` | No null-check on `Model` — if DB is empty or query fails → crash on `foreach` | Added `if (Model != null && Model.Any())` guard |
| 12 | `Cart/Index` | `Model.Sum(...)` on null model → crash | Added null/empty guard |
| 13 | `Cart/Index` | **No `Layout` declaration** — page rendered without navbar/footer | Added `Layout = "~/Views/Shared/_Layout.cshtml"` |
| 14 | `Perfumes/Create` | `@Html.TextBoxFor(m => m.ImagePath)` → **compile error** because `Perfume` model had no `ImagePath` property | Added `ImagePath` to model + DB migration |
| 15 | `CartViewModel` | `Cart/Index` uses `CartViewModel` (PerfumeName, Price, CartId) but **model class didn't exist anywhere** in project → compile error | Created `CartViewModel` class |
| 16 | Namespace | Views declare `@model PerfumeStore.Models.*` but `Global.asax` shows assembly `pufumetxt` — namespace mismatch | All models placed under `PerfumeStore.Models` |

---

## 🟡 Medium (Logic / UX)

| # | File | Bug | Fix |
|---|------|-----|-----|
| 17 | `Account/Login` | Email input `type="text"` instead of `type="email"` | Changed to `type="email"` |
| 18 | `Account/Register` | `TextBoxFor` on email doesn't emit `type="email"` | Added explicit `type = "email"` in htmlAttributes |
| 19 | `Perfumes/Dashboard` | **No access control** in view — any user knowing the URL could view admin panel | Added Session role check; controller should also use `[Authorize]` |
| 20 | `Perfumes/Create` | No access control in view | Added admin Session guard |
| 21 | `Cart/Index` | Checkout button had no action — plain `<button>` with no form/href | Wired to POST `/Cart/Checkout` |
| 22 | DB (Cart table) | FK constraints on `Cart` were commented out | Added proper FK constraints |
| 23 | DB (Perfumes) | `ImagePath` column missing from table | Added in migration script |
| 24 | DB (Users) | No `UNIQUE` constraint on `Email` | Added `CONSTRAINT UQ_Users_Email UNIQUE ([Email])` |

---

## ✅ What Was Already Good

- Bootstrap 5 + RTL layout well structured
- Luxury gold/dark theme consistent across pages  
- Admin-only UI sections guarded with `Session["UserRole"]` checks
- Proper use of `Html.ActionLink` / `Url.Action` in most places
- Assembly binding redirects all present and correct

---

## 📁 Files Changed

```
Web.config                          ← security hardening
Models/Models.cs                    ← new file: all model classes + CartViewModel
Views/Account/Login.cshtml          ← CSRF token, email type fix, styled
Views/Account/Register.cshtml       ← CSRF token, email type fix
Views/Perfumes/Create.cshtml        ← CSRF token, admin guard, ImagePath fix
Views/Perfumes/Index.cshtml         ← POST delete, null guards, nullable Price fix
Views/Perfumes/Dashboard.cshtml     ← POST delete, admin guard, null guards
Views/Cart/Index.cshtml             ← layout fix, null guards, POST remove
Views/Home/Index.cshtml             ← null guards, nullable Price fix
Database_Schema_Fixed.sql           ← complete corrected schema + migration
```
