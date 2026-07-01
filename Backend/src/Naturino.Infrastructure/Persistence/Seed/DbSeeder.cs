using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Naturino.Domain.Entities;
using Naturino.Domain.Enums;

namespace Naturino.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    private const string SuperAdminEmail = "admin@naturino.com";
    private const string SuperAdminDefaultPassword = "ChangeMe123!";

    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        var permissions = await SeedPermissionsAsync(context);
        var roles = await SeedRolesAsync(context, permissions);
        await SeedSuperAdminAsync(context, roles, logger);
        await SeedSettingsAsync(context);
        await SeedPagesAsync(context);
        await SeedLanguagesAsync(context);
        await SeedThemesAsync(context);
        await SeedCertificatesAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedThemesAsync(ApplicationDbContext context)
    {
        if (await context.Themes.AnyAsync())
        {
            return;
        }

        var colorTokens = """
        {
          "brand": { "primary": "#0A4B3A", "secondary": "#487d25", "accent": "#f7a83b", "success": "#22C55E", "warning": "#F59E0B", "error": "#EF4444" },
          "surface": { "background": "#FAFAF7", "card": "#FFFFFF", "muted": "#F3F5EF", "border": "#E5E7EB", "hover": "#F8FAF5" },
          "text": { "heading": "#0F172A", "body": "#334155", "muted": "#64748B", "inverse": "#FFFFFF" }
        }
        """;

        var typographyTokens = """{ "fontScale": "medium", "headingWeight": "700", "bodyWeight": "400", "lineHeight": "1.6", "letterSpacing": "normal" }""";
        var radiusTokens = """{ "style": "soft", "sm": "8px", "md": "16px", "lg": "24px", "xl": "32px", "pill": "999px" }""";
        var shadowTokens = """{ "level": "medium", "sm": "0 4px 12px rgba(15,23,42,.06)", "md": "0 12px 32px rgba(15,23,42,.10)", "lg": "0 24px 64px rgba(15,23,42,.14)" }""";
        var buttonTokens = """{ "radius": "999px", "size": "medium", "primaryStyle": "solid", "secondaryStyle": "outline", "hoverEffect": "lift", "fontWeight": "600" }""";
        var brandingTokens = """{ "logoLight": "", "logoDark": "", "favicon": "", "brandName": "Naturino", "tagline": "Premium Pet Food Export" }""";

        context.Themes.Add(new Theme
        {
            Name = "Naturino Default",
            Slug = "naturino-default",
            IsActive = true,
            IsDarkMode = false,
            FontHeading = "Geist",
            FontBody = "Geist",
            ColorTokensJson = colorTokens,
            TypographyTokensJson = typographyTokens,
            RadiusTokensJson = radiusTokens,
            ShadowTokensJson = shadowTokens,
            ButtonTokensJson = buttonTokens,
            BrandingTokensJson = brandingTokens,
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedLanguagesAsync(ApplicationDbContext context)
    {
        if (await context.Languages.AnyAsync()) return;

        context.Languages.AddRange(
            new Language
            {
                Name = "O'zbek",
                NativeName = "O'zbek",
                Code = "uz",
                Locale = "uz-UZ",
                Flag = "🇺🇿",
                Direction = "ltr",
                IsDefault = true,
                IsActive = true,
                SortOrder = 0,
            },
            new Language
            {
                Name = "Russian",
                NativeName = "Русский",
                Code = "ru",
                Locale = "ru-RU",
                Flag = "🇷🇺",
                Direction = "ltr",
                IsDefault = false,
                IsActive = true,
                SortOrder = 1,
            },
            new Language
            {
                Name = "English",
                NativeName = "English",
                Code = "en",
                Locale = "en-US",
                Flag = "🇬🇧",
                Direction = "ltr",
                IsDefault = false,
                IsActive = true,
                SortOrder = 2,
            }
        );
    }

    private static async Task<List<Permission>> SeedPermissionsAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync())
        {
            return await context.Permissions.ToListAsync();
        }

        var definitions = new (string Code, string Module, string Description)[]
        {
            ("users.view", "Users", "View users"),
            ("users.create", "Users", "Create users"),
            ("users.edit", "Users", "Edit users"),
            ("users.delete", "Users", "Delete users"),

            ("roles.manage", "Roles", "Manage roles and their permissions"),

            ("products.view", "Products", "View products"),
            ("products.create", "Products", "Create products"),
            ("products.edit", "Products", "Edit products"),
            ("products.delete", "Products", "Delete products"),
            ("products.publish", "Products", "Publish/unpublish products"),

            ("categories.manage", "Categories", "Manage product categories"),

            ("pages.view", "Pages", "View CMS pages"),
            ("pages.edit", "Pages", "Edit CMS pages and sections"),
            ("pages.publish", "Pages", "Publish/unpublish CMS pages"),

            ("media.upload", "Media", "Upload media files"),
            ("media.delete", "Media", "Delete media files"),

            ("blogs.view", "Blogs", "View blog posts"),
            ("blogs.create", "Blogs", "Create blog posts"),
            ("blogs.edit", "Blogs", "Edit blog posts"),
            ("blogs.delete", "Blogs", "Delete blog posts"),
            ("blogs.publish", "Blogs", "Publish/unpublish blog posts"),

            ("certificates.view", "Certificates", "View certificates"),
            ("certificates.create", "Certificates", "Create certificates"),
            ("certificates.edit", "Certificates", "Edit certificates"),
            ("certificates.delete", "Certificates", "Delete certificates"),

            ("contacts.view", "Contacts", "View contact submissions"),
            ("contacts.manage", "Contacts", "Manage contact submission status"),

            ("seo.manage", "Seo", "Manage SEO metadata"),
            ("settings.manage", "Settings", "Manage site settings"),
        };

        var permissions = definitions
            .Select(d => new Permission { Code = d.Code, Module = d.Module, Description = d.Description })
            .ToList();

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();

        return permissions;
    }

    private static async Task<Dictionary<string, Role>> SeedRolesAsync(ApplicationDbContext context, List<Permission> permissions)
    {
        if (await context.Roles.AnyAsync())
        {
            return await context.Roles.ToDictionaryAsync(r => r.Name);
        }

        var permissionsByCode = permissions.ToDictionary(p => p.Code);

        var superAdmin = new Role { Name = "SuperAdmin", Description = "Full system access", IsSystemRole = true };
        var contentEditor = new Role { Name = "ContentEditor", Description = "Manages pages, blog posts, and media" };
        var productManager = new Role { Name = "ProductManager", Description = "Manages products and categories" };
        var viewer = new Role { Name = "Viewer", Description = "Read-only access" };

        AssignPermissions(superAdmin, permissions, permissionsByCode);
        AssignPermissions(contentEditor, permissions, permissionsByCode, code =>
            code.StartsWith("pages.") || code.StartsWith("blogs.") || code.StartsWith("media."));
        AssignPermissions(productManager, permissions, permissionsByCode, code =>
            code.StartsWith("products.") || code.StartsWith("categories."));
        AssignPermissions(viewer, permissions, permissionsByCode, code => code.EndsWith(".view"));

        var roles = new[] { superAdmin, contentEditor, productManager, viewer };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        return roles.ToDictionary(r => r.Name);
    }

    private static void AssignPermissions(
        Role role,
        List<Permission> allPermissions,
        Dictionary<string, Permission> permissionsByCode,
        Func<string, bool>? filter = null)
    {
        var selected = filter is null ? allPermissions : allPermissions.Where(p => filter(p.Code));

        foreach (var permission in selected)
        {
            role.RolePermissions.Add(new RolePermission { Role = role, Permission = permissionsByCode[permission.Code] });
        }
    }

    private static async Task SeedSuperAdminAsync(ApplicationDbContext context, Dictionary<string, Role> roles, ILogger logger)
    {
        if (await context.Users.AnyAsync(u => u.Email == SuperAdminEmail))
        {
            return;
        }

        var superAdminUser = new User
        {
            Email = SuperAdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(SuperAdminDefaultPassword),
            FirstName = "Super",
            LastName = "Admin",
            IsActive = true,
        };

        superAdminUser.UserRoles.Add(new UserRole { User = superAdminUser, Role = roles["SuperAdmin"] });

        context.Users.Add(superAdminUser);
        await context.SaveChangesAsync();

        logger.LogWarning(
            "Seeded SuperAdmin account {Email} with the default development password {Password} — rotate it immediately outside local dev.",
            SuperAdminEmail,
            SuperAdminDefaultPassword);
    }

    private static async Task SeedSettingsAsync(ApplicationDbContext context)
    {
        if (await context.Settings.AnyAsync())
        {
            return;
        }

        var defaults = new (string Key, string Value, SettingValueType Type)[]
        {
            ("SiteName", "Naturino", SettingValueType.String),
            ("ContactEmail", "info@naturino.com", SettingValueType.String),
            ("ContactPhone", "+998 00 000 00 00", SettingValueType.String),
            ("Address", "", SettingValueType.String),
            ("SocialLinks", "{}", SettingValueType.Json),
        };

        context.Settings.AddRange(defaults.Select(d => new Setting
        {
            GroupName = "General",
            Key = d.Key,
            Value = d.Value,
            ValueType = d.Type,
        }));

        await context.SaveChangesAsync();
    }

    private static async Task SeedPagesAsync(ApplicationDbContext context)
    {
        var existingSlugs = await context.Pages.Select(p => p.Slug).ToListAsync();

        var pageDefinitions = new (string Slug, string Title, bool IsHomePage, (SectionType Type, string Content)[] Sections)[]
        {
            ("", "Home", true, HomeSections()),
            ("about-us", "About Us", false, new[] { (SectionType.Hero, "{}"), (SectionType.About, "{}") }),
            ("products", "Products", false, new[] { (SectionType.Hero, "{}"), (SectionType.Products, "{}") }),
            ("certificates", "Certificates", false, new[] { (SectionType.Hero, "{}"), (SectionType.Certificates, "{}") }),
            ("partnership", "Hamkorlik", false, PartnershipSections()),
            ("quality", "Sifat", false, QualitySections()),
            ("blog", "Blog / News", false, new[] { (SectionType.Hero, "{}") }),
            ("contact", "Contact Us", false, new[] { (SectionType.Hero, "{}"), (SectionType.Contact, "{}") }),
        };

        foreach (var definition in pageDefinitions)
        {
            if (existingSlugs.Contains(definition.Slug))
            {
                continue;
            }

            var page = new Page
            {
                Slug = definition.Slug,
                Title = definition.Title,
                IsHomePage = definition.IsHomePage,
                IsPublished = true,
            };

            for (var i = 0; i < definition.Sections.Length; i++)
            {
                page.Sections.Add(new PageSection
                {
                    Page = page,
                    SectionType = definition.Sections[i].Type,
                    SortOrder = i,
                    IsEnabled = true,
                    Content = definition.Sections[i].Content,
                });
            }

            context.Pages.Add(page);
        }

        await context.SaveChangesAsync();
    }

    private static (SectionType Type, string Content)[] HomeSections() => new[]
    {
        (SectionType.Hero, """
        {
          "banners": [
            {
              "isActive": true,
              "badge": "B2B ISHLAB CHIQARUVCHI · EKSPORTCHI",
              "title": "Markaziy Osiyodan uy hayvonlari ozuqasi yetkazib beruvchi ",
              "highlight": "ishonchli hamkoringiz.",
              "subtitle": "Biz mushuk va itlar uchun eksportga tayyor quruq va ho'l ozuqa ishlab chiqaramiz — sertifikatlangan sifat, doimiy yetkazib berish va sizning marjangizni himoya qiluvchi private-label moslashuvchanligi.",
              "primaryButtonText": "Mahsulotlar",
              "primaryButtonUrl": "/products",
              "secondaryButtonText": "Ariza qoldirish",
              "secondaryButtonUrl": "/contact",
              "imageStats": [
                { "value": "20+", "label": "EKSPORT DAVLATLARI" },
                { "value": "12k t", "label": "YILLIK QUVVAT" },
                { "value": "ISO", "label": "22000 SERTIFIKATI" }
              ],
              "checklist": [
                "ISO 22000 sertifikatlangan",
                "MOQ 5 tonnadan",
                "FOB va CIF qo'llab-quvvatlanadi",
                "Private-label OEM",
                "Veterinariya tasdig'i",
                "24 soat ichida javob"
              ]
            }
          ]
        }
        """),
        (SectionType.TrustBar, """
        {
          "eyebrow": "XALQARO STANDARTLAR BILAN TASDIQLANGAN",
          "badges": ["ISO 22000", "HACCP", "GMP+", "EAC", "VETERINARIYA TASDIG'I"],
          "highlight": "20+ DAVLATGA EKSPORT"
        }
        """),
        (SectionType.Stats, """
        {
          "items": [
            { "icon": "factory", "value": "12 000+", "label": "Tonna / yil quvvat" },
            { "icon": "globe", "value": "20+", "label": "Eksport davlatlari" },
            { "icon": "badge", "value": "12", "label": "Yillik tajriba" },
            { "icon": "box", "value": "40+", "label": "Mahsulot turlari (SKU)" }
          ]
        }
        """),
        (SectionType.Features, """
        {
          "eyebrow": "NEGA STEPPE NUTRITION",
          "title": "Faqat mahsulot emas — ishonchli biznes hamkor.",
          "subtitle": "Distribyutorlar va importyorlar bizdan shunchaki uy hayvonlari ozuqasidan ko'proq narsa oladi — doimiy sifat, barqaror yetkazib berish va marjangizni himoya qiluvchi private-label moslashuvchanligi."
        }
        """),
        (SectionType.Comparison, """
        {
          "eyebrow": "NEGA NATURINO",
          "title": "Nega distribyutorlar Naturino'ni tanlaydi?",
          "subtitle": "Naturino distribyutorlar va importyorlarga ishonchli ta'minot, xalqaro standartlar, eksport tajribasi va doimiy kengayib boruvchi mahsulot assortimenti orqali o'sishga yordam beradi.",
          "leftLabel": "BOSHQA YETKAZIB BERUVCHILAR",
          "leftTitle": "Bozordagi odatiy muammolar",
          "leftItems": ["Beqaror ta'minot rejalashtirish", "Sifat nazoratining bir xil emasligi", "Cheklangan mahsulot assortimenti", "Eksport tajribasining yetishmasligi", "Distribyutorlarga zaif qo'llab-quvvatlash", "Noaniq yetkazib berish muddatlari"],
          "rightBadge": "NATURINO",
          "rightLabel": "ISHONCHLI EKSPORT HAMKORI",
          "rightTitle": "Naturino bilan barqaror o'sish",
          "rightItems": ["Yiliga 12 000+ tonna ishlab chiqarish quvvati", "20+ bozorda eksport tajribasi", "ISO 22000 va HACCP standartlari", "40+ mahsulot turi (SKU)", "Barqaror ishlab chiqarish rejalashtirish", "Uzoq muddatli biznes hamkorlik", "Marketing va distribyutorlarga qo'llab-quvvatlash", "Eksportga tayyor hujjatlar"]
        }
        """),
        (SectionType.Products, """
        {
          "eyebrow": "MAHSULOTLARIMIZ",
          "title": "Eksportga tayyor asosiy yo'nalishlar",
          "subtitle": "Sertifikatlangan retseptlar, doimiy sifat. To'liq katalogda 40+ SKU.",
          "buttonText": "Barcha mahsulotlar",
          "buttonUrl": "/products"
        }
        """),
        (SectionType.PrivateLabel, """
        {
          "eyebrow": "EXPORT PARTNERSHIP",
          "title": "Nega distribyutorlar Naturino'ni tanlaydi?",
          "subtitle": "Naturino mushuk va itlar uchun premium ozuqa mahsulotlarini ishlab chiqaradi va xalqaro bozorlarga yetkazib beradi. Distribyutorlar va importyorlar uchun barqaror ta'minot, sifat nazorati va eksport hujjatlari bilan to'liq qo'llab-quvvatlash taqdim etiladi.",
          "steps": [
            { "number": "01", "label": "Eksport bozorlari", "title": "Eksport bozorlari", "icon": "formula", "description": "20+ davlatdagi hamkorlar va eksport tajribasi bilan xalqaro bozorlarda faoliyat yuritamiz." },
            { "number": "02", "label": "Ishlab chiqarish quvvati", "title": "Ishlab chiqarish quvvati", "icon": "packaging", "description": "Yiliga 12 000+ tonna ishlab chiqarish imkoniyati bilan barqaror ta'minotni ta'minlaymiz." },
            { "number": "03", "label": "Sifat nazorati", "title": "Sifat nazorati", "icon": "quality", "description": "Mahsulotlar ISO 22000, HACCP va xalqaro standartlar asosida nazorat qilinadi." },
            { "number": "04", "label": "Barqaror ta'minot", "title": "Barqaror ta'minot", "icon": "export", "description": "Distribyutorlar uchun uzluksiz yetkazib berish, eksport hujjatlari va logistika qo'llab-quvvatlashi." }
          ],
          "ctaTitle": "Eksport hamkorligini boshlang",
          "ctaSubtitle": "Naturino jamoasi distribyutorlar va importyorlar bilan uzoq muddatli hamkorlikka tayyor.",
          "buttonText": "Hamkorlik haqida batafsil",
          "buttonUrl": "/partnership"
        }
        """),
        (SectionType.Process, """
        {
          "eyebrow": "JARAYON",
          "title": "Hamkorlik qanday boshlanadi",
          "steps": [
            { "number": "1", "title": "So'rov yuboring", "description": "Mahsulot, hajm va talablaringizni ayting." },
            { "number": "2", "title": "Namuna va taklif oling", "description": "Namuna yuboramiz, narx va shartlarni kelishamiz." },
            { "number": "3", "title": "Buyurtma va eksport", "description": "Ishlab chiqaramiz va hujjatlar bilan yetkazamiz." }
          ],
          "buttonText": "So'rovni boshlang",
          "buttonUrl": "/contact"
        }
        """),
        (SectionType.Quality, """
        {
          "eyebrow": "SIFAT BILAN BOSHQARILADI",
          "title": "Retseptlar veterinar-nutritsionistlar tomonidan ishlab chiqilgan.",
          "items": ["O'z laboratoriyamizda sifat nazorati", "Veterinar-nutritsionist nazorati", "AAFCO standartlariga to'liq moslik", "Har partiya sertifikatlanadi"]
        }
        """),
        (SectionType.Partners, """
        {
          "eyebrow": "HAMKORLAR",
          "title": "Xalqaro hamkorlar ishonadi",
          "logos": ["PETCO ASIA", "NORDIC PAWS", "MENA VET", "PRIMEPET", "ARALIA TRADE", "KALMAR FOODS"],
          "testimonials": [
            { "quote": "Doimiy sifat, partiyadan partiyaga, private-label moslashuvchanligi esa bizga o'z brendimizni 8 oyda ishga tushirish imkonini berdi.", "name": "Azamat R.", "role": "Distribyutor", "location": "Qozog'iston" },
            { "quote": "Hujjatlar birinchi konteyner bilan mukammal yetib keldi. Bojxonadan bironta kechikishsiz o'tdi.", "name": "Fatima A.", "role": "Importyor", "location": "BAA" },
            { "quote": "Har bir partiya laboratoriya hisobotlari bilan keladi. 3 yillik hamkorlikda bir marta ham xafa bo'lmadik.", "name": "Mehmet K.", "role": "Private-label brendi", "location": "Turkiya" }
          ]
        }
        """),
        (SectionType.CTA, """
        {
          "title": "Hamkorlikni boshlang. ",
          "highlight": "24 soat ichida",
          "titleEnd": " javob beramiz.",
          "subtitle": "Distribyutor yoki importyormisiz? Narx, MOQ va namuna uchun so'rov yuboring — eksport jamoamiz tezda bog'lanadi.",
          "buttonText": "Narx so'rash",
          "buttonUrl": "/contact"
        }
        """),
    };

    private static (SectionType Type, string Content)[] PartnershipSections() => new[]
    {
        (SectionType.Hero, """
        {
          "banners": [
            {
              "isActive": true,
              "badge": "B2B HAMKORLIK",
              "title": "Naturino mahsulotlarini o'z bozoringizga olib keling — ",
              "highlight": "ishonchli eksport hamkori.",
              "subtitle": "Distribyutorlar, importyorlar va chakana tarmoqlar uchun sertifikatlangan uy hayvonlari ozuqasi — barqaror yetkazib berish, raqobatbardosh narx va to'liq hujjatlar bilan.",
              "primaryButtonText": "Hamkorlikni boshlash",
              "primaryButtonUrl": "/contact",
              "secondaryButtonText": "Mahsulotlar katalogi",
              "secondaryButtonUrl": "/products",
              "imageStats": [
                { "value": "20+", "label": "EKSPORT BOZORLARI" },
                { "value": "12k t", "label": "YILLIK QUVVAT" },
                { "value": "ISO", "label": "SERTIFIKATLANGAN" }
              ],
              "checklist": [
                "ISO 22000 va HACCP sertifikatlangan",
                "MOQ 5 tonnadan",
                "FOB va CIF yetkazib berish",
                "To'liq eksport hujjatlari",
                "Veterinariya tasdig'i",
                "24 soat ichida javob"
              ]
            }
          ]
        }
        """),
        (SectionType.WhyPartner, """
        {
          "eyebrow": "NEGA NATURINO",
          "title": "Hamkorlik uchun ishonchli sabablar",
          "subtitle": "Biz bilan ishlash — barqaror sifat va uzoq muddatli biznes munosabatlari demakdir.",
          "cards": [
            { "icon": "factory", "title": "Sifatli ishlab chiqarish", "description": "Zamonaviy zavod, xalqaro standartlarga mos laboratoriya nazorati." },
            { "icon": "truck", "title": "Barqaror yetkazib berish", "description": "Doimiy ishlab chiqarish quvvati va aniq yetkazib berish muddatlari." },
            { "icon": "globe", "title": "Eksport tajribasi", "description": "20+ davlatga eksport tajribasi, bojxona va logistika bo'yicha to'liq qo'llab-quvvatlash." },
            { "icon": "support", "title": "Biznes qo'llab-quvvatlash", "description": "Marketing materiallari, narx siyosati va doimiy aloqa uchun shaxsiy menejer." }
          ]
        }
        """),
        (SectionType.WhoWeWorkWith, """
        {
          "eyebrow": "KIMLAR BILAN ISHLAYMIZ",
          "title": "Hamkorlik turlari",
          "subtitle": "Bizning eksport dasturimiz turli xil biznes modellariga moslashtirilgan.",
          "cards": [
            { "icon": "warehouse", "title": "Distribyutorlar", "description": "Mintaqaviy distribyutsiya huquqi va eksklyuziv shartlar." },
            { "icon": "document", "title": "Importyorlar", "description": "To'liq eksport hujjatlari va bojxona qo'llab-quvvatlash." },
            { "icon": "store", "title": "Chakana tarmoqlar", "description": "Katalogdagi mahsulotlar yoki maxsus assortiment bo'yicha yetkazib berish." },
            { "icon": "handshake", "title": "Pet shop tarmoqlari", "description": "Kichik va o'rta hajmdagi muntazam buyurtmalar uchun moslashuvchan shartlar." }
          ]
        }
        """),
        (SectionType.ProductRange, """
        {
          "eyebrow": "MAHSULOT ASSORTIMENTI",
          "title": "Eksportga tayyor mahsulot turlari",
          "subtitle": "Quruq va ho'l ozuqa — mushuk va itlar uchun barcha yosh guruhlarida.",
          "categorySlugs": []
        }
        """),
        (SectionType.Process, """
        {
          "eyebrow": "JARAYON",
          "title": "Hamkorlik qanday boshlanadi",
          "steps": [
            { "number": "1", "title": "Murojaat", "description": "So'rovingizni yuboring — hajm, mahsulot turi va bozor haqida ma'lumot bering." },
            { "number": "2", "title": "Mahsulotni baholash", "description": "Namuna yuboramiz, sifat va spetsifikatsiyalarni ko'rib chiqasiz." },
            { "number": "3", "title": "Tijoriy muzokara", "description": "Narx, MOQ va yetkazib berish shartlarini kelishamiz." },
            { "number": "4", "title": "Buyurtma va logistika", "description": "Ishlab chiqarish, qadoqlash va eksport hujjatlarini tayyorlaymiz." },
            { "number": "5", "title": "Uzoq muddatli hamkorlik", "description": "Muntazam yetkazib berish va doimiy biznes qo'llab-quvvatlash." }
          ],
          "buttonText": "So'rovni boshlang",
          "buttonUrl": "/contact"
        }
        """),
        (SectionType.Stats, """
        {
          "items": [
            { "icon": "globe", "value": "20+", "label": "Eksport bozorlari" },
            { "icon": "factory", "value": "12 000+", "label": "Tonna / yil quvvat" },
            { "icon": "badge", "value": "ISO", "label": "22000 standartlari" },
            { "icon": "support", "value": "24/7", "label": "B2B qo'llab-quvvatlash" }
          ]
        }
        """),
        (SectionType.Certificates, """
        {
          "title": "Sertifikatlar",
          "subtitle": "Xalqaro sifat va xavfsizlik standartlariga muvofiqlik",
          "items": [
            { "name": "ISO 22000" },
            { "name": "HACCP" },
            { "name": "HALAL" }
          ]
        }
        """),
        (SectionType.ExportCapabilities, """
        {
          "eyebrow": "EKSPORT IMKONIYATLARI",
          "title": "Eksportni soddalashtiramiz",
          "subtitle": "Hujjatlardan logistikagacha — barcha bosqichda yordam beramiz.",
          "cards": [
            { "icon": "document", "title": "Eksport hujjatlari", "description": "Sertifikatlar, invoyslar va bojxona hujjatlarini to'liq tayyorlaymiz." },
            { "icon": "truck", "title": "Xalqaro yetkazib berish", "description": "FOB va CIF shartlarida dengiz va quruqlik orqali yetkazib berish." },
            { "icon": "box", "title": "Yirik hajmdagi buyurtmalar", "description": "Konteyner hajmidagi buyurtmalarni barqaror ishlab chiqarish." },
            { "icon": "chart", "title": "Bozor qo'llab-quvvatlash", "description": "Marketing materiallari va mahalliy bozor tahlili bilan yordam." },
            { "icon": "warehouse", "title": "Logistika yordami", "description": "Eksport-import jarayonida tajribali logistika hamkorlari." }
          ]
        }
        """),
        (SectionType.Gallery, """
        {
          "eyebrow": "ZAVODIMIZ",
          "title": "Ishlab chiqarish galereyasi",
          "images": [
            { "category": "Ishlab chiqarish", "caption": "Ishlab chiqarish liniyasi" },
            { "category": "Qadoqlash", "caption": "Avtomatik qadoqlash" },
            { "category": "Sifat nazorati", "caption": "Laboratoriya tekshiruvi" },
            { "category": "Ombor", "caption": "Tayyor mahsulot ombori" },
            { "category": "Logistika", "caption": "Eksport uchun yuklash" }
          ]
        }
        """),
        (SectionType.FAQ, """
        {
          "eyebrow": "SAVOL-JAVOBLAR",
          "title": "Tez-tez so'raladigan savollar",
          "items": [
            { "question": "Minimal buyurtma hajmi qancha?", "answer": "Mahsulot turiga qarab MOQ 5 tonnadan boshlanadi." },
            { "question": "Qaysi yetkazib berish shartlari mavjud?", "answer": "FOB va CIF shartlarida xalqaro yetkazib berishni qo'llab-quvvatlaymiz." },
            { "question": "Private-label xizmati bormi?", "answer": "Hozircha biz faqat o'z brendimiz ostida eksport qilamiz, private-label ishlab chiqarish xizmatini taqdim etmaymiz." },
            { "question": "Namuna olish mumkinmi?", "answer": "Ha, tijoriy muzokaradan oldin namuna yuboramiz." }
          ]
        }
        """),
        (SectionType.CTA, """
        {
          "title": "Hamkorlikni boshlang. ",
          "highlight": "24 soat ichida",
          "titleEnd": " javob beramiz.",
          "subtitle": "Distribyutor, importyor yoki chakana tarmoqsiz? Narx, MOQ va namuna uchun so'rov yuboring.",
          "buttonText": "Hamkorlikni so'rash",
          "buttonUrl": "/contact"
        }
        """),
    };

    private static async Task SeedCertificatesAsync(ApplicationDbContext context)
    {
        if (await context.Certificates.AnyAsync()) return;

        var certs = new[]
        {
            new Certificate
            {
                Title = "ISO 22000 — Oziq-ovqat xavfsizligi menejment tizimi",
                Description = "Oziq-ovqat xavfsizligi menejment tizimiga oid xalqaro standart. Ishlab chiqarishning barcha bosqichlarida xavfsizlik nazoratini ta'minlaydi.",
                IssuedBy = "Bureau Veritas Certification",
                CertificateNumber = "BV-ISO22000-2024-UZ",
                IssueDate = new DateOnly(2024, 3, 15),
                ExpiryDate = new DateOnly(2027, 3, 14),
                Icon = "shield",
                Category = "quality",
                Scope = "international",
                VerificationUrl = "https://www.bureauveritas.com/",
                TranslationsJson = """{"ru":{"title":"ISO 22000 — Система менеджмента безопасности пищевых продуктов","description":"Международный стандарт по системам менеджмента безопасности пищевых продуктов."},"en":{"title":"ISO 22000 — Food Safety Management System","description":"International standard for food safety management systems covering all stages of production."}}""",
                SortOrder = 0,
                IsActive = true,
            },
            new Certificate
            {
                Title = "HACCP — Xavflarni tahlil qilish va kritik nazorat nuqtalari",
                Description = "Oziq-ovqat xavfsizligini ta'minlashning profilaktik yondashuvi. Biologik, kimyoviy va jismoniy xavflarni nazorat qiladi.",
                IssuedBy = "SGS Certification Services",
                CertificateNumber = "SGS-HACCP-2024-001",
                IssueDate = new DateOnly(2024, 1, 10),
                ExpiryDate = new DateOnly(2026, 1, 9),
                Icon = "shield",
                Category = "safety",
                Scope = "international",
                VerificationUrl = "https://www.sgs.com/",
                TranslationsJson = """{"ru":{"title":"HACCP — Анализ рисков и критические контрольные точки","description":"Превентивный подход к обеспечению безопасности пищевых продуктов."},"en":{"title":"HACCP — Hazard Analysis and Critical Control Points","description":"Preventive approach to food safety managing biological, chemical and physical hazards."}}""",
                SortOrder = 1,
                IsActive = true,
            },
            new Certificate
            {
                Title = "HALOL sertifikati — Islom talablariga muvofiqlik",
                Description = "Mahsulotlar islom talablariga to'liq mos tarzda ishlab chiqarilishini tasdiqlaydi. MDH va Yaqin Sharq bozorlariga eksport uchun majburiy.",
                IssuedBy = "O'zbekiston Musulmonlari Idorasi",
                CertificateNumber = "UMI-HALAL-2024-0892",
                IssueDate = new DateOnly(2024, 6, 1),
                ExpiryDate = new DateOnly(2025, 5, 31),
                Icon = "seal",
                Category = "halal",
                Scope = "cis",
                TranslationsJson = """{"ru":{"title":"Сертификат ХАЛЯЛЬ — Соответствие требованиям ислама","description":"Подтверждает, что продукция производится в полном соответствии с исламскими требованиями."},"en":{"title":"HALAL Certificate — Islamic Compliance","description":"Confirms that products are manufactured in full compliance with Islamic requirements."}}""",
                SortOrder = 2,
                IsActive = true,
            },
            new Certificate
            {
                Title = "Veterinariya sertifikati — O'zbekiston Respublikasi",
                Description = "O'zbekiston Qishloq xo'jaligi vazirligi tomonidan berilgan veterinariya sog'liqni saqlash sertifikati. Uy hayvonlari ozuqasini eksport qilish uchun talab qilinadi.",
                IssuedBy = "O'zbekiston Qishloq xo'jaligi vazirligi — Davlat veterinariya xizmati",
                CertificateNumber = "DVX-2024-PET-4471",
                IssueDate = new DateOnly(2024, 2, 20),
                ExpiryDate = new DateOnly(2025, 2, 19),
                Icon = "document",
                Category = "veterinary",
                Scope = "uzbekistan",
                TranslationsJson = """{"ru":{"title":"Ветеринарный сертификат — Республика Узбекистан","description":"Ветеринарный санитарный сертификат, выданный Министерством сельского хозяйства."},"en":{"title":"Veterinary Certificate — Republic of Uzbekistan","description":"Veterinary sanitary certificate issued by the Ministry of Agriculture for pet food export."}}""",
                SortOrder = 3,
                IsActive = true,
            },
            new Certificate
            {
                Title = "EAC — Yevroosiyo muvofiqlik belgisi",
                Description = "Mahsulot Yevroosiyo iqtisodiy ittifoqi texnik reglamentlariga muvofiqligini tasdiqlovchi sertifikat. Rossiya, Qozog'iston, Belarus va boshqa EAIU davlatlariga eksport uchun zarur.",
                IssuedBy = "Yevroosiyo iqtisodiy ittifoqi akkreditatsiya markazi",
                CertificateNumber = "EAC-TR-CU-2024-PF-001",
                IssueDate = new DateOnly(2023, 11, 5),
                ExpiryDate = new DateOnly(2026, 11, 4),
                Icon = "seal",
                Category = "export",
                Scope = "cis",
                TranslationsJson = """{"ru":{"title":"EAC — Знак соответствия Евразийского союза","description":"Сертификат подтверждает соответствие продукции техническим регламентам ЕАЭС."},"en":{"title":"EAC — Eurasian Conformity Mark","description":"Confirms product compliance with Eurasian Economic Union technical regulations for export."}}""",
                SortOrder = 4,
                IsActive = true,
            },
            new Certificate
            {
                Title = "GMP+ — Yaxshi ishlab chiqarish amaliyoti",
                Description = "Oziq-ovqat xavfsizligi uchun yaxshi ishlab chiqarish amaliyoti sertifikati. Xom ashyo tanlovidan tayyor mahsulot qadoqlashgacha bo'lgan butun jarayonni qamrab oladi.",
                IssuedBy = "GMP+ International B.V.",
                CertificateNumber = "GMPPLUS-FC-2024-1183",
                IssueDate = new DateOnly(2024, 4, 8),
                ExpiryDate = new DateOnly(2026, 4, 7),
                Icon = "award",
                Category = "quality",
                Scope = "eu",
                VerificationUrl = "https://www.gmpplus.org/",
                TranslationsJson = """{"ru":{"title":"GMP+ — Надлежащая производственная практика","description":"Сертификат надлежащей производственной практики, охватывающий весь производственный процесс."},"en":{"title":"GMP+ — Good Manufacturing Practice","description":"Good manufacturing practice certificate covering the entire production process from raw materials to packaging."}}""",
                SortOrder = 5,
                IsActive = true,
            },
        };

        context.Certificates.AddRange(certs);
        await context.SaveChangesAsync();
    }

    private static (SectionType Type, string Content)[] QualitySections() => new[]
    {
        (SectionType.Hero, """
        {
          "banners": [
            {
              "isActive": true,
              "badge": "SIFAT NAZORATI",
              "title": "Har bir mahsulotda barqaror sifat — ",
              "highlight": "ishonchli ishlab chiqarish standartlari.",
              "subtitle": "Naturino xom ashyo tanlovidan tayyor mahsulotgacha bo'lgan har bir bosqichni nazorat qiladi — sifat, xavfsizlik va barqarorlikni ta'minlash uchun.",
              "primaryButtonText": "Sertifikatlarni ko'rish",
              "primaryButtonUrl": "#sertifikatlar",
              "secondaryButtonText": "Ishlab chiqarishni ko'rish",
              "secondaryButtonUrl": "#galereya",
              "imageStats": [
                { "value": "20+", "label": "EKSPORT BOZORLARI" },
                { "value": "12k t", "label": "YILLIK QUVVAT" },
                { "value": "ISO", "label": "XALQARO STANDART" }
              ],
              "checklist": [
                "Har partiya laboratoriya tekshiruvidan o'tadi",
                "Veterinar-nutritsionist nazorati",
                "AAFCO standartlariga moslik",
                "To'liq hujjatlashtirilgan jarayon",
                "Xalqaro sertifikatlar",
                "Eksportga tayyor sifat"
              ]
            }
          ]
        }
        """),
        (SectionType.WhyPartner, """
        {
          "eyebrow": "SIFAT TAMOYILLARI",
          "title": "Sifat tamoyillari",
          "subtitle": "Har bir mahsulot xavfsizlik, barqarorlik va ozuqaviy qiymatni ta'minlash uchun qattiq standartlar asosida ishlab chiqiladi.",
          "cards": [
            { "icon": "badge", "title": "Tanlangan xom ashyo", "description": "Faqat sifat talablariga javob beradigan tekshirilgan yetkazib beruvchilardan xom ashyo." },
            { "icon": "factory", "title": "Nazorat qilinadigan ishlab chiqarish", "description": "Har bir bosqich standart operatsion protseduralar asosida nazorat qilinadi." },
            { "icon": "shield", "title": "Laboratoriya testlari", "description": "Har partiya o'z laboratoriyamizda mikrobiologik va ozuqaviy tahlildan o'tadi." },
            { "icon": "globe", "title": "Eksportga tayyor mahsulot", "description": "Xalqaro standartlarga mos, eksport uchun to'liq hujjatlashtirilgan mahsulot." }
          ]
        }
        """),
        (SectionType.Process, """
        {
          "eyebrow": "JARAYON",
          "title": "Sifat nazorati jarayoni",
          "steps": [
            { "number": "1", "title": "Xom ashyo qabul qilish", "description": "Yetkazib beruvchilardan kelgan xom ashyo dastlabki tekshiruvdan o'tadi." },
            { "number": "2", "title": "Retsept tayyorlash", "description": "Veterinar-nutritsionistlar tomonidan tasdiqlangan retsept bo'yicha aralashtirish." },
            { "number": "3", "title": "Ishlab chiqarish jarayoni", "description": "Standart operatsion protseduralar asosida nazorat qilinadigan ishlab chiqarish." },
            { "number": "4", "title": "Qadoqlash nazorati", "description": "Qadoqlash sifati va og'irligi har partiyada tekshiriladi." },
            { "number": "5", "title": "Tayyor mahsulot testi", "description": "Yakuniy mahsulot laboratoriya tahlilidan o'tkaziladi." },
            { "number": "6", "title": "Ombor va eksportga tayyorlash", "description": "Saqlash standartlariga mos omborlashtirish va eksport hujjatlarini tayyorlash." }
          ],
          "buttonText": "Sertifikatlarni ko'rish",
          "buttonUrl": "#sertifikatlar"
        }
        """),
        (SectionType.Certificates, """
        {
          "title": "Sertifikatlar va muvofiqlik",
          "subtitle": "Xalqaro sifat, xavfsizlik va diniy standartlarga muvofiqlik",
          "items": [
            { "name": "ISO 22000" },
            { "name": "HACCP" },
            { "name": "HALAL" },
            { "name": "Veterinariya hujjatlari" },
            { "name": "Eksport sertifikatlari" },
            { "name": "Laboratoriya hisobotlari" }
          ]
        }
        """),
        (SectionType.Gallery, """
        {
          "eyebrow": "ISHLAB CHIQARISHDAN OLDIN VA KEYIN",
          "title": "Sifat — ishlab chiqarishning har bosqichida",
          "images": [
            { "category": "Laboratoriya", "caption": "Mikrobiologik tahlil" },
            { "category": "Ishlab chiqarish liniyasi", "caption": "Avtomatlashtirilgan ishlab chiqarish" },
            { "category": "Qadoqlash", "caption": "Qadoqlash nazorati" },
            { "category": "Ombor", "caption": "Tayyor mahsulot ombori" },
            { "category": "Sifat nazorati", "caption": "Yakuniy tekshiruv" }
          ]
        }
        """),
        (SectionType.ExportCapabilities, """
        {
          "eyebrow": "EKSPORT SIFATI",
          "title": "Xalqaro bozorlar uchun tayyor",
          "subtitle": "Naturino mahsulotlari barqaror yetkazib berish, eksport hujjatlari va sifat kafolati bilan taqdim etiladi.",
          "cards": [
            { "icon": "document", "title": "Eksport hujjatlari", "description": "Har bir jo'natma uchun to'liq sertifikatlar va hujjatlar." },
            { "icon": "badge", "title": "Partiya nazorati", "description": "Har partiya alohida kodlanadi va laboratoriya hisoboti bilan birga keladi." },
            { "icon": "box", "title": "Barqaror qadoqlash", "description": "Uzoq masofali tashish uchun mo'ljallangan qadoqlash standartlari." },
            { "icon": "warehouse", "title": "Saqlash standartlari", "description": "Mahsulot sifatini saqlaydigan nazorat qilinadigan ombor sharoitlari." },
            { "icon": "truck", "title": "Logistikaga tayyorgarlik", "description": "Eksport jarayonida tajribali logistika hamkorlari bilan ishlaymiz." }
          ]
        }
        """),
        (SectionType.FAQ, """
        {
          "eyebrow": "SAVOL-JAVOBLAR",
          "title": "Sifat bo'yicha savollar",
          "items": [
            { "question": "Mahsulotlar qanday sifat nazoratidan o'tadi?", "answer": "Xom ashyo qabulidan tayyor mahsulotgacha har bosqichda laboratoriya va vizual nazorat amalga oshiriladi." },
            { "question": "Xalqaro sertifikatlaringiz bormi?", "answer": "Ha, ISO 22000, HACCP va HALAL sertifikatlarimiz mavjud." },
            { "question": "Eksport hujjatlari taqdim etiladimi?", "answer": "Ha, har bir jo'natma uchun to'liq eksport hujjatlari beriladi." },
            { "question": "Ishlab chiqarish partiyalari tekshiriladimi?", "answer": "Ha, har bir partiya alohida kodlanadi va laboratoriya tahlilidan o'tadi." },
            { "question": "Qadoqlash sifati qanday ta'minlanadi?", "answer": "Qadoqlash og'irligi va mustahkamligi har partiyada nazorat qilinadi." }
          ]
        }
        """),
        (SectionType.CTA, """
        {
          "title": "Ishonchli uy hayvonlari ozuqasi ishlab chiqaruvchisi bilan ",
          "highlight": "ishlang",
          "titleEnd": ".",
          "subtitle": "Barqaror sifat va eksportga tayyor yetkazib berish uchun Naturino bilan hamkorlik qiling.",
          "buttonText": "Aloqaga chiqish",
          "buttonUrl": "/contact"
        }
        """),
    };
}
